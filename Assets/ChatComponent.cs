using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

using Google.Protobuf.Collections;
using Gardarike;

public class ChatComponent : MonoBehaviour
{
    public static int MAX_MESSAGES_DISPLAYED = 4;

    private Text[] chatRows;
    public InputField input;
    private RepeatedField<ChatMessage> displayedMessages;

    // Start is called before the first frame update
    void Start()
    {
        chatRows = GetComponentsInChildren<Text>();

        // Don't load chat history
        //EventBus.instance.onCharacterSelected += LoadChatHistory;
        EventBus.instance.onChatHistoryLoaded += UpdateChatText;
        EventBus.instance.onNewMessageArrived += ShowNewMessage;

        displayedMessages = new RepeatedField<ChatMessage>();
        displayedMessages.Add(new ChatMessage {Sender = "Gardarike", Text = "Welcome to Bugaga server", Type = ChatMessage.Types.Type.System});

        UpdateChatText(displayedMessages);

        input.onEndEdit.AddListener(delegate {PostMessage();});
    }

    private void PostMessage() {
        Debug.Log("posting: " + input.text);
        if (input.text.Trim() == "") return;

        ChatMessage message = new ChatMessage {
            Text = input.text,
            Sender = PlayerPrefs.GetString("currentCharName") 
        };

        //ShowNewMessage(message);

        EventBus.instance.SendChatMessage(input.text);

        input.text = "";
    }

    private void LoadChatHistory() {
        EventBus.instance.LoadChatHistory();
    }

    private void UpdateChatText(RepeatedField<ChatMessage> chatMessages) {
        for (int i = 0; i < chatRows.Length; i++)
        {
            chatRows[i].text = "";
            chatRows[i].color = Color.black;
        }

        var index = 0;
        foreach (var message in chatMessages) {
            var row = chatRows[index++];
            row.text = message.Sender + "|" + message.Text;
            if (message.Type == ChatMessage.Types.Type.System) row.color = Color.yellow;
        }

        displayedMessages = chatMessages;
    }

    private void ShowNewMessage(ChatMessage newMessage) {
        if (displayedMessages.Count >= MAX_MESSAGES_DISPLAYED) {
            displayedMessages.RemoveAt(0);
        }
        displayedMessages.Add(newMessage);

        UpdateChatText(displayedMessages);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
