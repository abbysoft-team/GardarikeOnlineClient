using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public Dialog messageDialog;
    public Dialog loadingDialog;
    public InputDialog inputDialog;
    public Queue<DialogInfo> dialogQueue = new Queue<DialogInfo>();
    public DialogInfo current;

    private void Start()
    {
        EventBus.instance.onErrorShowRequest += ShowError;
        EventBus.instance.onOpenOrCloseLoadingDialog += UpdateLoadingDialogState;
        EventBus.instance.onInputDialogShowRequest += (id, title, message, property) => AddToQueue(id, title, message, property, DialogType.INPUT);
        EventBus.instance.onInfoMessageShowRequest += (title, message) => AddToQueue(-1, title, message, null, DialogType.INFO);
        gameObject.SetActive(false);
    }

    public void Update()
    {
        if (NetworkManagerImpl.networkError)
        {
            UpdateLoadingDialogState(false);
            ShowError("Could not connect to server");   
            NetworkManagerImpl.networkError = false;
        }
    }

    private void AddToQueue(int id, string title, string message, string inputProperty, DialogType type)
    {
        var info = new DialogInfo(id, title, message, inputProperty, type);
        dialogQueue.Enqueue(info);

        if (current == null)
        {
            ShowNext();
        }
    }

    public void ShowNext()
    {
        if (dialogQueue.Count == 0)
        {
            current = null;
            return;
        }

        current = dialogQueue.Dequeue();

        if (current.type == DialogType.INPUT)
        {
            ShowInputDialog(current);
        }
        else if (current.type == DialogType.INFO)
        {
            ShowInfoDialog(current.title, current.message);
        }
    }

    private void ShowInputDialog(DialogInfo info)
    {
        Debug.Log("Input dialog show " + info.title);

        inputDialog.input.text = "";
        inputDialog.SetTitle(info.title);
        inputDialog.SetBody(info.message);
        inputDialog.property = info.inputProperty;

        inputDialog.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    private void ShowError(string message)
    {
        // TODO maybe extract to separate consumer
        Debug.LogError(message);

        messageDialog.SetBody(message);
        messageDialog.SetTitle("Warning");

        messageDialog.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    private void ShowInfoDialog(string title, string message) {
        Debug.Log("Showing info dialog");
        messageDialog.SetTitle("Info");
        messageDialog.SetBody(message);
    
        messageDialog.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    public void DialogFinished()
    {
        messageDialog.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
        
        // error dialog are not in the queue
        if (current == null) return;

        EventBus.instance.NotifyEventFinished(current.id, DialogResult.SUCCESS);

        ShowNext();
    }

    private void UpdateLoadingDialogState(bool visibility)
    {
        loadingDialog.gameObject.SetActive(visibility);
        this.gameObject.SetActive(visibility);
    }
}
