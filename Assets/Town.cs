using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Town : MonoBehaviour
{
    public Text population;
    public InputField name;
    public Text empireName;
    public Text selectedUIName;
    public GameObject headerUI;
    public GameObject selectedUI;

    public Gardarike.Town config;

    // Start is called before the first frame update
    void Start()
    {
        var goToTownButton = selectedUI.GetComponentInChildren<Button>();
        goToTownButton.onClick.AddListener(() => EventBus.instance.GoToTownView(this));

        name.onEndEdit.AddListener((value) => selectedUIName.text = value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WasClicked()
    {
        selectedUI.SetActive(true);
        headerUI.SetActive(false);
    }

    public void ResetSelection()
    {
        selectedUI.SetActive(false);
        headerUI.SetActive(true);
    }

    public void Init(Gardarike.Town configuration)
    {
        config = configuration;
        empireName.text = configuration.OwnerName;
        name.text = configuration.Name;
        selectedUIName.text = configuration.Name;
        population.text = "" + configuration.Population;

        headerUI.SetActive(true);
    }
}
