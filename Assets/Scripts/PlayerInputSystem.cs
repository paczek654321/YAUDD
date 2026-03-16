using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(GhostInputSystemGroup))]
public partial class PlayerInputSystem : SystemBase
{
	public Controls ControlsInstance {get; private set;}

	protected override void OnCreate()
	{
		ControlsInstance = new Controls();
		ControlsInstance.Enable();
	}

	protected override void OnUpdate()
	{
		foreach(RefRW<PlayerInput> player in SystemAPI.Query<RefRW<PlayerInput>>().WithAll<GhostOwnerIsLocal>())
		{
			player.ValueRW.Move = ControlsInstance.Player.Move.ReadValue<Vector2>();
		}
	}

	protected override void OnDestroy() { ControlsInstance.Disable(); }
}