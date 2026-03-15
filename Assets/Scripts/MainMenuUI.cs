using System.Threading.Tasks;
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

    public void Host()
	{
		World.DefaultGameObjectInjectionWorld.Dispose();

		World serverWorld = ClientServerBootstrap.CreateServerWorld("ServerWorld");
		World clientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld");

		World.DefaultGameObjectInjectionWorld = serverWorld;

		GetSingletonRW<NetworkStreamDriver>(serverWorld.EntityManager).ValueRW.Listen(NetworkEndpoint.AnyIpv4.WithPort(Port));
		GetSingletonRW<NetworkStreamDriver>(ClientServerBootstrap.ClientWorld.EntityManager).ValueRW.Connect(clientWorld.EntityManager, NetworkEndpoint.LoopbackIpv4.WithPort(Port));

		SceneManager.LoadScene("Main", LoadSceneMode.Single);
		SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);
	}
	public void Join()
	{
		
	}
}
