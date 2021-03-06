using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagementScreen : MonoBehaviour
{
    public ConstructionOptionsView constructionOptionsView; 


    public void Show()
    {
        gameObject.SetActive(true);
        ShowConstructionView();
    }

    public void ShowConstructionView()
    {
        constructionOptionsView.Show();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
