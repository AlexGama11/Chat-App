using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Client : MonoBehaviour
{
    private TcpClient client;
    public string serverIp = "127.0.0.1";
    public int port = 13000;

    private NetworkStream stream;
    void Start()
    {
        Invoke(nameof(connectToServer), 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void connectToServer()
    {
        try
        {
            Debug.Log("Connecting to the server!");
            client = new TcpClient(serverIp,port);
            stream = client.GetStream();
            StartCoroutine(sendMessages());
        }

        catch (Exception e)
        {
            Debug.Log("Error connecting to the server! Message: " + e.Message);
        }

    }

    IEnumerator sendMessages()
    {
        yield return new WaitForSeconds(1f);
        sendMessage("Hello World!");
        yield return new WaitForSeconds(1f);
        sendMessage("Hello World (1)");
    }

    void sendMessage(string msg)
    {
        byte[] data = System.Text.Encoding.ASCII.GetBytes(msg);
        stream.Write(data, 0, data.Length);
    }

    private void OnDisable()
    {
        stream.Close();
        client.Close();
    }
}
