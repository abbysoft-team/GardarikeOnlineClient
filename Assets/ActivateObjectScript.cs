using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateObjectScript : MonoBehaviour
{
    public GameObject activateTarget;
    private bool active = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {
        activateTarget.GetComponent<Animator>().SetBool("Activate", !active);
        activateTarget.SetActive(true);
        active = !active;
    }
}
