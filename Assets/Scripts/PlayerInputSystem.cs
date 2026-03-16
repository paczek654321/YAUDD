using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(GhostInputSystemGroup))]
public partial class PlayerInputSystem : SystemBase
{
	private Controls _controls;

	protected override void OnCreate()
	{
		_controls = new Controls();
		_controls.Enable();
	}

	protected override void OnUpdate()
	{
		foreach(RefRW<PlayerInput> player in SystemAPI.Query<RefRW<PlayerInput>>().WithAll<GhostOwnerIsLocal>())
		{
			player.ValueRW.Move = _controls.Player.Move.ReadValue<Vector2>();
		}
	}

	protected override void OnDestroy() { _controls.Disable(); }
}