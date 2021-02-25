using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionOptionsView : MonoBehaviour
{
    private List<BuyOption> options;

    void Awake()
    {
        var currentView = PlayerPrefs.GetString("View");
        if (currentView == "Global")
        {
            options = GetGlobalOptions();
        }
        else
        {
           options = GetTownOptions();
        }

        InitGUI(options);
    }

    private List<BuyOption> GetGlobalOptions()
    {
        var global = new List<BuyOption>();

        var town = new BuyOption();
        town.optionName = "Town";
        town.sprite = ImageManager.GetSprite("town");
        town.peopleReq = 100;
        town.leatherCost = 100;
        town.woodCost = 100;
        town.foodCost = 100;
        town.stoneCost = 100;

        global.Add(town);

        return global;
    }

    private List<BuyOption> GetTownOptions()
    {
        var townOptions = new List<BuyOption>();

        var house = new BuyOption();
        house.optionName = "House";
        house.sprite = ImageManager.GetSprite("house");
        house.leatherCost = 1;
        house.woodCost = 7;
        house.stoneCost = 3;

        var storage = new BuyOption();
        storage.optionName = "Storage";
        storage.sprite = ImageManager.GetSprite("storage");
        storage.leatherCost = 3;
        storage.woodCost = 7;
        storage.stoneCost = 10;

        var quarry = new BuyOption();
        quarry.optionName = "Quarry";
        quarry.sprite = ImageManager.GetSprite("quarry");
        quarry.woodCost = 15;
        quarry.stoneCost = 10;

        townOptions.Add(house);
        townOptions.Add(storage);
        townOptions.Add(quarry);

        return townOptions;
    }

    private void InitGUI(List<BuyOption> options)
    {
        var referenceOption = GetComponentInChildren<BuyOption>(true);

        foreach (var option in options)
        {
            CreateOption(option, referenceOption.gameObject);
        }
    }

    private void CreateOption(BuyOption option, GameObject reference)
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

        newObject.transform.parent = transform;

        newObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
