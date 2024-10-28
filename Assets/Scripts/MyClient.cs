using UnityEngine;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

public class MyClient : MonoBehaviour
{
    public static MyClient Instance;
    private TcpClient _client;
    private NetworkStream _stream;
    private Thread _receiverThread;

    private void Awake()
    {
        Instance = this;
    }

    public async Task ConnectToServerAsync(string username)
    {
        _client = new TcpClient();
        await _client.ConnectAsync(Globals.IpAddressString, Globals.Port);
        _stream = _client.GetStream();
        Debug.Log("Connected to server");
        await SendDataAsync("JOIN:" + username);

        // Start a separate thread for receiving messages
        _receiverThread = new Thread(ReceiveMessages);
        _receiverThread.Start();
    }

    public async Task SendDataAsync(string msg)
    {
        byte[] bytes = System.Text.Encoding.ASCII.GetBytes(msg);
        await _stream.WriteAsync(bytes, 0, bytes.Length);
        Debug.Log("Client Sent :: " + msg);
    }

    public async Task<(string, string)> ReceiveMessageAsync()
    {
        byte[] bytes = new byte[256];
        int bytesRead = await _stream.ReadAsync(bytes, 0, bytes.Length);

        if (bytesRead > 0)
        {
            string receivedData = System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRead);
            string[] parts = receivedData.Split(':');

            if (parts.Length == 2)
            {
                return (parts[0], parts[1]);
            }
        }

        return (null, null);
    }

    private async void ReceiveMessages()
    {
        while (true)
        {
            (string receivedUsername, string receivedMsg) = await ReceiveMessageAsync();

            if (receivedMsg != null)
            {
                Debug.Log("Message received by Client: " + receivedUsername + ": " + receivedMsg);
                // Handle the received message (possibly update UI using MainThreadDispatcher)
            }
        
            // Introduce a delay to avoid busy-waiting and give other tasks a chance to run
            await Task.Delay(100);
        }
    }

}
