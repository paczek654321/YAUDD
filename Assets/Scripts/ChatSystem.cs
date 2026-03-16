using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

public struct ChatRPCCommand : IRpcCommand
{
	public FixedString512Bytes Message;
	public int Target;
	public bool Exclusive;
}

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerChatSystem : ISystem
{
	private void SendRpc<T>(EntityCommandBuffer buffer, T command) where T : unmanaged, IComponentData
	{
		Entity entity = buffer.CreateEntity();
		buffer.AddComponent(entity, new SendRpcCommandRequest());
		buffer.AddComponent(entity, command);	
	}

	private void SendRpcIndividual<T>(EntityCommandBuffer buffer, T command, Entity connection) where T : unmanaged, IComponentData
	{
		Entity entity = buffer.CreateEntity();
		buffer.AddComponent(entity, new SendRpcCommandRequest{TargetConnection = connection});
		buffer.AddComponent(entity, command);
	}

	public void OnUpdate(ref SystemState state)
	{
		EntityCommandBuffer buffer = new(Allocator.Temp);
		foreach (var (request, command, entity) in SystemAPI.Query<ReceiveRpcCommandRequest, ChatRPCCommand>().WithEntityAccess())
		{
			if (command.Exclusive)
			{
				if (SystemAPI.GetComponent<NetworkId>(request.SourceConnection).Value != command.Target)
				{
					foreach (var (networkId, networkEntity) in SystemAPI.Query<NetworkId>().WithEntityAccess())
					{
						if (networkId.Value == command.Target)
						{
							SendRpcIndividual(buffer, command, networkEntity);		
							break;
						}
					}
				}
				SendRpcIndividual(buffer, command, request.SourceConnection);
			}
			else
			{
				SendRpc(buffer, command);
			}
			buffer.DestroyEntity(entity);
		}
		buffer.Playback(state.EntityManager);
		buffer.Dispose();
	}
}

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct ClientChatSystem : ISystem
{
	public void OnCreate(ref SystemState state)
	{
		ChatUI.Instance?.gameObject.SetActive(true);
	}

	//Messages are mirrored back to the client by the server to simplify display and propagation logic - This is suboptimal but its also simple and works
	public static void SendRpc<T>(T command) where T : unmanaged, IComponentData
	{
		EntityCommandBuffer buffer = new(Allocator.Temp);

		Entity entity = buffer.CreateEntity();
		buffer.AddComponent(entity, new SendRpcCommandRequest());
		buffer.AddComponent(entity, command);
		
		buffer.Playback(ClientServerBootstrap.ClientWorld.EntityManager);
		buffer.Dispose();
	}

	public void OnUpdate(ref SystemState state)
	{
		EntityCommandBuffer buffer = new(Allocator.Temp);
		foreach (var (_, command, entity) in SystemAPI.Query<ReceiveRpcCommandRequest, ChatRPCCommand>().WithEntityAccess())
		{
			ChatUI.Instance?.DisplayMessage(command.Message.ToString());
			buffer.DestroyEntity(entity);
		}
		buffer.Playback(state.EntityManager);
		buffer.Dispose();
	}
}