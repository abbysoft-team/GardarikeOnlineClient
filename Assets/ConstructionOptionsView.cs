using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionOptionsView : MonoBehaviour
{
    private List<BuyOption> options;

    private BuyOption lastOption;

    void Start()
    {
        EventBus.instance.onBuyOptionChoosen += (option) => lastOption = option;
    }

    public void Show()
    {
        ClearOptions();
        
        var currentView = PlayerPrefs.GetString(GlobalConstants.CURRENT_VIEW_PROPERTY);
        List<BuyOption> optionsInfo;
        if (currentView == GlobalConstants.GLOBAL_VIEW_PROPERTY || currentView == "")
        {
            optionsInfo = GetGlobalOptions();
        }
        else
        {
           optionsInfo = GetTownOptions();
        }

        options = InitGUI(optionsInfo);

        gameObject.SetActive(true);
    }

    private void ClearOptions()
    {
        if (options == null) return;
        
        foreach (BuyOption option in options)
        {
            Destroy(option.gameObject);
        }
    }

    private List<BuyOption> GetGlobalOptions()
    {
        var global = new List<BuyOption>();

        var town = new BuyOption();
        town.optionName = "Town";
        town.function = "build.town";
        town.sprite = ImageManager.GetSprite("town");
        town.peopleReq = 100;
        town.leatherCost = 100;
        town.woodCost = 500;
        town.foodCost = 500;
        town.stoneCost = 900;

        global.Add(town);

        return global;
    }

    private List<BuyOption> GetTownOptions()
    {
        var townOptions = new List<BuyOption>();

        var house = new BuyOption();
        house.optionName = "House";
        house.function = "build.house";
        house.sprite = ImageManager.GetSprite("house");
        house.leatherCost = 1;
        house.woodCost = 7;
        house.stoneCost = 3;

        var storage = new BuyOption();
        storage.optionName = "Storage";
        storage.function = "build.storage";
        storage.sprite = ImageManager.GetSprite("storage");
        storage.leatherCost = 3;
        storage.woodCost = 7;
        storage.stoneCost = 10;

        var quarry = new BuyOption();
        quarry.optionName = "Quarry";
        quarry.function = "build.quarry";
        quarry.sprite = ImageManager.GetSprite("quarry");
        quarry.woodCost = 15;
        quarry.stoneCost = 10;

        townOptions.Add(house);
        townOptions.Add(storage);
        townOptions.Add(quarry);

        return townOptions;
    }

    private List<BuyOption> InitGUI(List<BuyOption> options)
    {
        var referenceOption = GetComponentInChildren<BuyOption>(true);
        var list = new List<BuyOption>();

        foreach (var option in options)
        {
            var optionToAdd = CreateOption(option, referenceOption.gameObject);
            list.Add(optionToAdd);
        }

        return list;
    }

    private BuyOption CreateOption(BuyOption option, GameObject reference)
    {
        var newObject = Instantiate(reference, this.transform, true);

        var optionComponent = newObject.GetComponent<BuyOption>();
        optionComponent.woodCost = option.woodCost;
        optionComponent.foodCost = option.foodCost;
        optionComponent.stoneCost = option.stoneCost;
        optionComponent.peopleReq = option.peopleReq;
        optionComponent.leatherCost = option.leatherCost;
        optionComponent.image.sprite = option.sprite;
        optionComponent.optionName = option.optionName;
        optionComponent.function = option.function;

        newObject.transform.parent = transform;

        newObject.SetActive(true);

        return optionComponent;
    }

    public void UndoneLastBuyOption()
    {
        lastOption.Unbuy();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
