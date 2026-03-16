using Unity.Entities;
using Unity.NetCode;
using Unity.Collections;
using UnityEngine;

public struct GoInGameRpcCommand : IRpcCommand{}

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerInitSystem : ISystem
{
	const int MaxPlayers = 4;

	[Unity.Burst.BurstCompile]
	public void OnCreate(ref SystemState state)
	{
		Debug.Log("--- Starting a Server ---");
		state.RequireForUpdate<PrefabManager>();
	}

	[Unity.Burst.BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		PrefabManager prefabManager = SystemAPI.GetSingleton<PrefabManager>();
		EntityCommandBuffer buffer = new EntityCommandBuffer(Allocator.Temp);

		int numPlayers = SystemAPI.QueryBuilder().WithAll<NetworkStreamInGame>().Build().CalculateEntityCount();

		foreach (var (request, command, entity) in SystemAPI.Query<ReceiveRpcCommandRequest, GoInGameRpcCommand>().WithEntityAccess())
		{
			numPlayers += 1;

			if (numPlayers > MaxPlayers)
			{
				buffer.AddComponent<NetworkStreamRequestDisconnect>(request.SourceConnection);
			}
			else
			{
				buffer.AddComponent<NetworkStreamInGame>(request.SourceConnection);
				Entity player = buffer.Instantiate(prefabManager.Player);
				buffer.AddComponent(player, new GhostOwner{ NetworkId = SystemAPI.GetComponent<NetworkId>(request.SourceConnection).Value });

				buffer.AppendToBuffer(request.SourceConnection, new LinkedEntityGroup{Value = player});
			}
			buffer.DestroyEntity(entity);
		}
		buffer.Playback(state.EntityManager);
		buffer.Dispose();
	}
}

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct ClientInitSystem : ISystem
{
	private void SendRpc<T>(EntityCommandBuffer buffer, T command) where T : unmanaged, IComponentData
	{
		Entity entity = buffer.CreateEntity();
		buffer.AddComponent(entity, new SendRpcCommandRequest());
		buffer.AddComponent(entity, command);	
	}

	[Unity.Burst.BurstCompile]
	public void OnCreate(ref SystemState state) { Debug.Log("--- Starting a Client ---"); }
	
	[Unity.Burst.BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		EntityCommandBuffer buffer = new(Allocator.Temp);
		foreach((NetworkId _, Entity entity) in SystemAPI.Query<NetworkId>().WithNone<NetworkStreamInGame>().WithEntityAccess())
		{
			buffer.AddComponent<NetworkStreamInGame>(entity);
			SendRpc(buffer, new GoInGameRpcCommand());
		}
		buffer.Playback(state.EntityManager);
		buffer.Dispose();
	}
}
