using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Core;

public class ChatBox : MonoBehaviour {

    public int maxMessages = 25; //TODO Make local only(or find way to send to logs)/Max messages in chatbox

    public GameObject chatPanel, textObject;
    public InputField chatBox;
    public string username;

    public Color playerMessage, info, command;

    public DayAndNightControl dayAndNightControl;

    [SerializeField]
    List<Message> messageList = new List<Message>();

	void Start ()
    {
        dayAndNightControl = GameObject.Find("Day and Night Controller").GetComponent<DayAndNightControl>();
    }
	
	void Update ()
    {
        if(chatBox.text == "/time set dawn")
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                SendMessageToChat("Time Set To Dawn.", Message.MessageType.command);
                chatBox.text = "";
                dayAndNightControl.currentTime = 5f;
            }
        }
        else
                if (chatBox.text != "")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessageToChat(username + ": " + chatBox.text, Message.MessageType.playerMessage);
                chatBox.text = "";
            }
        }
            if (!chatBox.isFocused && Input.GetKeyDown(KeyCode.Return))
            {
                chatBox.ActivateInputField();
            }
        if (!chatBox.isFocused && Input.GetKeyDown(KeyCode.Question))
        {
            chatBox.text = "/";
            chatBox.ActivateInputField();
        }

    }
    public void SendMessageToChat(string text, Message.MessageType messageType)
    {
        if(messageList.Count >= maxMessages)
        {
            Destroy(messageList[0].textObject.gameObject);
            messageList.Remove(messageList[0]);
        }

        Message newMessage = new Message();

        newMessage.text = text;

        GameObject newText = Instantiate(textObject, chatPanel.transform);

        newMessage.textObject = newText.GetComponent<Text>();

        newMessage.textObject.text = newMessage.text;
        newMessage.textObject.color = messageTypeColor(messageType);

        messageList.Add(newMessage);
    }

    Color messageTypeColor(Message.MessageType messageType)
    {
        Color color = info;

        switch(messageType)
        {
            case Message.MessageType.command:
                color = command;
                break;
            case Message.MessageType.playerMessage:
                color = playerMessage;
                break;
            case Message.MessageType.info:
                color = info;
                break;
        }

        return color;
    }
}
[System.Serializable]
public class Message
{
    public string text;
    public Text textObject;
    public MessageType messageType;

    public enum MessageType
    {
        playerMessage,
        info,
        command
    }
}