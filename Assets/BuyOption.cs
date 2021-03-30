using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyOption : MonoBehaviour
{
    public int woodCost = 0;
    public int stoneCost = 0;
    public int leatherCost = 0;
    public int foodCost = 0;
    public int peopleReq = 0;

    public string optionName = "";

    public Text woodText;
    public Text stoneText;
    public Text leatherText;
    public Text foodText;
    public Text peopleText;
    public Text optionNameText;

    public Image image;
    public Sprite sprite;

    private Button button;

    public string function;

    void Start()
    {
        button = this.GetComponent<Button>();
        button.onClick.AddListener(() => Buy());

        InitText(woodText, GlobalConstants.WOOD, woodCost);
        InitText(stoneText, GlobalConstants.STONE, stoneCost);
        InitText(foodText, GlobalConstants.FOOD, foodCost);
        InitText(leatherText, GlobalConstants.LEATHER, leatherCost);
        InitText(peopleText, GlobalConstants.PEOPLE_COUNT, peopleReq);

        optionNameText.text = optionName;
    }

    private void InitText(Text text, string property, int cost)
    {
        if (text == null) return;

        text.text = PlayerPrefs.GetInt(property) + "/" + cost.ToString();

        if (cost > PlayerPrefs.GetInt(property)) {
            //Disable(text);
        } else {
            text.color = Color.black;
        }
    }

    private void Disable(Text text)
    {
        text.color = Color.red;
        button.enabled = false;
        optionNameText.color = Color.red;
    }

    public void Buy()
    {
        Utility.AddToIntProperty(GlobalConstants.WOOD, woodCost * -1);
        Utility.AddToIntProperty(GlobalConstants.STONE, stoneCost * -1);
        Utility.AddToIntProperty(GlobalConstants.FOOD, foodCost * -1);
        Utility.AddToIntProperty(GlobalConstants.LEATHER, leatherCost * -1);

        EventBus.instance.BuyOptionChoosen(this);
        EventBus.instance.DispatchGameEvent(function, null);
    }

    public void Unbuy()
    {
        Utility.AddToIntProperty(GlobalConstants.WOOD, woodCost * 1);
        Utility.AddToIntProperty(GlobalConstants.STONE, stoneCost * 1);
        Utility.AddToIntProperty(GlobalConstants.FOOD, foodCost * 1);
        Utility.AddToIntProperty(GlobalConstants.LEATHER, leatherCost * 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
