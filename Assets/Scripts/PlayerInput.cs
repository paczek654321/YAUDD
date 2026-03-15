using Unity.Entities;

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
		foreach(RefRW<Player> player in SystemAPI.Query<RefRW<Player>>())//.WithAll<GhostOwnerIsLocal>())
		{
			player.ValueRW.move = _controls.Player.Move.ReadValue<UnityEngine.Vector2>();
		}
	}

	protected override void OnDestroy() { _controls.Disable(); }
}