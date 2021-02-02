using UnityEngine;
using UnityEngine.EventSystems;

using System.Collections.Generic;

class ScrollAndPitch : MonoBehaviour
{
#if UNITY_IOS || UNITY_ANDROID
    public GameObject camera;
    public bool Rotate;
    protected Plane Plane;
    public static ScrollAndPitch instance;
    private float rotationDegrees;

    private void Awake()
    {
        if (camera == null)
            camera = Camera.main.gameObject;

        instance = this;
    }

    private void Update()
    {
        rotationDegrees = 0;
        Vector3 pos1b = Vector3.zero;
        Vector3 pos1 = Vector3.zero;

        //Update Plane
        if (Input.touchCount >= 1) {
            Plane.SetNormalAndPosition(transform.up, transform.position);
            pos1b = PlanePosition(Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition);
        }

        var Delta1 = Vector3.zero;
        var Delta2 = Vector3.zero;

        //Pinch
        if (Input.touchCount >= 2)
        {
            pos1 = PlanePosition(Input.GetTouch(0).position);
            var pos2 = PlanePosition(Input.GetTouch(1).position);
            var pos2b = PlanePosition(Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition);

            var midPoint = (pos1b + pos2) / 2;

            rotationDegrees = Vector3.SignedAngle(pos2 - pos1, pos2b - pos1b, Plane.normal);

            if (Rotate && pos2b != pos2) {
                camera.transform.RotateAround(midPoint, Plane.normal, rotationDegrees);
                return;
            }

            //calc zoom
            var zoom = Vector3.Distance(pos1, pos2) /
                       Vector3.Distance(pos1b, pos2b);



            // //edge case
            // if (zoom <= 3 || zoom > 5)
            //    return;

            //Move cam amount the mid ray
            var beforeZoom = camera.transform.position;
            camera.transform.position = Vector3.LerpUnclamped(midPoint, camera.transform.position, 1 / zoom);
            ApplyCameraBorders(beforeZoom);

            return;
        }

        //Scroll
        if (Input.touchCount == 1)
        {
            Delta1 = PlanePositionDelta(Input.GetTouch(0));
            //Delta1 /= Vector3.Distance(pos1, camera.transform.position);
            //Delta1 *= 10;
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
                camera.transform.Translate(Delta1, Space.World);
            if (Input.GetTouch(0).phase == TouchPhase.Began)
                SoundManager.instance.PlaySound("click");
        }

    }

    private void ApplyCameraBorders(Vector3 beforeZoom)
    {
        var minY = Utility.GetGroundedPoint(camera.transform.position).y + GlobalConstants.MIN_CAMERA_Y;

        Debug.Log("minY");
        Debug.Log(camera.transform.position);

        if (camera.transform.position.y < minY)
        {
            camera.transform.position = beforeZoom;
        } else if (camera.transform.position.y > GlobalConstants.MAX_CAMERA_Y)
        {
            camera.transform.position = beforeZoom;
        }

    }

    protected Vector3 PlanePositionDelta(Touch touch)
    {
        //not moved
        if (touch.phase != TouchPhase.Moved)
            return Vector3.zero;

        //delta
        var rayBefore = Camera.main.ScreenPointToRay(touch.position - touch.deltaPosition);
        var rayNow = Camera.main.ScreenPointToRay(touch.position);
        if (Plane.Raycast(rayBefore, out var enterBefore) && Plane.Raycast(rayNow, out var enterNow))
            return rayBefore.GetPoint(enterBefore) - rayNow.GetPoint(enterNow);

        //not on plane
        return Vector3.zero;
    }

    protected Vector3 PlanePosition(Vector2 screenPos)
    {
        //position
        var rayNow = Camera.main.ScreenPointToRay(screenPos);
        if (Plane.Raycast(rayNow, out var enterNow))
            return rayNow.GetPoint(enterNow);

        return Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + transform.up);
    }

    public static bool IsClickedOnSomeWorldspaceUI()
    {
        var pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        return raycastResults.Count > 0 && raycastResults[0].gameObject.layer == LayerMask.NameToLayer("WorldUI");
    }

    public static float GetRotationDegrees()
    {
        return instance.rotationDegrees;
    }

#endif
}