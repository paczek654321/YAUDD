using UnityEngine;

public class ChatUI : MonoBehaviour
{
    public static ChatUI Instance { get; private set; }
	
	void Start()
	{
		Instance = this;
		Invoke("Hello", 5);
	}

	void Hello()
	{
		ClientChatSystem.SendRpc<ChatRPCCommand>(new ChatRPCCommand{Message = "Hello!"});
	}

	public void DisplayMessage(string message)
	{
		Debug.Log(message);
	}
}