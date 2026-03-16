using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct PlayerMovement : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ( var (transform, player) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<PlayerInput>>() )
		{
			transform.ValueRW.Position.x += player.ValueRO.move.x*PlayerInput.MoveSpeed*SystemAPI.Time.DeltaTime;
			transform.ValueRW.Position.z += player.ValueRO.move.y*PlayerInput.MoveSpeed*SystemAPI.Time.DeltaTime;
		}
    }
}
