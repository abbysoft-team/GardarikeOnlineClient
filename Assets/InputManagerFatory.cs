using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManagerFatory : MonoBehaviour
{
    private static InputManager defaultOne;

    void Start()
    {
        defaultOne = InputManagerDeprecated.instance;
    }

    public static InputManager GetDefaultManager()
    {
        return defaultOne;
    }

}
