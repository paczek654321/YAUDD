using Unity.NetCode;

[UnityEngine.Scripting.Preserve]
public class AutoConnectBootstrap: ClientServerBootstrap
{
	public override bool Initialize(string defaultWorldName) { return false; }
}