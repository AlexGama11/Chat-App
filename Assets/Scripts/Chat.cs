using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    public MessageBubble ChatBubble;
    public Transform ChatContentBox;
    public TMP_InputField ChatInput;
    public string ChatMessage;

    // Define an event for message received
    public delegate void MessageReceivedEventHandler(string username, string message);
    public static event MessageReceivedEventHandler OnMessageReceived;

    // Start is called before the first frame update
    private void Start()
    {
        // Subscribe the DisplayMessage method to the OnMessageReceived event
        OnMessageReceived += DisplayMessage;

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
                await MyServer.Instance.SendDataAsync(Globals.Username + ":" + ChatMessage);
                Debug.Log(Globals.Username + ": " + ChatMessage);
            }
            else
            {
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
    
    private async void Update()
    {
        // Check for incoming messages regularly
        if (Globals.IsServer)
        {
            (string receivedUsername, string receivedMessage) = await MyServer.Instance.ReceiveMessageAsync();
            if (receivedMessage != null)
            {
                // Trigger the event when a message is received
                OnMessageReceived?.Invoke(receivedUsername, receivedMessage);
            }
        }
        else
        {
            (string receivedUsername, string receivedMessage) = await MyClient.Instance.ReceiveMessageAsync();
            if (receivedMessage != null)
            {
                // Trigger the event when a message is received
                OnMessageReceived?.Invoke(receivedUsername, receivedMessage);
            }
        }
    }
}
