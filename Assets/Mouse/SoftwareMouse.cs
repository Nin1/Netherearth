using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoftwareMouse : MonoBehaviour {

    private static SoftwareMouse instance;

    /** Mouse position in pixels (0 .. screenSize) */
    private static float m_mousePosX = 0.0f;
    private static float m_mousePosY = 0.0f;

    private static float m_deltaX = 0.0f;
    private static float m_deltaY = 0.0f;

    /** Mouse position in pixels before clamping */
    private static float m_unclampedPosX = 0.0f;
    private static float m_unclampedPosY = 0.0f;

    /** Did the mouse move in the last frame? */
    private static bool m_moved;

    public float m_sensitivity = 2.0f;

    private int m_screenWidth;
    private int m_screenHeight;
    private bool m_inFocus;

    private Image m_cursor;
    public Sprite m_defaultCursor;
    public CanvasScaler m_canvasScaler;
    private static Sprite m_currentCursor;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            m_cursor = GetComponent<Image>();
            m_canvasScaler = GetComponentInParent<CanvasScaler>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

	// Use this for initialization
	void Start () {
        if (Application.isFocused)
        {
            Cursor.lockState = CursorLockMode.Locked;
            m_inFocus = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            m_inFocus = false;
        }
        
        m_mousePosX = Screen.width / 2.0f;
        m_mousePosY = Screen.height / 2.0f;
	}
	
	// Update is called once per frame
	void LateUpdate ()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            m_inFocus = false;
            Cursor.lockState = CursorLockMode.None;
        }
        if(Input.GetMouseButtonDown(0))
        {
            m_inFocus = true;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (m_inFocus)
        {
            m_deltaX = Input.GetAxis("Mouse X");
            m_deltaY = Input.GetAxis("Mouse Y");

            if (m_deltaX != 0.0f || m_deltaY != 0.0f)
            {
                m_unclampedPosX = (m_mousePosX + m_deltaX * m_sensitivity);
                m_unclampedPosY = (m_mousePosY + m_deltaY * m_sensitivity);

                m_mousePosX = Mathf.Clamp(m_unclampedPosX, 0.0f, Screen.width);
                m_mousePosY = Mathf.Clamp(m_unclampedPosY, 0.0f, Screen.height);

                m_moved = true;
            }
            else
            {
                m_moved = false;
            }
        }

        UpdateCursor();
    }

    private void UpdateCursor()
    {
        if (m_currentCursor)
        {
            if (m_cursor.sprite != m_currentCursor)
            {
                m_cursor.sprite = m_currentCursor;
            }
        }
        else
        {
            m_cursor.sprite = m_defaultCursor;
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        m_inFocus = focus;

        if (focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private float GetCanvasScaleFactor()
    {
        float screenAspectRatio = Screen.width / Screen.height;
        float referenceAspectRatio = m_canvasScaler.referenceResolution.x / m_canvasScaler.referenceResolution.y;

        if (m_canvasScaler.screenMatchMode == CanvasScaler.ScreenMatchMode.Expand)
        {
            if (screenAspectRatio > 1.0f)
            {
                return m_canvasScaler.referenceResolution.y / Screen.height;
            }
            else
            {
                return m_canvasScaler.referenceResolution.x / Screen.width;
            }
        }
        else if (m_canvasScaler.screenMatchMode == CanvasScaler.ScreenMatchMode.Shrink)
        {
            if (screenAspectRatio < 1.0f)
            {
                return m_canvasScaler.referenceResolution.y / Screen.height;
            }
            else
            {
                return m_canvasScaler.referenceResolution.x / Screen.width;
            }
        }

        return m_canvasScaler.referenceResolution.x / Screen.width;
    }

    /** PUBLIC STATIC FUNCTIONS */

    public static Vector2 ScreenToCanvas(Vector2 screenPos)
    {
        return screenPos * instance.GetCanvasScaleFactor();
    }

    public static Vector2 CanvasToScreen(Vector2 canvasPos)
    {
        return canvasPos / instance.GetCanvasScaleFactor();
    }

    /** @return The mouse position in screen pixels (bottom-left is at 0,0) */
    public static void GetMousePos(out float x, out float y)
    {
        x = m_mousePosX;
        y = m_mousePosY;
    }

    public static Vector2 GetMousePos()
    {
        return new Vector2(m_mousePosX, m_mousePosY);
    }

    public static Vector2 GetUnclampedMousePos()
    {
        return new Vector2(m_unclampedPosX, m_unclampedPosY);
    }

    /** NOTE: Calling this resets the unclamped and clamped mouse values so should be used with caution! */
    public static void SetMousePos(Vector2 newMousePos)
    {
        m_unclampedPosX = newMousePos.x;
        m_unclampedPosY = newMousePos.y;
        m_mousePosX = Mathf.Clamp(m_unclampedPosX, 0.0f, Screen.width);
        m_mousePosY = Mathf.Clamp(m_unclampedPosY, 0.0f, Screen.height);

    }

    public static float GetDeltaX()
    {
        return m_deltaX;
    }

    public static float GetDeltaY()
    {
        return m_deltaY;
    }

    /** @return Did the mouse move in the last frame? */
    public static bool Moved()
    {
        return m_moved;
    }

    public static Vector3 GetMouseWorldDirection(Camera camera)
    {
        Vector3 mousePos = GetMousePos();
        mousePos.z = 10.0f;

        return (camera.ScreenToWorldPoint(mousePos) - camera.transform.position).normalized;
    }

    public static void SetCursorImage(Sprite newCursor)
    {
        m_currentCursor = newCursor;
    }

    public static void SetCursorToDefault()
    {
        m_currentCursor = null;
    }

    public static Vector3 GetScreenSize()
    {
        return new Vector2(Screen.width, Screen.height);
    }

    public static Vector3 GetHalfScreenSize()
    {
        return new Vector2(Screen.width, Screen.height) / 2;
    }
}
