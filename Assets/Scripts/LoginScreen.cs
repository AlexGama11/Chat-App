using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginScreen : MonoBehaviour
{
    public TMP_InputField IpAddress;
    public TMP_InputField Port;
    public Button ServerButton;
    public Button ClientButton;


    public static Action OnServerConnected;

    void Start()
    {
        ServerButton.onClick.AddListener(ServerButtonClicked);
        ClientButton.onClick.AddListener(ClientButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ServerButtonClicked()
    {
        Debug.Log("Server CLicked");
        //TCPServer.Instance.CreateServer(IPAddress.text, int.Parse(Port.text));
    }

    void ClientButtonClicked()
    {
        Debug.Log("Client CLicked");
    }
}
