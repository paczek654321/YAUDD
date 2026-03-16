# Yet Another Unity DOTS Demo
# Gameplay
- 4 players multiplayer ✅
- No physics/collisions ✅
- One of the players hosts ✅
- A chat with an UI ✅
- Both private and public chat ✅
# Requirements
- Unity 6000.0.70f1 ✅
- All UI must be TMP ✅
- Compiler - IL2CPP maximal level of stripping and optimization ✅
	- Project Settings → Scripting backend: IL2CPP
	- Project Settings → Additional Compiler Arguments:
		- Prebake Collision Meshes: true
		- Managed Stripping level: High
		- Optimize Mesh Data: true
		- Texture Mipmap Stripping: true
- New input system for movement ✅
- Netcode for entities for cube movement ✅
- RPC for chat ✅
- The cube is a ghost ✅