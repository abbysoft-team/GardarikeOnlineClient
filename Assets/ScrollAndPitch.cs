using UnityEngine;
using UnityEngine.EventSystems;

using System.Collections.Generic;

class ScrollAndPitch : MonoBehaviour
{
#if true
    public GameObject camera;
    private bool debugCameraMovement = false;
    public bool Rotate;
    protected Plane Plane;
    public static ScrollAndPitch instance;
    private float rotationDegrees;

    private float cameraHeight = (GlobalConstants.MAX_CAMERA_Y - GlobalConstants.MIN_CAMERA_Y) / 2;

    private void Awake()
    {
        if (camera == null)
            camera = Camera.main.gameObject;

        instance = this;
    }

    public Vector2 GetCachedCameraPosition()
    {
        return new Vector2(PlayerPrefs.GetFloat(GlobalConstants.CAMERA_X_PROPERTY), PlayerPrefs.GetFloat(GlobalConstants.CAMERA_Z_PROPERTY));
    }

    public Vector3 InitCameraPosition()
    {
        // Set camera at the saved point
		var x = PlayerPrefs.GetFloat(GlobalConstants.CAMERA_X_PROPERTY);
		var z = PlayerPrefs.GetFloat(GlobalConstants.CAMERA_Z_PROPERTY);
		var y = (GlobalConstants.MAX_CAMERA_Y - GlobalConstants.MIN_CAMERA_Y) / 2 + Utility.GetGroundedPoint(new Vector3(x, GlobalConstants.CHUNK_HEIGHT + 200f, z)).y;

		Camera.main.transform.position = new Vector3(x, y, z);

        return Camera.main.transform.position;
    }

    public void SetStartPosition(Vector3 position)
    {
		PlayerPrefs.SetFloat(GlobalConstants.CAMERA_X_PROPERTY, position.x);
		PlayerPrefs.SetFloat(GlobalConstants.CAMERA_Z_PROPERTY, position.z);
    }

    private void Update()
    {
        instance.rotationDegrees = 0;
        Vector3 pos1b = Vector3.zero;
        Vector3 pos1 = Vector3.zero;

        if (instance.debugCameraMovement) {
            camera.transform.position += new Vector3(30, 0, 0);
            TerrainGenerator.instance.CameraMoved(camera.transform.position);
            SetStartPosition(camera.transform.position);
            return;
        }

        // DEBUG 

        // if (Input.GetMouseButton(2))
        // {
        //     instance.rotationDegrees = 10;
        //     Debug.Log("rotated 10 degrees");
        // }

        if (Input.GetMouseButton(0)) {
                SoundManager.instance.PlaySound("click");
                var collider = Utility.GetColliderFromTouch(Input.mousePosition);
                EventBus.instance.ClickWasMade(collider);
        }

        // END DEBUG

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

            instance.rotationDegrees = Vector3.SignedAngle(pos2 - pos1, pos2b - pos1b, Plane.normal);

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
            //Go up or down, depending on terrain height
            var groundHeight = Utility.GetGroundedPoint(new Vector3(camera.transform.position.x, GlobalConstants.CHUNK_HEIGHT + 200, camera.transform.position.z)).y;
            cameraHeight = camera.transform.position.y - groundHeight;

            ApplyCameraBorders(beforeZoom);

            PlayerPrefs.SetFloat("cameraX", camera.transform.position.x);
            PlayerPrefs.SetFloat("cameraZ", camera.transform.position.z);

            return;
        }

        //Scroll
        if (Input.touchCount == 1)
        {
            Delta1 = PlanePositionDelta(Input.GetTouch(0));
            //Delta1 /= Vector3.Distance(pos1, camera.transform.position);
            //Delta1 *= 10;
            if (Input.GetTouch(0).phase == TouchPhase.Moved) {
                var beforeTranslate = camera.transform.position;
                camera.transform.Translate(Delta1, Space.World);
                var groundHeight = Utility.GetGroundedPoint(new Vector3(camera.transform.position.x, GlobalConstants.CHUNK_HEIGHT + 200, camera.transform.position.z)).y;
                camera.transform.position = new Vector3(camera.transform.position.x, groundHeight + cameraHeight, camera.transform.position.z);
            }
            if (Input.GetTouch(0).phase == TouchPhase.Began) {
                SoundManager.instance.PlaySound("click");
                var collider = Utility.GetColliderFromTouch(Input.GetTouch(0).position);
                EventBus.instance.ClickWasMade(collider);
            }
        }

        TerrainGenerator.instance.CameraMoved(camera.transform.position);
    }

    private void ApplyCameraBorders(Vector3 beforeZoom)
    {
        Debug.Log("minY");
        Debug.Log(camera.transform.position);

        if (cameraHeight < GlobalConstants.MIN_CAMERA_Y)
        {
            camera.transform.position = beforeZoom;
        } else if (cameraHeight > GlobalConstants.MAX_CAMERA_Y)
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
        // var rayNow = Camera.main.ScreenPointToRay(screenPos);
        // if (Plane.Raycast(rayNow, out var enterNow))
        //     return rayNow.GetPoint(enterNow);

        return Utility.GetPositionOnTheGround(screenPos);


        //return Vector3.zero;
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

    /*
        Focus maincamera on some gameobject
    */
    public void FocusOn(GameObject toFocus)
    {
        camera.transform.position = new Vector3(toFocus.transform.position.x, toFocus.transform.position.y + GlobalConstants.MIN_CAMERA_Y, toFocus.transform.position.z - GlobalConstants.FOCUS_OBJECT_OFFSET);
    }

    /*
        Start moving camera on x axis on each Update()
    */
    public static void DebugCameraMovement()
    {
        instance.debugCameraMovement = true;
    }

#endif
}