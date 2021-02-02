using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameUI : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (ScrollAndPitch.GetRotationDegrees() != 0 || Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began) {
            this.transform.LookAt(Camera.main.transform.position);
            transform.Rotate(0, 180, 0);
        }
    }
}
