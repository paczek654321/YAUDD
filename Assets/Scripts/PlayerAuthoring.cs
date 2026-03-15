using Unity.Entities;
using UnityEngine;

public struct Player : IComponentData
{
	public const float MoveSpeed = 5;
	public Vector2 move;
}

class PlayerAuthoring : MonoBehaviour
{
	class Baker : Baker<PlayerAuthoring>
	{
		public override void Bake(PlayerAuthoring authoring)
		{
			Entity entity = GetEntity(TransformUsageFlags.Dynamic);
			AddComponent(entity, new Player{});
		}
	}
}
