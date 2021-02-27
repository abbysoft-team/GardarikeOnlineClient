using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Collections;
using System;

using Gardarike;

public class SceneManager : MonoBehaviour
{
    public GameObject loginScreen;
    public TownsManager townsManager;
    public TreeGenerator treeGenerator;

    public static SceneManager instance;

    // Start is called before the first frame update
    void Awake() 
    {   
        instance = this;
        EventBus.instance.onLoginComplete += LoginComplete;
        EventBus.instance.onCharacterSelected += CharacterSelected;
        EventBus.instance.onWorldMapChunkLoaded += ProcessMapChunk;
        EventBus.instance.onLocalChunksArrived += LocalChunksArrived;
    }

    private void LocalChunksArrived(GetLocalMapResponse response)
    {
        // TODO use real data about trees and other stuff
        //EventBus.instance.MapObjectsLoaded(response.Map.Buildings, 10);

        // Transfer fake heights
        EventBus.instance.TerrainLoaded(129, 19, GetHeights());
    }

	private float[,] GetHeights() {
		var array = new float[129, 129];

		for (int i = 0; i < 129; i++)
		{
			for (int j = 0; j < 129; j++)
			{
				array[i, j] = 0.6f + 0.001f * i;
			}
		}

		return array;
	}
    private void CharacterSelected(RepeatedField<Gardarike.Town> towns) {
        Debug.Log("Character selected");

        // We've got towns from getMap request, so this one is obsolete
        //townsManager.InitTowns(towns);

        // Update info about resource count
        EventBus.instance.SendResourceUpdateRequest();

        EventBus.instance.LoadMap(PlayerPrefs.GetString("sessionId"));
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

        EventBus.instance.SelectCharacterRequest(character.Id);
    }

    private void UpdateCharacterInfo(Character character) 
    {
        Debug.Log("update character: " + character);

        PlayerPrefs.SetInt("userId", (int) character.Id);
        PlayerPrefs.SetString("currentCharName", character.Name);
        PlayerPrefs.SetInt("Population", (int) character.CurrentPopulation);
        PlayerPrefs.SetInt("MaxPopulation", (int) character.MaxPopulation);
    }

    private void ShowTutorial() {
        if (PlayerPrefs.GetInt(GlobalConstants.TUTORIAL_COMPLETE_PROPERTY) == 2) {
            return;
        }

        Debug.Log("Showing tutorial message");
        // TODO extract messages to Strings.cs
        var dialogId = EventBus.instance.ShowInputDialog(Strings.TUTORIAL_TITLE, Strings.TUTORIAL_MESSAGE, GlobalConstants.COUNTRY_NAME_PROPERTY);
        dialogId = EventBus.instance.ShowInputDialog("Capital", "Some of the first men of " + PlayerPrefs.GetString(GlobalConstants.COUNTRY_NAME_PROPERTY) + " found first town. Choose the name for your capital.", GlobalConstants.CAPITAL_NAME_PROPERTY);
        EventBus.instance.onEventFinished += (id, result) => {
            if (id == dialogId) 
                SendCapitalFoundationRequest();
        };

        //EventBus.instance.SendChatMessage("New empire " + PlayerPrefs.GetString(GlobalConstants.COUNTRY_NAME_PROPERTY) + " now exists");
        //EventBus.instance.SendChatMessage(PlayerPrefs.GetString(GlobalConstants.CAPITAL_NAME_PROPERTY) + " is now the capital of " + GlobalConstants.COUNTRY_NAME_PROPERTY);

        PlayerPrefs.SetInt(GlobalConstants.TUTORIAL_COMPLETE_PROPERTY, 1);
    }
    private void SendCapitalFoundationRequest()
    {
        Vector2D location = null;
        var character = PlayerPrefs.GetString(GlobalConstants.COUNTRY_NAME_PROPERTY);

        EventBus.instance.SendNewCharacterRequest(character);
        EventBus.instance.SendNewTownRequest(location, PlayerPrefs.GetString(GlobalConstants.CAPITAL_NAME_PROPERTY));
    }

    private void ProcessMapChunk(GetWorldMapResponse chunkInfo)
    {
        Debug.Log("Processing map chunk...");

        Debug.Log("Towns on the chunk: " + chunkInfo.Map.Towns.Count);
        Debug.Log("Trees on chank: " + chunkInfo.Map.Trees);
        Debug.Log("Plants on chank: " + chunkInfo.Map.Plants);
        Debug.Log("Stones on chank: " + chunkInfo.Map.Stones);

        PlayerPrefs.SetString("View", "Global");

        var width = (int) Mathf.Sqrt(chunkInfo.Map.Data.Count);
        var height = width;
        var heights = ProtoConverter.ToHeightsFromProto(chunkInfo.Map.Data, width, height);
        EventBus.instance.TerrainLoaded(width, height, heights);
        //EventBus.instance.MapObjectsLoaded(getMapResponse.Map.Buildings, (int) getMapResponse.Map.TreesCount);

        // world map is just some model, so don't draw all trees on the chunk
        treeGenerator.GenerateTrees((int) (chunkInfo.Map.Trees / 100));
        townsManager.InitTowns(chunkInfo.Map.Towns);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToTownView(Town town) {
       Debug.Log("Open " + town + " town");
    
       PlayerPrefs.SetString("View", "Town");

       EventBus.instance.ClearMapRequest();

       EventBus.instance.LocalChunksLoadRequest(new Vector2(), new Vector2());
    }
}
