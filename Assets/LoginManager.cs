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
    public GameObject registrationScreen;
    public string sessionID;

    public static LoginManager instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //EventBus.instance.onResourceUpdateArrived += UpdateResourceCount;
    }

    // private void UpdateResourceCount(ResourceUpdatedEvent resourceUpdate) {
    //     PlayerPrefs.SetInt("TreesCount", (int) resourceUpdate.TreesCount);
    // }

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

    public void SignUp()
    {
        Debug.Log("Open registration form");
        registrationScreen.SetActive(true);
    }
}
