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

    public event Action<float[,]> onTerrainGenerationFinished;
    public event Action<float[,], int, int> onTerrainLoadingComplete;
    public event Action<RepeatedField<Building>, int> onMapObjectsLoadingComplete;
    public event Action<string, RepeatedField<Character>> onLoginComplete;
    public event Action<string> onErrorShowRequest;
    public event Action<string, string> onInfoMessageShowRequest;
    public event Action<int, string, string, string> onInputDialogShowRequest;
    public event Action<string, string> onLoginRequest;
    public event Action<long> onSelectCharacterRequest;
    public event Action onLoadChatHistoryRequest;
    public event Action<RepeatedField<ChatMessage>> onChatHistoryLoaded;
    public event Action<int, int> onMapLoadRequest;
    public event Action<Vector2, Vector2> onLocalChunksLoadRequest;
    public event Action<RepeatedField<Gardarike.Town>> onCharacterSelected;
    public event Action<ChatMessage> onNewMessageArrived;
    public event Action<string> onChatMessagePublishRequest;
    public event Action<Character> onCharacterUpdateArrived;
    public event Action<bool> onOpenOrCloseLoadingDialog;
    public event Action<int> onPeopleCountIncreased;
    public event Action<int> onSpawnTreesRequest;
    public event Action onMapReady;
    public event Action<GetWorldMapResponse> onWorldMapChunkLoaded;
    public event Action<GetLocalMapResponse> onLocalChunksArrived;
    //public event Action<ResourceUpdatedEvent> onResourceUpdateArrived;
    public event Action onJobMarketLoadingRequest;
    public event Action<string, string, string> onRegistrationRequest;
    public event Action<int, System.Object> onEventFinished;
    public event Action onRegistrationComplete;
    public event Action<Vector2D, String> onNewTownRequest;
    public event Action<String> onNewCharacterRequest;

    public event Action<int, GameObject> onBuildingStarted;
    public event Action<object> onBuildingInitiated;
    public event Action<BuildItemInfo> onBuildingRegistrationEvent;
    public event Action<Building> onBulidingComplete;
    public event Action onBuildingProcessFinished;

    public event Action onResourceUpdateRequest;
    public event Action<Gardarike.Resources> onResourceUpdateArrived;
    public event Action<GameObject> onClickWasMade;
    public event Action onClearMapRequest;
    public event Action<string, object> onGameEvent;
    public event Action onRequestTopEmpires;
    public event Action<GetEmpiresRatingResponse> onTopEmpiresStatisticReceived;
    public event Action<Town> onGoToTownView;
    public event Action onGoToGlobalView;
    public event Action<BuyOption> onBuyOptionChoosen;

    private void Awake()
    {
        instance = this;
    }

    public void BuyOptionChoosen(BuyOption option)
    {
        onBuyOptionChoosen?.Invoke(option);
    }

    public void DispatchGameEvent(string eventAddress, object arguments)
    {
        if (eventAddress.StartsWith("build"))
        {
            var type = eventAddress.Substring(6);
            onBuildingInitiated?.Invoke(type);
        }
    }

    public void ClearMapRequest()
    {
        onClearMapRequest?.Invoke();
    }
    
    public void ClickWasMade(GameObject collider)
    {
        onClickWasMade?.Invoke(collider);
    }

    public void RegisterBuilding(BuildItemInfo building)
    {
        onBuildingRegistrationEvent?.Invoke(building);
    }

    public void BuildingComplete(Building building)
    {
        onBulidingComplete?.Invoke(building);
    }

    public void TerrainGenerationFinished(float[,] heights)
    {
        onTerrainGenerationFinished?.Invoke(heights);
    }

    public void TerrainLoaded(float[,] heights, int x, int y)
    {
        onTerrainLoadingComplete?.Invoke(heights, x, y);
    }

    public void MapObjectsLoaded(RepeatedField<Building> buildings, int treesCount) {
        onMapObjectsLoadingComplete?.Invoke(buildings, treesCount);
    }

    public void LoginComplete(string sessionId, RepeatedField<Character> characters) {
        onLoginComplete?.Invoke(sessionId, characters);
    }

    public void ShowError(string error)
    {
        onErrorShowRequest?.Invoke(error);
    }

    public void ShowInfo(string title, string message)
    {
        onInfoMessageShowRequest?.Invoke(title, message);
    }

    /**
    Return dialog id, you can use complete callback for this dialog using provided id
    */
    public int ShowInputDialog(string title, string bodyMessage, string property)
    {
        // TODO maybe bad generation and not the right place for it
        var randomId = UnityEngine.Random.Range(0, 9999999);

        onInputDialogShowRequest?.Invoke(randomId, title, bodyMessage, property);

        return randomId;
    }

    public void LoginRequest(string username, string password)
    {
        onLoginRequest?.Invoke(username, password);
    }

    public void SelectCharacterRequest(long charId)
     {
        onSelectCharacterRequest?.Invoke(charId);
    }

    public void LoadMap(int x, int y) {
        onMapLoadRequest?.Invoke(x, y);
    }

    public void CharacterSelectionConfirmed(RepeatedField<Gardarike.Town> town) {
        onCharacterSelected?.Invoke(town);
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

    public void SpawnTrees(int count)
    {
        onSpawnTreesRequest?.Invoke(count);
    }

    public void MapIsReady()
    {
        onMapReady?.Invoke();
    }

    // public void UpdateResources(ResourceUpdatedEvent update) {
    //     onResourceUpdateArrived?.Invoke(update);
    // }

    public void RequestJobMarketInfo() {
        onJobMarketLoadingRequest?.Invoke();
    }

    public void SendRegistrationRequest(string user, string password, string email)
    {
        onRegistrationRequest?.Invoke(user, password, email);
    }

    public void NotifyEventFinished(int eventId, System.Object result)
    {
        onEventFinished?.Invoke(eventId, result);
    }
    
    public void NotifyRegistrationComplete()
    {
        onRegistrationComplete?.Invoke();
    }

    public void SendNewTownRequest(Vector2D location, String name)
    {
        onNewTownRequest?.Invoke(location, name);
    }

    public void SendNewCharacterRequest(string name)
    {
        onNewCharacterRequest?.Invoke(name);
    }

    public void WorldMapChunkLoaded(GetWorldMapResponse chunkInfo)
    {
        onWorldMapChunkLoaded?.Invoke(chunkInfo);
    }

    public int ChooseLocationForBuilding(GameObject reference)
    {
        var eventId = UnityEngine.Random.Range(0, 9999999);
        onBuildingStarted?.Invoke(eventId, reference);

        return eventId;
    }

    public void SendResourceUpdateRequest()
    {
        onResourceUpdateRequest?.Invoke();
    }

    public void ResourceUpdateReceived(Gardarike.Resources resources)
    {
        onResourceUpdateArrived?.Invoke(resources);
    }

    public void LocalChunksLoadRequest(Vector2 globalMapChunk, Vector2 localOffsetChunks)
    {
        onLocalChunksLoadRequest?.Invoke(globalMapChunk, localOffsetChunks);
    }

    public void LocalMapChunksLoaded(GetLocalMapResponse localChunksReponse)
    {
        onLocalChunksArrived?.Invoke(localChunksReponse);
    }

    public void RequestTopEmpires()
    {
        onRequestTopEmpires?.Invoke();
    }

    public void TopEmpiresStatisticReceived(GetEmpiresRatingResponse response)
    {
        onTopEmpiresStatisticReceived?.Invoke(response);
    }

    public void FireBuildingFinished()
    {
        onBuildingProcessFinished?.Invoke();
    }

    public void GoToTownView(Town town)
    {
        onGoToTownView?.Invoke(town);
    }

    public void GoToGlobalView()
    {
        onGoToGlobalView?.Invoke();
    }
}
