using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegistrationScreen : MonoBehaviour
{
    public InputField emailField;
    public InputField userField;
    public InputField passwordField;
    public Text errorLabel;

    void Awake()
    {
        //EventBus.instance.onRegistrationFailed();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Register()
    {
        if (!ValidateFields())
        {
            return;
        }

        var user = userField.text;
        var password = passwordField.text;
        var email = emailField.text;

        EventBus.instance.SendRegistrationRequest(user, password, email);

        EventBus.instance.ShowInfo("Registration", "Registration complete");
        
        gameObject.SetActive(false);
    }

    private bool ValidateFields()
    {
        if (userField.text.Trim() == "")
        {
            SetError("User field cannot be empty");
            return false;
        }
        if (emailField.text.Trim() == "")
        {
            SetError("Email field cannot be empty");
            return false;
        }
        if (passwordField.text.Trim() == "")
        {
            SetError("Password field cannot be empty");
            return false;
        }


        return true;
    }

    private void SetError(string message)
    {
        errorLabel.text = message;
        errorLabel.gameObject.SetActive(true);
    }
}
