using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Server : MonoBehaviour
{
    private TcpListener server;
    private TcpClient client;

    public int port = 13000;
    public IPAddress serverIp = IPAddress.Parse("127.0.0.1");

    private bool isClientConnected = false;
    private Thread serverThread;

    private NetworkStream stream;
    private byte[] data;


    // Start is called before the first frame update
    void Start()
    {
        try
        {
            server = new TcpListener(serverIp, port);
            server.Start();
            Debug.Log("Server started!");
        }

        catch (Exception e)
        {
            Debug.Log("Error creating the server! Please read: " + e.Message);
        }

        serverThread = new Thread(receiverThread);
        serverThread.Start();
        Debug.Log("Receiver Thread Started!");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void receiverThread()
    {
        while (true)
        {
            if (isClientConnected == false)
            {
                client = server.AcceptTcpClient();
                Debug.Log("Client connected to the server!");
                isClientConnected = true;
                stream = client.GetStream();
            }
            else
            {
                //receive the messages
                data = new byte[256];
                string msg = string.Empty;
                int bytes = stream.Read(data, 0, data.Length);
                msg = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Debug.Log("Server received a message: " + msg);
            }
            Thread.Sleep(100);
        }
    }

    private void OnDisable()
    {
        stream.Close();
        server.Stop();
    }
}
