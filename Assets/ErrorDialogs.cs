using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorDialogs : MonoBehaviour
{
    public Dialog messageDialog;
    public Dialog loadingDialog;
    public InputDialog inputDialog;

    private void Start()
    {
        EventBus.instance.onErrorShowRequest += ShowError;
        EventBus.instance.onOpenOrCloseLoadingDialog += UpdateLoadingDialogState;
        EventBus.instance.onInputDialogShowRequest += ShowInputDialog;
        EventBus.instance.onInfoMessageShowRequest += ShowInfoDialog;
        gameObject.SetActive(false);
    }

    private void ShowInputDialog(string title, string bodyMessage, string property)
    {
        Debug.Log("Input dialog show " + title);

        inputDialog.SetTitle(title);
        inputDialog.SetBody(bodyMessage);
        inputDialog.property = property;

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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DialogFinished()
    {
        messageDialog.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }

    private void UpdateLoadingDialogState(bool visibility)
    {
        loadingDialog.gameObject.SetActive(visibility);
        this.gameObject.SetActive(visibility);
    }
}
