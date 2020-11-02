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
        EventBus.instance.onCharacterUpdateArrived += UpdateCharacterInfo;
    }

    private void LoginComplete(string sessionID, RepeatedField<Character> characters)
    {
        Debug.Log("Login complete. Welcome, " + sessionID);
        Debug.Log("Defaulting character to 0 " + characters[0]);

        PlayerPrefs.SetString("sessionId", sessionID);

        UpdateCharacterInfo(characters[0]);

        EventBus.instance.SelectCharacterRequest(characters[0]);
        EventBus.instance.CloseLoadingDialog();
        
        gameObject.SetActive(false);
        this.sessionID = sessionID;
    }

    private void UpdateCharacterInfo(Character character) 
    {
        PlayerPrefs.SetInt("userId", character.Id);
        PlayerPrefs.SetString("currentCharName", character.Name);
        PlayerPrefs.SetInt("Gold", (int) character.Gold);
        PlayerPrefs.SetInt("Population", (int) character.CurrentPopulation);
        PlayerPrefs.SetInt("MaxPopulation", (int) character.MaxPopulation);
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
        EventBus.instance.OpenLoadingDialog();
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
