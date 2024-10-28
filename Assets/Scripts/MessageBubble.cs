using TMPro;
using UnityEngine;

public class MessageBubble : MonoBehaviour
{
    public TextMeshProUGUI UserText;
    public TextMeshProUGUI MessageText;

    public void SetUserText(string user)
    {
        UserText.text = user;
    }

    public void SetMessageText(string message)
    {
        MessageText.text = message;
    }
}
