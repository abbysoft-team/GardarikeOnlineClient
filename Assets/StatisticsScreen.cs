using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsScreen : MonoBehaviour
{
    public GameObject referenceRow;
    public GameObject container;

    void Start()
    {
        EventBus.instance.onTopEmpiresStatisticReceived += InitScreen;
    }

    private void InitScreen(Gardarike.GetEmpiresRatingResponse response)
    {
        EventBus.instance.CloseLoadingDialog();

        var otherEmpiresToShow = response.PlayerRating == null ? 7 : 6;

        // init screen based on data received
        for (int i = 0; i < otherEmpiresToShow; i++)
        {
            CreateRow((ulong) i, response.Entries[i].EmpireName, response.Entries[i].Value, i == 0 ? Color.yellow : Color.black);
        }

        if (otherEmpiresToShow == 6) {
            CreateRow(response.PlayerRating.Position, response.PlayerRating.EmpireName, response.PlayerRating.Value, Color.red);
        }
    }

    private void CreateRow(ulong i, string empire, ulong peopleCount, Color color)
    {
        var row = Instantiate(referenceRow);
        var textComponent = row.GetComponent<Text>();
        textComponent.color = color;
        textComponent.text = (i + 1) + ". " + empire + " - " + peopleCount + " people";

        textComponent.transform.parent = container.transform;

        row.SetActive(true);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        EventBus.instance.OpenLoadingDialog();

        EventBus.instance.RequestTopEmpires();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
