using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {
    
    const float MOUSE_DEPTH = 10000.0f;

    // The fraction of the screen in which the mouse moves freely
    //public float m_deadZoneWidth = 0.50f;
    //public float m_deadZoneHeight = 0.50f;
    public float m_deadZoneRadius = 0.2f;
    public float m_lookSpeed = 5.0f;
    public bool m_useFocus = false;
    // The canvas scaler that the deadzone colliders are using
    public CanvasScaler m_canvasScaler;

    // The transform to receive horizontal rotation - this should be your player object
    public Transform m_horizontalRotator;
    // The transform to receive vertical rotation - This should be your camera
    public Transform m_verticalRotator;

    private Camera m_camera;

    // Used to cut off movement when very close to the deadzone
    private bool m_isMoving;
    // The last point in world space that the mouse was over when it moved
    private Vector3 m_focusPoint;
    // The position that the camera thinks the mouse it - Used to make the camera slow gradually even when mousing-over an interface
    private Vector2 m_virtualMousePos;

	// Use this for initialization
	void Start () {
        m_camera = Camera.main;
	}
	
	// Update is called once per frame
	void Update ()
    {
        bool updateFocusPoint = false;

        // If the mouse is over the 3D view, use the actual mouse position for input
        if (UIElementDetect.mouseOverElement == UIElement.GAME_VIEW)
        {
            m_virtualMousePos = SoftwareMouse.GetMousePos();

            if (SoftwareMouse.Moved())
            {
                m_isMoving = true;
                updateFocusPoint = true;
            }
        }

        if (m_isMoving)
        {
            // Get mouse position
            //Vector3 unclampedMousePos = SoftwareMouse.GetUnclampedMousePos();
            Vector3 actualMousePos = m_virtualMousePos;
            //Vector3 mouseCompensation = unclampedMousePos - actualMousePos;
            //mouseCompensation += SoftwareMouse.GetHalfScreenSize();

            if (true)//!MouseInDeadZone(actualMousePos))
            {
                /*
                if (Vector3.Distance(mouseCompensation, SoftwareMouse.GetHalfScreenSize()) > 0)
                {
                    //Debug.Log("Mouse moved beyond screen boundaries! Compensating...");

                    // TODO: There is a bug here that causes the Y axis to behave differently depending on what direction you are facing on the x axis

                    // STEP 1: Move camera so that mouse would be within window (i.e. look towards 'deltaMousePos')
                    // Calculate target direction vector
                    mouseCompensation.z = MOUSE_DEPTH;
                    Vector3 compWorldPos = m_camera.ScreenToWorldPoint(mouseCompensation);
                    Vector3 compDirection = compWorldPos - transform.position;

                    // Find rotation
                    Quaternion rotationDelta = Quaternion.FromToRotation(transform.forward, compDirection);
                    transform.rotation *= rotationDelta;
                    //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0.0f);
                }
                */

                // STEP 2: Smooth eye-look
                // Calculate mouse position in world-space
                if (m_useFocus)
                {
                    // We want to focus on the last point the mousewas over
                    if (updateFocusPoint)
                    {
                        UpdateFocusPoint(actualMousePos);
                    }
                }
                else
                {
                    // We want to focus on whatever the mouse is over now
                    actualMousePos.z = MOUSE_DEPTH;
                    m_focusPoint = m_camera.ScreenToWorldPoint(actualMousePos);
                }

                // Calculate camera-to-mousePos vector
                Vector3 mouseDirection = m_focusPoint - transform.position;

                // Calculate target direction vector
                Vector3 targetScreenPos = GetTargetPositionOnDeadZone2(actualMousePos);
                targetScreenPos.z = MOUSE_DEPTH;
                Vector3 targetWorldPos = m_camera.ScreenToWorldPoint(targetScreenPos);
                Vector3 targetDirection = targetWorldPos - transform.position;

                // Calculate speed to rotate
                float angle = Vector3.Angle(targetDirection, mouseDirection);
                float speed = (angle / 90.0f) * (angle / 90.0f) * m_lookSpeed;

                // Rotate towards
                float step = speed * Time.deltaTime;
                if (step > 0.0001f)
                {
                    Vector3 newDir = Vector3.RotateTowards(targetDirection, mouseDirection, step, 0.0f);

                    // Find new horizonral rotation
                    Quaternion rotationDeltaH = Quaternion.FromToRotation(targetDirection, m_horizontalRotator.forward);
                    m_horizontalRotator.rotation = rotationDeltaH * Quaternion.LookRotation(newDir);
                    m_horizontalRotator.localEulerAngles = new Vector3(0, m_horizontalRotator.localEulerAngles.y, 0.0f);

                    // Find new vertical rotation
                    Quaternion rotationDeltaV = Quaternion.FromToRotation(targetDirection, m_verticalRotator.forward);
                    m_verticalRotator.rotation = rotationDeltaV * Quaternion.LookRotation(newDir);
                    m_verticalRotator.localEulerAngles = new Vector3(m_verticalRotator.localEulerAngles.x, 0, 0.0f);

                    // Calculate new mouse position in screen-space
                    m_virtualMousePos = m_camera.WorldToScreenPoint(m_focusPoint);
                    // If the mouse is over the 3D view, update its position
                    if (UIElementDetect.mouseOverElement == UIElement.GAME_VIEW)
                    {
                        SoftwareMouse.SetMousePos(m_virtualMousePos);
                    }
                }
                else
                {
                    m_isMoving = false;
                }
            }
        }
	}

    /* Rectangular deadzone
    bool MouseInDeadZone(Vector2 mousePos)
    {
        float mouseX = mousePos.x / Screen.width;
        float mouseY = mousePos.y / Screen.height;

        float deadZoneMinX = (1.0f - m_deadZoneWidth) / 2;
        float deadZoneMaxX = 1.0f - deadZoneMinX;

        float deadZoneMinY = (1.0f - m_deadZoneHeight) / 2;
        float deadZoneMaxY = 1.0f - deadZoneMinY;

        return (mouseX >= deadZoneMinX &&
                mouseX <= deadZoneMaxX &&
                mouseY >= deadZoneMinY &&
                mouseY <= deadZoneMaxY);
    }
    */

    void UpdateFocusPoint(Vector3 mousePos)
    {
        RaycastHit hit;
        Ray ray = m_camera.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out hit))
        {
            m_focusPoint = hit.point;
        }
        else
        {
            mousePos.z = MOUSE_DEPTH;
            m_focusPoint = m_camera.ScreenToWorldPoint(mousePos);
        }
    }

    bool MouseInDeadZone(Vector2 mousePos)
    {
        Vector3 mouseWorldPos = SoftwareMouse.ScreenToCanvas(mousePos) * m_canvasScaler.transform.localScale.x;
        List<Collider2D> colliders = TopLevelUI.instance.GetDeadzoneColliders();
        // Find the world-space collision point (since colliders use world-space)
        foreach (Collider2D col in colliders)
        {
            if(col.OverlapPoint(mouseWorldPos))
            {
                return true;
            }
        }
        
        /*
        float mouseX = mousePos.x / Screen.width;
        float mouseY = mousePos.y / Screen.height;

        // Translate points around 0,0
        mouseX -= 0.5f;
        mouseY -= 0.5f;

        return mouseX * mouseX + mouseY * mouseY < m_deadZoneRadius * m_deadZoneRadius;
        */

        return false;
    }

    Vector2 GetTargetPositionOnDeadZone(Vector2 mousePos)
    {
        float mouseX = mousePos.x / Screen.width;
        float mouseY = mousePos.y / Screen.height;

        Vector2 heading = new Vector2(mouseX - 0.5f, mouseY - 0.5f);
        heading.Normalize();

        heading *= m_deadZoneRadius;
        heading.x += 0.5f;
        heading.y += 0.5f;		
        heading.x *= Screen.width;
        heading.y *= Screen.height;

        return heading;     
    }

    // Using colliders
    Vector2 GetTargetPositionOnDeadZone2(Vector2 mousePos)
    {
        List<Collider2D> colliders = TopLevelUI.instance.GetDeadzoneColliders();
        Vector3 closestPoint = Vector2.zero;
        float shortestDist = float.MaxValue;
        Vector3 mouseWorldPos = SoftwareMouse.ScreenToCanvas(mousePos) * m_canvasScaler.transform.localScale.x;

        // Find the world-space collision point (since colliders use world-space)
        foreach (Collider2D col in colliders)
        {
            Vector3 pointOnCol = col.ClosestPoint(mouseWorldPos);
            float dist = (pointOnCol - mouseWorldPos).magnitude;
            if(shortestDist > dist)
            {
                shortestDist = dist;
                closestPoint = pointOnCol;
            }
        }

        // Convert world-space point to canvas space
        // This can't just be WorldToScreenPoint since the position isn't actually on camera, it's on the giant canvas in world-space
        Vector2 canvasSpace = closestPoint / m_canvasScaler.transform.localScale.x;
        Vector2 screenSpace = SoftwareMouse.CanvasToScreen(canvasSpace);
        screenSpace = new Vector2(screenSpace.x, screenSpace.y);
        return screenSpace;
    }
}
