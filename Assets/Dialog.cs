using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    public Text title;
    public Text body;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTitle(string text)
    {
        title.text = text;
    }

    public void SetBody(string bodyText)
    {
        body.text = bodyText;
    }
}
