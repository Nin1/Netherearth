using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MouseInteractButton
{
    PRIMARY,
    SECONDARY
}

public enum MouseButtonState
{
    UP,
    DOWN
}

/**
 * World Input 
 * Acts as a wrapper for the PlayerEntity class to pass inputs through to it.
 */
public class WorldInput : MonoBehaviour {

    [SerializeField]
    UIElementDetect m_detect;
    [SerializeField]
    PlayerEntity m_player;

    Camera m_camera;

    // The point in the world where the mouse was clicked down
    Vector3 m_worldSelectedPoint;
    // The point in the world where the mouse currently is
    Vector3 m_worldFocusPoint;
    // True if the position of the mouse in world-space has moved since the mouse was clicked down
    bool m_worldMouseDragged = false;
    bool m_primaryButtonHeld = false;

    private void Awake()
    {
        if (!m_detect)
        {
            m_detect = GetComponent<UIElementDetect>();
        }
        if (!m_player)
        {
            m_player = FindObjectOfType<PlayerEntity>();
        }
        if (!m_camera)
        {
            m_camera = Camera.main;
        }
        m_worldFocusPoint = CalculateWorldFocusPoint();
    }

    // Update is called once per frame
    void Update ()
    {
        if (UIElementDetect.mouseOverElement == UIElement.GAME_VIEW)
        {
            HandleMouseInput();
        }
    }

    void HandleMouseInput()
    {
        UpdateWorldFocusPoint();
        UpdatePlayerEyeDirection();

        if (Input.GetButtonDown("Primary Interact"))
        {
            UpdateWorldSelectedPoint();
            m_primaryButtonHeld = true;
            m_player.SetSelectedEntity(m_player.GetLookingAtEntity());
        }
        else if (Input.GetButtonUp("Primary Interact"))
        {
            m_primaryButtonHeld = false;
            if (m_player.IsHoldingEntity())
            {
                // Try to drop an item if we're holding one
                if(m_player.TryDropHeldEntity())
                {
                    SoftwareMouse.SetCursorToDefault();
                }
            }
            else
            {
                if (m_player.GetLookingAtEntity() == m_player.GetSelectedEntity())
                {
                    // If we are still looking at what we initially clicked on, interact with it
                    m_player.InteractWithWorld(MouseInteractButton.PRIMARY);
                }
            }
        }
        else if (m_worldMouseDragged)
        {
            if (m_player.GetLookingAtEntity() == m_player.GetSelectedEntity())
            {
                // If we are dragging the mouse over what we initially clicked on, try to pick it up
                m_player.TryPickUp();
                m_player.SetSelectedEntity(null);
            }
            else
            {
                // Otherwise we ain't dragging over anything any more!
                m_player.SetSelectedEntity(null);
            }
        }


        if (Input.GetButtonDown("Secondary Interact"))
        {
            m_player.InteractWithWorld(MouseInteractButton.SECONDARY);
        }
    }

    void UpdateWorldSelectedPoint()
    {
        m_worldSelectedPoint = CalculateWorldFocusPoint();
    }

    void UpdateWorldFocusPoint()
    {
        m_worldFocusPoint = CalculateWorldFocusPoint();
        m_worldMouseDragged = m_primaryButtonHeld && (m_worldFocusPoint - m_worldSelectedPoint).sqrMagnitude > 0.1f;
    }

    Vector3 CalculateWorldFocusPoint()
    {
        Vector3 mouseScreenPos = SoftwareMouse.GetMousePos();
        mouseScreenPos.z = 5.0f;    // Changing this also changes the sensitivity needed in UpdateWorldFocusPoint
        return m_camera.ScreenToWorldPoint(mouseScreenPos);
    }

    void UpdatePlayerEyeDirection()
    {
        m_player.SetEyeDirection((m_worldFocusPoint - m_player.GetEyePos()).normalized);
    }
}
