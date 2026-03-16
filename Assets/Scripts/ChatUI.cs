using TMPro;
using Unity.NetCode;
using Unity.Collections;
using UnityEngine;
using System.Text;
using System;

public class ChatUI : MonoBehaviour
{
    public static ChatUI Instance { get; private set; }
	
	public GameObject MessageContainer;
	public GameObject MessagePrefab;

	public TMP_InputField InputField;

	private PlayerInputSystem _playerInputSystem;

	void Start()
	{
		Instance = this;
		_playerInputSystem = ClientServerBootstrap.ClientWorld.GetExistingSystemManaged<PlayerInputSystem>();
	}

	public void OnChatSelected()
	{
		_playerInputSystem.ControlsInstance.Player.Disable();
	}
	public void OnChatDeselected()
	{
		_playerInputSystem.ControlsInstance.Player.Enable();
	}

	string TunrcateString(string input, int maxBytes)
	{
		var bytes = Encoding.Unicode.GetBytes(input);
		int charCount = Encoding.Unicode.GetCharCount(bytes, 0, Math.Min(maxBytes, bytes.Length));
		return input[..charCount];
	}

	public void SendChatMessage(string message)
	{
		InputField.text = "";
		ClientChatSystem.SendRpc(new ChatRPCCommand{Message = TunrcateString(message, FixedString512Bytes.UTF8MaxLengthInBytes)});
	}

	public void DisplayMessage(string message)
	{
		var messageInstance = Instantiate(MessagePrefab, MessageContainer.transform);
		messageInstance.GetComponent<TextMeshProUGUI>().text = message;
	}
}