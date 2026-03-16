using Unity.Entities;
using UnityEngine;

public struct NeedsRecolorTag : IComponentData {}

class PlayerRecolorAuthoring : MonoBehaviour
{
	class Baker : Baker<PlayerRecolorAuthoring>
	{
		public override void Bake(PlayerRecolorAuthoring authoring)
		{
			Entity entity = GetEntity(TransformUsageFlags.Renderable);
			AddComponent(entity, new NeedsRecolorTag{});
		}
	}
}
