using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputDialog : Dialog
{
    public InputField input;
    public Text errorLabel;
    public string property;

    public DialogManager dialogManager;

    // Start is called before the first frame update
    void Start()
    {
        errorLabel.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetEnteredText() {
        return input.text;
    }

    public bool ValidateInput() {
        if (input.text.Trim() == "") {
            SetErrorLabel("This field cannot be empty");
            return false;
        } else if (input.text.Trim().Length < 5)
        {
            SetErrorLabel("This field cannot be shorter than 5 symbols");
            return false;
        }

        return true;
    }

    private void SetErrorLabel(string error)
    {
        errorLabel.text = error;
        errorLabel.gameObject.SetActive(true);
    }

    public void SubmitDialog()
    {
        if(!ValidateInput())
        {
            return;
        }

        PlayerPrefs.SetString(property, input.text);
        gameObject.SetActive(false);
        dialogManager.DialogFinished();
    }
}
