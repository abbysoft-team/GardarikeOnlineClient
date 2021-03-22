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
        EventBus.instance.onGoToTownView += GoToTownView;
        EventBus.instance.onTownPlacedResponse += OnTownPlaced;
    }

    private void OnTownPlaced(PlaceTownResponse response)
    {
        if (PlayerPrefs.GetInt(GlobalConstants.TUTORIAL_COMPLETE_PROPERTY) != 3) {
            return;
        }

        Debug.Log("First town placed, end tutorial");

        // first town placed. Register it and
        // set camera position to it (at least x and z coords, y coord should be set when terrain is ready)

        var newTown = new Gardarike.Town();
        newTown.X = (long) response.Location.X;
        newTown.Y = (long) response.Location.Y;
        newTown.Population = 0;
        newTown.OwnerName = PlayerPrefs.GetString(GlobalConstants.COUNTRY_NAME_PROPERTY);
        newTown.Name = PlayerPrefs.GetString(GlobalConstants.CAPITAL_NAME_PROPERTY);

        var townObject = TownsManager.instance.RegisterTown(newTown);

        ScrollAndPitch.instance.FocusOn(townObject);

        PlayerPrefs.SetInt(GlobalConstants.TUTORIAL_COMPLETE_PROPERTY, 4);
    }

    private void LocalChunksArrived(GetLocalMapResponse response)
    {
        // TODO use real data about trees and other stuff
        //EventBus.instance.MapObjectsLoaded(response.Map.Buildings, 10);

        // Transfer fake heights
        EventBus.instance.TerrainLoaded(GetHeights(), 0, 0);
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

        Debug.Log("towns: " + towns);

        if (towns.Count == 0)
        {
            Debug.Log("No towns, founding new town");
            PlayerPrefs.SetInt(GlobalConstants.TUTORIAL_COMPLETE_PROPERTY, 3);
            // build first town
            EventBus.instance.SendNewTownRequest(null, PlayerPrefs.GetString(GlobalConstants.CAPITAL_NAME_PROPERTY));
        }

        // We've got towns from getMap request, so this one is obsolete
        //townsManager.InitTowns(towns);

        // Update info about resource count
        EventBus.instance.SendResourceUpdateRequest();

        TerrainGenerator.instance.LoadMap(0, 0);
    }

    private void LoginComplete(string sessionID, RepeatedField<Character> characters)
    {
        Debug.Log("Login complete, selecting character");

        PlayerPrefs.SetString("sessionId", sessionID);
    
        loginScreen.SetActive(false);
        EventBus.instance.CloseLoadingDialog();

        if (characters.Count == 0) {
            Debug.Log("No characters on the account, show create new empire screen");
            PlayerPrefs.SetFloat("cameraX", GlobalConstants.CHUNK_SIZE / 2);
            PlayerPrefs.SetFloat("cameraZ", GlobalConstants.CHUNK_SIZE / 2);
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

        PlayerPrefs.SetString(GlobalConstants.CURRENT_VIEW_PROPERTY, GlobalConstants.GLOBAL_VIEW_PROPERTY);

        UpdateCharacterInfo(character);
        //SpawnNewPeople(characters[0]);

        EventBus.instance.SelectCharacterRequest(character.Id);
    }

    private void UpdateCharacterInfo(Character character) 
    {
        Debug.Log("update character: " + character);

        PlayerPrefs.SetInt("userId", (int) character.Id);
        PlayerPrefs.SetString(GlobalConstants.COUNTRY_NAME_PROPERTY, character.Name);
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
        EventBus.instance.onEventFinished += (id, result) => {
            if (id != dialogId) return;
            var secondId = EventBus.instance.ShowInputDialog("Capital", "Some of the first men of " + PlayerPrefs.GetString(GlobalConstants.COUNTRY_NAME_PROPERTY) + " found first town. Choose the name for your capital.", GlobalConstants.CAPITAL_NAME_PROPERTY);
            EventBus.instance.onEventFinished += (id, result) => {
                if (id == secondId) 
                    SendCapitalFoundationRequest();
            };
        };

        //EventBus.instance.SendChatMessage("New empire " + PlayerPrefs.GetString(GlobalConstants.COUNTRY_NAME_PROPERTY) + " now exists");
        //EventBus.instance.SendChatMessage(PlayerPrefs.GetString(GlobalConstants.CAPITAL_NAME_PROPERTY) + " is now the capital of " + GlobalConstants.COUNTRY_NAME_PROPERTY);

        PlayerPrefs.SetInt(GlobalConstants.TUTORIAL_COMPLETE_PROPERTY, 1);
    }
    private void SendCapitalFoundationRequest()
    {
        var character = PlayerPrefs.GetString(GlobalConstants.COUNTRY_NAME_PROPERTY);

        EventBus.instance.SendNewCharacterRequest(character);
        //EventBus.instance.SelectCharacterRequest(0);
    }

    private void ProcessMapChunk(GetWorldMapResponse chunkInfo)
    {
        Debug.Log("Processing map chunk...");

        Debug.Log("Towns on the chunk: " + chunkInfo.Map.Towns.Count);
        Debug.Log("Trees on chank: " + chunkInfo.Map.Trees);
        Debug.Log("Plants on chank: " + chunkInfo.Map.Plants);
        Debug.Log("Stones on chank: " + chunkInfo.Map.Stones);

        PlayerPrefs.SetString("View", "Global");

        MapCache.StoreWorldChunk(chunkInfo);

        var heights = ProtoConverter.ToHeightsFromProto(chunkInfo.Map.Data);
        EventBus.instance.TerrainLoaded(heights, chunkInfo.Map.X, chunkInfo.Map.Y);
        //EventBus.instance.MapObjectsLoaded(getMapResponse.Map.Buildings, (int) getMapResponse.Map.TreesCount);

        // world map is just some model, so don't draw all trees on the chunk
        var treeCount = (int) chunkInfo.Map.Trees;
        treeCount = Math.Min(200, treeCount);
        treeGenerator.GenerateTrees(treeCount);
        townsManager.InitTowns(chunkInfo.Map.Towns);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToTownView(Town town) {
       Debug.Log("Open " + town + " town");
    
       PlayerPrefs.SetString(GlobalConstants.CURRENT_VIEW_PROPERTY, GlobalConstants.TOWN_VIEW_PROPERTY);
       PlayerPrefs.SetString(GlobalConstants.CURRENT_TOWN_PROPERTY, town.name.text);

       EventBus.instance.ClearMapRequest();
       EventBus.instance.LocalChunksLoadRequest(new Vector2(), new Vector2());
    }

    public void GoToGlobalView()
    {
        Debug.Log("Go to global view");

        EventBus.instance.GoToGlobalView();

        PlayerPrefs.SetString(GlobalConstants.CURRENT_VIEW_PROPERTY, GlobalConstants.GLOBAL_VIEW_PROPERTY);
        TownsManager.instance.RestoreTowns();
        //TerrainGenerator.instance.LoadMap();
    }
}
