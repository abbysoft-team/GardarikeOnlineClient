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

    public Text woodText;
    public Text stoneText;
    public Text leatherText;
    public Text foodText;
    public Text peopleText;

    public Image image;
    public Sprite sprite;

    private Button button;

    void Start()
    {
        button = this.GetComponent<Button>();

        InitText(woodText, GlobalConstants.WOOD, woodCost);
        InitText(stoneText, GlobalConstants.STONE, stoneCost);
        InitText(foodText, GlobalConstants.FOOD, foodCost);
        InitText(leatherText, GlobalConstants.LEATHER, leatherCost);
        InitText(peopleText, GlobalConstants.PEOPLE_COUNT, peopleReq);
    }

    private void InitText(Text text, string property, int cost)
    {
        if (text == null) return;

        text.text = PlayerPrefs.GetInt(property) + "/" + cost.ToString();

        if (cost > PlayerPrefs.GetInt(property)) {
            text.color = Color.red;
            button.enabled = false;
        } else {
            text.color = Color.black;
            button.enabled = true;
        }
    }

    public void Buy()
    {
        Utility.AddToIntProperty(GlobalConstants.WOOD, woodCost * -1);
        Utility.AddToIntProperty(GlobalConstants.STONE, stoneCost * -1);
        Utility.AddToIntProperty(GlobalConstants.FOOD, foodCost * -1);
        Utility.AddToIntProperty(GlobalConstants.LEATHER, leatherCost * -1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
