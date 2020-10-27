using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

using Google.Protobuf.Collections;
using Gardarike;

public class ChatComponent : MonoBehaviour
{
    public Text textArea;
    public InputField input;

    // Start is called before the first frame update
    void Start()
    {
        EventBus.instance.onCharacterSelected += LoadChatHistory;
        EventBus.instance.onChatHistoryLoaded += ShowChatHistory;
    }

    private void LoadChatHistory() {
        EventBus.instance.LoadChatHistory();
    }

    private void ShowChatHistory(RepeatedField<ChatMessage> chatMessages) {
        textArea.text = "";

        StringBuilder builder = new StringBuilder();
        foreach (var message in chatMessages) {
            builder.Append(message.Owner);
            builder.Append(" | ");
            builder.Append(message.Text);
            builder.Append("\n");
        }

        textArea.text = builder.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
