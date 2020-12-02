using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorDialogs : MonoBehaviour
{
    public Dialog errorDialog;
    public Dialog loadingDialog;
    public InputDialog inputDialog;

    private void Start()
    {
        EventBus.instance.onErrorShowRequest += ShowError;
        EventBus.instance.onOpenOrCloseLoadingDialog += UpdateLoadingDialogState;
        EventBus.instance.onInputDialogShowRequest += ShowInputDialog;
        gameObject.SetActive(false);
    }

    private void ShowInputDialog(string title, string bodyMessage, string property)
    {
        Debug.Log("Input dialog show " + title);

        inputDialog.SetTitle(title);
        inputDialog.SetBody(bodyMessage);
        inputDialog.property = property;
    }

    private void ShowError(string message)
    {
        // TODO maybe extract to separate consumer
        Debug.LogError(message);

        errorDialog.SetBody(message);

        errorDialog.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DialogFinished()
    {
        errorDialog.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }

    private void UpdateLoadingDialogState(bool visibility)
    {
        loadingDialog.gameObject.SetActive(visibility);
        this.gameObject.SetActive(visibility);
    }
}
