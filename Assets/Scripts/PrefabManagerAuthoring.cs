using UnityEngine;
using Unity.Entities;

public class PrefabManagerAuthoring : MonoBehaviour
{
    public GameObject player;

	public class PrefabManagerBaker : Baker<PrefabManagerAuthoring>
	{
		public override void Bake(PrefabManagerAuthoring authoring)
		{
			AddComponent(GetEntity(TransformUsageFlags.Dynamic), new PrefabManager{ Player = GetEntity(authoring.player, TransformUsageFlags.Dynamic) });
		}
	}
}

public struct PrefabManager : IComponentData
{
	public Entity Player;
}
