using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gardarike;
using Google.Protobuf.Collections;

public class LoginManager : MonoBehaviour
{
    public InputField usernameField;
    public InputField passwordField;
    public string sessionID;

    public static LoginManager instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        EventBus.instance.onLoginComplete += LoginComplete;
    }

    private void LoginComplete(string sessionID, RepeatedField<Character> characters)
    {
        Debug.Log("Login complete. Welcome, " + sessionID);
        gameObject.SetActive(false);
        this.sessionID = sessionID;
        PlayerPrefs.SetString("sessionId", sessionID);
        PlayerPrefs.SetInt("Gold", (int) characters[0].Gold);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Login()
    {
        if (!ValidateFields())
        {
            return;
        }

        EventBus.instance.LoginRequest(usernameField.text, passwordField.text);
    }

    private bool ValidateFields()
    {
        if (usernameField.text.Trim() == "")
        {
            EventBus.instance.ShowError("Username field empty");
            return false;
        }
        if (passwordField.text.Trim() == "")
        {
            EventBus.instance.ShowError("Password field empty");
            return false;
        }

        return true;
    }
}
