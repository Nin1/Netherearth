using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateHoldingItem : PlayerState
{
    public override PlayerStateName state { get { return PlayerStateName.HOLDING_ITEM; } }

    const float HOLDING_CAMERA_DEADZONE_RADIUS = 0.2f;

    CameraController m_cameraController;
    float m_originalRadius;

    // Called immediately when changing to this state
    public override void OnChangeTo(PlayerController pc)
    {
        // Increase deadzone radius when holding an item
        m_cameraController = pc.GetComponent<CameraController>();
        if (m_cameraController)
        {
            m_originalRadius = m_cameraController.m_deadZoneRadius;
            m_cameraController.m_deadZoneRadius = HOLDING_CAMERA_DEADZONE_RADIUS;
        }
    }

    // Called immediately when changing from this state
    public override void OnChangeFrom(PlayerController pc)
    {
        if (m_cameraController)
        {
            m_cameraController.m_deadZoneRadius = m_originalRadius;
        }
        SoftwareMouse.SetCursorToDefault();
    }
}
