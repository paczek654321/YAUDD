using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
	const ushort Port = 7979;

	static RefRW<T> GetSingletonRW<T>(EntityManager manager)  where T : unmanaged, IComponentData
	{
		return manager.CreateEntityQuery(typeof(T)).GetSingletonRW<T>();
	}

	void SharedInit()
	{
		GetSingletonRW<NetworkStreamDriver>(ClientServerBootstrap.ClientWorld.EntityManager).ValueRW
			.Connect(ClientServerBootstrap.ClientWorld.EntityManager, NetworkEndpoint.LoopbackIpv4.WithPort(Port));

		SceneManager.LoadScene("Main", LoadSceneMode.Single);
		SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);
	}

    public void Host()
	{
		World.DefaultGameObjectInjectionWorld.Dispose();

		World serverWorld = ClientServerBootstrap.CreateServerWorld("ServerWorld");
		ClientServerBootstrap.CreateClientWorld("ClientWorld");

		World.DefaultGameObjectInjectionWorld = serverWorld;

		GetSingletonRW<NetworkStreamDriver>(serverWorld.EntityManager).ValueRW
			.Listen(NetworkEndpoint.AnyIpv4.WithPort(Port));

		SharedInit();
	}
	public void Join()
	{
		World.DefaultGameObjectInjectionWorld.Dispose();

		World clientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld");
		World.DefaultGameObjectInjectionWorld = clientWorld;

		SharedInit();
	}
}
