using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gardarike;
using Google.Protobuf.Collections;

public class EventBus : MonoBehaviour
{
    public const string NEW_BUILDING_EVENT = "NEW_BUILDING";
    public static EventBus instance;

    public event Action<BuildItem> onBuildingRegistrationEvent;
    public event Action<BuildItem> onBulidingComplete;
    public event Action<float[,]> onTerrainGenerationFinished;
    public event Action<int, int, float[,]> onTerrainLoadingComplete;
    public event Action<RepeatedField<Building>> onMapObjectsLoadingComplete;
    public event Action<string, RepeatedField<Character>> onLoginComplete;
    public event Action<string> onErrorShowRequest;
    public event Action<string, string> onLoginRequest;
    public event Action<Character> onSelectCharacterRequest;
    public event Action onLoadChatHistoryRequest;
    public event Action<RepeatedField<ChatMessage>> onChatHistoryLoaded;
    public event Action<string> onMapLoadRequest;
    public event Action onCharacterSelected;
    public event Action<ChatMessage> onNewMessageArrived;
    public event Action<string> onChatMessagePublishRequest;
    public event Action<Character> onCharacterUpdateArrived;
    public event Action<bool> onOpenOrCloseLoadingDialog;
    public event Action<int> onPeopleCountIncreased;

    private void Awake()
    {
        instance = this;
    }

    public void RegisterBuilding(BuildItem building)
    {
        onBuildingRegistrationEvent?.Invoke(building);
    }

    public void BuildingComplete(BuildItem building)
    {
        onBulidingComplete?.Invoke(building);
    }

    public void TerrainGenerationFinished(float[,] heights)
    {
        onTerrainGenerationFinished?.Invoke(heights);
    }

    public void TerrainLoaded(int width, int height, float[,] heights)
    {
        onTerrainLoadingComplete?.Invoke(width, height, heights);
    }

    public void MapObjectsLoaded(RepeatedField<Building> buildings) {
        onMapObjectsLoadingComplete?.Invoke(buildings);
    }

    public void LoginComplete(string sessionId, RepeatedField<Character> characters) {
        onLoginComplete?.Invoke(sessionId, characters);
    }

    public void ShowError(string error)
    {
        onErrorShowRequest?.Invoke(error);
    }

    public void LoginRequest(string username, string password)
    {
        onLoginRequest?.Invoke(username, password);
    }

    public void SelectCharacterRequest(Character character)
     {
        onSelectCharacterRequest?.Invoke(character);
    }

    public void LoadMap(string sessionId) {
        onMapLoadRequest?.Invoke(sessionId);
    }

    public void CharacterSelectionConfirmed() {
        onCharacterSelected?.Invoke();
    }

    public void LoadChatHistory() {
        onLoadChatHistoryRequest?.Invoke();
    }

    public void ChatHistoryLoaded(RepeatedField<ChatMessage> chatMessages) {
        onChatHistoryLoaded?.Invoke(chatMessages);
    }

    public void NewMessageArrived(ChatMessage chatMessage) {
        onNewMessageArrived?.Invoke(chatMessage);
    }

    public void SendChatMessage(string text) {
        onChatMessagePublishRequest?.Invoke(text);
    }

    public void CharacterUpdateArrived(Character newState)
    {
        onCharacterUpdateArrived?.Invoke(newState);
    }

    public void OpenLoadingDialog()
    {
        onOpenOrCloseLoadingDialog?.Invoke(true);
    }
    public void CloseLoadingDialog()
    {
        onOpenOrCloseLoadingDialog?.Invoke(false);
    }

    public void PeopleCountIncreased(int newPeople)
    {
        onPeopleCountIncreased?.Invoke(newPeople);
    }
}
