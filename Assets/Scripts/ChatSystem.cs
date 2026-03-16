using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

public struct ChatRPCCommand : IRpcCommand
{
	public FixedString512Bytes Message;
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

	public void OnUpdate(ref SystemState state)
	{
		EntityCommandBuffer buffer = new EntityCommandBuffer(Allocator.Temp);
		foreach (var (request, command, entity) in SystemAPI.Query<ReceiveRpcCommandRequest, ChatRPCCommand>().WithEntityAccess())
		{
			SendRpc<ChatRPCCommand>(buffer, command);
			buffer.DestroyEntity(entity);
		}
		buffer.Playback(state.EntityManager);
		buffer.Dispose();
	}
}

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct ClientChatSystem : ISystem
{
	public static void SendRpc<T>(T command) where T : unmanaged, IComponentData
	{
		EntityCommandBuffer buffer = new EntityCommandBuffer(Allocator.Temp);

		Entity entity = buffer.CreateEntity();
		buffer.AddComponent(entity, new SendRpcCommandRequest());
		buffer.AddComponent(entity, command);
		
		buffer.Playback(ClientServerBootstrap.ClientWorld.EntityManager);
		buffer.Dispose();
	}

	public void OnUpdate(ref SystemState state)
	{
		EntityCommandBuffer buffer = new EntityCommandBuffer(Allocator.Temp);
		foreach (var (request, command, entity) in SystemAPI.Query<ReceiveRpcCommandRequest, ChatRPCCommand>().WithEntityAccess())
		{
			ChatUI.Instance?.DisplayMessage(command.Message.ToString());
			buffer.DestroyEntity(entity);
		}
		buffer.Playback(state.EntityManager);
		buffer.Dispose();
	}
}