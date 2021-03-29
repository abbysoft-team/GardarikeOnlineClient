using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagementScreen : MonoBehaviour
{
    public ConstructionOptionsView constructionOptionsView; 
    public StatisticsScreen statisticsView;


    public void Show()
    {
        gameObject.SetActive(true);
        ShowConstructionView();
    }

    public void ShowConstructionView()
    {
        constructionOptionsView.Show();
        statisticsView.gameObject.SetActive(false);
    }

    public void ShowStatisticsView()
    {
        statisticsView.Show();
        constructionOptionsView.Close();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
