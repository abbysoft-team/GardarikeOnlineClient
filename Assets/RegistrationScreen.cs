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
        EventBus.instance.onRegistrationComplete += CompleteRegistration;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CompleteRegistration()
    {
        EventBus.instance.CloseLoadingDialog();
        gameObject.SetActive(false);
        EventBus.instance.ShowInfo("Registration", "Registration complete");
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

        EventBus.instance.onErrorShowRequest += SetError;

        EventBus.instance.SendRegistrationRequest(user, password, email);
        EventBus.instance.OpenLoadingDialog();
    }

    private bool ValidateFields()
    {
        if (userField.text.Trim() == "")
        {
            SetError("Username field cannot be empty");
            return false;
        }
        if (emailField.text.Trim() == "")
        {
            SetError("Email field cannot be empty");
            return false;
        }
        if (emailField.text.Trim().Length < 4 || !emailField.text.Contains("@"))
        {
            SetError("Email is not valid");
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
