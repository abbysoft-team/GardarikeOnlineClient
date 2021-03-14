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

    private void InitScreen()
    {
        EventBus.instance.CloseLoadingDialog();

        // init screen based on data received
        for (int i = 0; i < 7; i++)
        {
            CreateRow(i, "Stub", 77 * i);
        }
    }

    private void CreateRow(int i, string empire, int peopleCount)
    {
        var row = Instantiate(referenceRow);
        var textComponent = row.GetComponent<Text>();
        if (i == 0) {
            textComponent.color = Color.yellow;
        }
        textComponent.text = (i + 1) + ". " + empire + " - " + peopleCount + " people";

        textComponent.transform.parent = container.transform;

        row.SetActive(true);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        //EventBus.instance.OpenLoadingDialog();

        EventBus.instance.RequestTopEmpires();

        InitScreen();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
