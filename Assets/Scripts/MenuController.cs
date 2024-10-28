using System;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject ChatObject;

    public GameObject ChatPanel;
    public GameObject ClientObject;
    public Button HostButton;

    public TMP_InputField IpField;
    public Button JoinButton;
    public GameObject MenuPanel;
    public TMP_InputField PortField;
    public GameObject ServerObject;
    public TMP_InputField UserField;

    // Start is called before the first frame update
    private void Start()
    {
        HostButton.onClick.AddListener(HostServer);
        JoinButton.onClick.AddListener(OpenClient);
    }

    private void GetInput()
    {
        if (IpField.text != string.Empty && PortField.text != string.Empty)
        {
            Globals.IpAddressString = IpField.text;
            Globals.IpAddress = IPAddress.Parse(IpField.text);
            Globals.Port = Int32.Parse(PortField.text);
            Globals.Username = UserField.text;
        }
    }

    public void HostServer()
    {
        GetInput();
        ServerObject.SetActive(true);
        _ = MyServer.Instance.CreateServerAsync();
        ChatPanel.SetActive(true);
        MenuPanel.SetActive(false);
        Globals.IsServer = true;
        ChatObject.SetActive(true);
    }

    public void OpenClient()
    {
        GetInput();
        ClientObject.SetActive(true);
        _ = MyClient.Instance.ConnectToServerAsync(Globals.Username);
        ChatPanel.SetActive(true);
        MenuPanel.SetActive(false);
        Globals.IsServer = false;
        ChatObject.SetActive(true);
    }
}
