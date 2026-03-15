using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

partial struct PlayerMovement : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ( var (transform, player) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<Player>>() )
		{
			transform.ValueRW.Position.x += player.ValueRO.move.x*Player.MoveSpeed*Time.deltaTime;
			transform.ValueRW.Position.z += player.ValueRO.move.y*Player.MoveSpeed*Time.deltaTime;
		}
    }
}
