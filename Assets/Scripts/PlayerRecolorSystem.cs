using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Rendering;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct PlayerRecolorSystem : ISystem
{
	[Unity.Burst.BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		EntityCommandBuffer buffer = new(Allocator.Temp);
		foreach (var (tag, owner, entity) in SystemAPI.Query<NeedsRecolorTag, GhostOwner>().WithEntityAccess())
		{
			float4 color = ((owner.NetworkId-1)%4) switch
			{
				1 => new float4(0,1,0,1),
				2 => new float4(0,0,1,1),
				3 => new float4(1,0,1,1),
				_ => new float4(1,0,0,1)
			};
			buffer.AddComponent(entity, new URPMaterialPropertyBaseColor{ Value = color });
			buffer.AddComponent(entity, new URPMaterialPropertyEmissionColor{ Value = color*32f });
			buffer.RemoveComponent<NeedsRecolorTag>(entity);
		}
		buffer.Playback(state.EntityManager);
		buffer.Dispose();
	}
}