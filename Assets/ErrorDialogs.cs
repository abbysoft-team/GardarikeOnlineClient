using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorDialogs : MonoBehaviour
{
    public Dialog errorDialog;

    private void Start()
    {
        EventBus.instance.onErrorShowRequest += ShowError;
        gameObject.SetActive(false);
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
}
