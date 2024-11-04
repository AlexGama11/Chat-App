using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class MyServer : MonoBehaviour
{
    public static MyServer Instance;
    private TcpListener _server;
    private Thread _serverThread;
    private NetworkStream _stream = null;

    // Define an event for message received
    public delegate void MessageReceivedEventHandler(string username, string message);
    public static event MessageReceivedEventHandler OnMessageReceived;

    private void Awake()
    {
        Instance = this;
    }

    public async Task CreateServerAsync()
    {
        Debug.Log(Globals.IpAddressString);
        _server = new TcpListener(Globals.IpAddress, Globals.Port);
        _server.Start();
        Debug.Log("Server created successfully");
        await AcceptClientAsync();
    }

    private async Task AcceptClientAsync()
    {
        while (true)
        {
            TcpClient client = await _server.AcceptTcpClientAsync();
            Debug.Log("Client is connected :: " + client.Client.LocalEndPoint);
            _stream = client.GetStream();
            await StartReceivingAsync();
        }
    }

    private async Task StartReceivingAsync()
    {
        while (true)
        {
            if (_stream != null)
            {
                (string receivedUsername, string receivedMessage) = await ReceiveMessageAsync();

                if (receivedMessage != null)
                {
                    // Process data sent by the client
                    string msgForClient = receivedUsername + ": " + receivedMessage;
                    await SendDataAsync(msgForClient);
                }
            }
        }
    }


    public async Task SendDataAsync(string msg)
    {
        byte[] bytesToClient = System.Text.Encoding.ASCII.GetBytes(msg);
        await _stream.WriteAsync(bytesToClient, 0, bytesToClient.Length);
        Debug.Log("Server Sent :: " + msg);
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
                
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    OnMessageReceived?.Invoke(parts[0], parts[1]);
                });
            }
        }

        return (null, null);
    }
}
