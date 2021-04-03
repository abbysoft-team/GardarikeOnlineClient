using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface InputManager
{
    /**
        Clicked on some worldspace ui. 
        Buildings canvases for example
    */
    bool IsClickedOnSomeWorldspaceUI();

    /**
        Rotation degrees last frame
    */
    float GetRotationDegrees();

    /**
        Disable / Enable rotation
    */
    void ToggleRotationMode(bool rotationEnabled);

    /**
        Focus camera on that gameobject
    */
    void FocusOn(GameObject gameObject);

    /**
        Set this position as start position for camera

        This position will be set when game starts
    */
    void SetCameraStartPosition(Vector3 position);

    /**
        Get camera start position

        usually it is last camera position, because we save camera
        state
    */
    Vector3 GetCameraStartPosition();

    /**
        Get camera position
    */
    Vector3 GetCameraPosition();

    /**
        Set camera position to the initial stored point

        Can access that using #GetCameraStartPosition()
    */
    Vector3 InitCameraPosition();
}
