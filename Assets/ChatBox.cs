using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Core;
using RPG.Characters;

public class ChatBox : MonoBehaviour {

    public int maxMessages = 25; //TODO Make local only(or find way to send to logs)/Max messages in chatbox

    public GameObject chatPanel, textObject;
    public InputField chatBox;
    public string username;
    Player player;
    Animator playerAnimator;

    public Color playerMessage, emote, info, command, error;

    public DayAndNightControl dayAndNightControl;

    [SerializeField]
    List<Message> messageList = new List<Message>();
     
	void Start ()
    {
        player = FindObjectOfType<Player>();
        dayAndNightControl = GameObject.Find("Day and Night Controller").GetComponent<DayAndNightControl>();
        playerAnimator = player.GetComponent<Animator>();
    }

    void Update()
    {
        if (chatBox.text == "/time set")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessageToChat("Usage", Message.MessageType.info);
                SendMessageToChat("/time set 'time of day'", Message.MessageType.info);
                SendMessageToChat("ex: '/time set sunset'", Message.MessageType.info);
                chatBox.text = "";
            }
        }
        if (chatBox.text == "/time set dawn")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessageToChat("Time Set To Dawn.", Message.MessageType.command);
                chatBox.text = "";
                dayAndNightControl.hour = 4f;
            }
        }
        if (chatBox.text == "/time set sunrise")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessageToChat("Time Set To Sunrise.", Message.MessageType.command);
                chatBox.text = "";
                dayAndNightControl.hour = 6f;
            }
        }
        if (chatBox.text == "/time set morning")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessageToChat("Time Set To Morning.", Message.MessageType.command);
                chatBox.text = "";
                dayAndNightControl.hour = 7f;
            }
        }
        if (chatBox.text == "/time set noon")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessageToChat("Time Set To Noon.", Message.MessageType.command);
                chatBox.text = "";
                dayAndNightControl.hour = 12f;
            }
        }
        if (chatBox.text == "/time set afternoon")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessageToChat("Time Set To Afternoon.", Message.MessageType.command);
                chatBox.text = "";
                dayAndNightControl.hour = 13f;
            }
        }
        if (chatBox.text == "/time set sunset")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessageToChat("Time Set To Sunset.", Message.MessageType.command);
                chatBox.text = "";
                dayAndNightControl.hour = 17f;
            }
        }
        if (chatBox.text == "/time set dusk")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessageToChat("Time Set To Dusk.", Message.MessageType.command);
                chatBox.text = "";
                dayAndNightControl.hour = 18f;
            }
        }
        if (chatBox.text == "/time set night")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessageToChat("Time Set To Night.", Message.MessageType.command);
                chatBox.text = "";
                dayAndNightControl.hour = 20f;
            }
        }
        if (chatBox.text == "/dance")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessageToChat(username + " dances.", Message.MessageType.emote);
                chatBox.text = "";
                playerAnimator.SetTrigger("Dance");
            }
        }
        if(chatBox.text == "/kickball")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessageToChat(username + " kicks a ball.", Message.MessageType.emote);
                chatBox.text = "";
                playerAnimator.SetTrigger("KickBall");
            }
        }
        if (chatBox.text == "/?")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessageToChat("Usable / commands", Message.MessageType.info);
                SendMessageToChat("/dance", Message.MessageType.info);
                SendMessageToChat("/kickball", Message.MessageType.info);
                chatBox.text = "";
            }
        }
        if (chatBox.text == "/")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessageToChat("Use /? for list of Commands", Message.MessageType.info);
                chatBox.text = "";            }
        }
        else
        if (chatBox.text.StartsWith("/"))
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SendMessageToChat("Unknown Command. (" + chatBox.text + ")", Message.MessageType.error);
                chatBox.text = "";
            }
        }
        else if (chatBox.text != "")
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
            chatBox.ActivateInputField();
            chatBox.text = "/";
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
            case Message.MessageType.playerMessage:
                color = playerMessage;
                break;
            case Message.MessageType.emote:
                color = emote;
                break;
            case Message.MessageType.info:
                color = info;
                break;
            case Message.MessageType.command:
                color = command;
                break;
            case Message.MessageType.error:
                color = error;
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
        emote,
        info,
        command,
        error
    }
}