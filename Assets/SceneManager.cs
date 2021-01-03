using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Collections;
using System;

using Gardarike;

public class SceneManager : MonoBehaviour
{

    public GameObject loginScreen;

    // Start is called before the first frame update
    void Awake() 
    {
        EventBus.instance.onLoginComplete += LoginComplete;
    }


    private void LoginComplete(string sessionID, RepeatedField<Character> characters)
    {
        Debug.Log("Login complete, selecting character");

        PlayerPrefs.SetString("sessionId", sessionID);
    
        loginScreen.SetActive(false);
        EventBus.instance.CloseLoadingDialog();

        if (characters.Count == 0) {
            Debug.Log("No characters on the account, show create new empire screen");
            ShowTutorial();
            return;
        }
                
        // already has characters, proceed
        SelectCharacter(characters[0]);
    }
    
    private void SelectCharacter(Character character)
    {
        Debug.Log("Login complete. Welcome, " + character.Name);
        Debug.Log("Selecting character to 0 " + character);

        UpdateCharacterInfo(character);
        //SpawnNewPeople(characters[0]);

        EventBus.instance.SelectCharacterRequest(character);
    }

    private void UpdateCharacterInfo(Character character) 
    {
        Debug.Log("update character: " + character);

        PlayerPrefs.SetInt("userId", (int) character.Id);
        PlayerPrefs.SetString("currentCharName", character.Name);
       // PlayerPrefs.SetInt("Gold", (int) character.Gold);
        PlayerPrefs.SetInt("Population", (int) character.CurrentPopulation);
        PlayerPrefs.SetInt("MaxPopulation", (int) character.MaxPopulation);
    }

    private void ShowTutorial() {
        if (PlayerPrefs.GetInt(GlobalConstants.TUTORIAL_COMPLETE_PROPERTY) == 2) {
            return;
        }

        Debug.Log("Showing tutorial message");
        // TODO extract messages to Strings.cs
        EventBus.instance.ShowInputDialog(Strings.TUTORIAL_TITLE, Strings.TUTORIAL_MESSAGE, GlobalConstants.COUNTRY_NAME_PROPERTY);
        //EventBus.instance.ShowInfo("Info", "New empire " + PlayerPrefs.GetString(GlobalConstants.COUNTRY_NAME_PROPERTY) + " now exist on the world map!");
        var dialogId = EventBus.instance.ShowInputDialog("Capital", "Some of the first men of " + PlayerPrefs.GetString(GlobalConstants.COUNTRY_NAME_PROPERTY) + " found first town. Choose the name for your capital.", GlobalConstants.CAPITAL_NAME_PROPERTY);
        EventBus.instance.onDialogResulted += (id, result) => {
            if (id == dialogId) 
                SendCapitalFoundationRequest();
        };

        //EventBus.instance.SendChatMessage("New empire " + PlayerPrefs.GetString(GlobalConstants.COUNTRY_NAME_PROPERTY) + " now exists");
        //EventBus.instance.SendChatMessage(PlayerPrefs.GetString(GlobalConstants.CAPITAL_NAME_PROPERTY) + " is now the capital of " + GlobalConstants.COUNTRY_NAME_PROPERTY);

        PlayerPrefs.SetInt(GlobalConstants.TUTORIAL_COMPLETE_PROPERTY, 1);
    }

    private void SendCapitalFoundationRequest() {
        Vector2D location = null;

        EventBus.instance.SendNewTownRequest(location, PlayerPrefs.GetString(GlobalConstants.CAPITAL_NAME_PROPERTY));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
