using Unity.Entities;
using Unity.NetCode;
using Unity.Mathematics;
using UnityEngine;

public struct PlayerInput : IInputComponentData
{
	public const float MoveSpeed = 5f;
	public float2 Move;
}

class PlayerInputAuthoring : MonoBehaviour
{
	class Baker : Baker<PlayerInputAuthoring>
	{
		public override void Bake(PlayerInputAuthoring authoring)
		{
			Entity entity = GetEntity(TransformUsageFlags.Dynamic);
			AddComponent(entity, new PlayerInput{});
		}
	}
}
