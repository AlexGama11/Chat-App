using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    public MessageBubble ChatBubble;
    public Transform ChatContentBox;
    public TMP_InputField ChatInput;
    public string ChatMessage;

    // Start is called before the first frame update
    private void Start()
    {
        if (Globals.IsServer)
        {
            // Subscribe the DisplayMessage method to the OnMessageReceived event
            MyServer.OnMessageReceived += DisplayMessage;
        }

        else
        {
            // Subscribe the DisplayMessage method to the OnMessageReceived event
            MyClient.OnMessageReceived += DisplayMessage;
        }
        
        // Add an event listener for the "Submit" event (Enter key) on the ChatInput
        ChatInput.onSubmit.AddListener(OnSubmit);
    }

    private void GetChatInput() => ChatMessage = ChatInput.text;

    private async void SendMessage()
    {
        GetChatInput();

        if (!string.IsNullOrEmpty(ChatMessage))
        {
            if (Globals.IsServer)
            {
                //make it wait for the client before being able to send messages
                //DisplayMessage(Globals.Username, ChatMessage);
                await MyServer.Instance.SendDataAsync(Globals.Username + ":" + ChatMessage);
                Debug.Log(Globals.Username + ": " + ChatMessage);
            }
            else
            {
                //DisplayMessage(Globals.Username, ChatMessage);
                await MyClient.Instance.SendDataAsync(Globals.Username + ":" + ChatMessage);
                Debug.Log(Globals.Username + ": " + ChatMessage);
            }

            // Clear the input field after sending the message
            ChatInput.text = string.Empty;
        }
    }

    // Display the message when the event is triggered
    private void DisplayMessage(string username, string message)
    {
        string formattedMessage = $"{username}: {message}";
        Debug.Log(formattedMessage);

        // Use MainThreadDispatcher to instantiate the UI elements in the main thread
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            var newMessageBubble = Instantiate(ChatBubble, ChatContentBox);
            newMessageBubble.GetComponent<MessageBubble>().SetUserText($"{username}:");
            newMessageBubble.GetComponent<MessageBubble>().SetMessageText(message);
        });
    }

    // Invoked when the user presses Enter
    private void OnSubmit(string text)
    {
        SendMessage();
    }
    
}
