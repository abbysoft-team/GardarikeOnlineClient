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

    public Text textArea;
    public InputField input;
    private RepeatedField<ChatMessage> displayedMessages;

    // Start is called before the first frame update
    void Start()
    {
        // Don't load chat history
        //EventBus.instance.onCharacterSelected += LoadChatHistory;
        EventBus.instance.onChatHistoryLoaded += UpdateChatText;
        EventBus.instance.onNewMessageArrived += ShowNewMessage;

        displayedMessages = new RepeatedField<ChatMessage>();
        displayedMessages.Add(new ChatMessage {Sender = "Gardarike", Text = "Welcome to Bugaga server"});

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
        textArea.text = "";

        StringBuilder builder = new StringBuilder();
        foreach (var message in chatMessages) {
            builder.Append(message.Sender);
            builder.Append(" | ");
            builder.Append(message.Text);
            builder.Append("\n");
        }

        textArea.text = builder.ToString();

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
