using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlayerController : MonoBehaviour {

    public static PlayerController instance;

    public bool m_vr = false;

    [Header("Movement")]
    public float m_maxSpeed = 2.0f;
    public float m_accelleration = 5.0f;
    public float m_decelleration = 10.0f;
    public float m_gravity = -10.0f;
    public float m_jumpAccelleration = 100.0f;

    // The average forward direction for these will be used as the forward direction
    public Transform[] m_directionReferences;

    [Header("Interaction")]
    public float m_reach = 5.0f;
    public float m_dropDistance = 3.0f;
    public float m_dropForce = 3.0f;

    [Header("References")]
    Camera m_camera;
    CharacterController m_controller;

    [Header("Variables")]
    PlayerState m_playerState = new PlayerStateNormal();
    Vector3 m_velocity;
    WorldEntity m_heldEntity;

    PlayerEntity m_entity;
    
    void Awake () {
        if(!instance)
        {
            instance = this;
            m_controller = GetComponent<CharacterController>();
            m_entity = GetComponent<PlayerEntity>();
            SetMainCamera();
            m_velocity = new Vector3(0, 0, 0);
        }
        else
        {
            Destroy(gameObject);
        }
	}
	
	void Update () {
        SetMainCamera();
        HandleInput();
    }

    void HandleInput()
    {
        //HandleInteract();
        HandleMovementInput();
    }

    void HandleMovementInput()
    {
        Vector3 forward = GetForwardMovementVector();

        // Get right direction (without y component)
        Vector3 right = GetRightMovementVector();

        // Forward/Backwards
        float vertical = m_vr ? SteamVR_Actions.default_Walk[SteamVR_Input_Sources.LeftHand].axis.y : Input.GetAxisRaw("Vertical");

        if (!Mathf.Approximately(vertical, 0.0f))
        {
            Accellerate(forward * vertical);
        }
        else
        {
            Decellerate(forward);
        }

        // Left/Right
        float horizontal = m_vr ? SteamVR_Actions.default_Walk[SteamVR_Input_Sources.LeftHand].axis.x : Input.GetAxisRaw("Horizontal");

        if (!Mathf.Approximately(horizontal, 0.0f))
        {
            Accellerate(right * horizontal);
        }
        else
        {
            Decellerate(right);
        }

        // Gravity & jumping
        if (!m_controller.isGrounded)
        {
            Accellerate(new Vector3(0.0f, m_gravity, 0.0f));
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            m_velocity.y += m_jumpAccelleration;
        }
        else
        {
            m_velocity.y = Mathf.Max(m_velocity.y, 0.0f);
        }

        // Max speed cap
        // Ignore y velocity
        float yVel = m_velocity.y;
        m_velocity.y = 0;
        if (m_velocity.magnitude > m_maxSpeed)
        {
            m_velocity.Normalize();
            m_velocity *= m_maxSpeed;
        }
        m_velocity.y = yVel;

        m_controller.SimpleMove(m_velocity);
    }

    Vector3 GetForwardMovementVector()
    {
        Vector3 forward = Vector3.zero;
        foreach (Transform t in m_directionReferences)
        {
            forward += t.forward;
        }

        if (forward == Vector3.zero)
        {
            // We've got no references or they have no forward vector - Get the camera forward instead
            forward = m_camera.transform.forward;
        }

        forward.y = 0;
        return forward.normalized;
    }

    Vector3 GetRightMovementVector()
    {
        Vector3 right = Vector3.zero;
        foreach (Transform t in m_directionReferences)
        {
            right += t.right;
        }

        if (right == Vector3.zero)
        {
            // We've got no references or they have no right vector - Get the camera right instead
            right = m_camera.transform.right;
        }

        right.y = 0;
        return right.normalized;
    }

    void Accellerate(Vector3 direction)
    {
        m_velocity += direction * m_accelleration * Time.deltaTime;
    }

    void Decellerate(Vector3 direction)
    {
        // Check if moving in this direction
        float forwardDot = Vector3.Dot(m_velocity, direction);
        float backwardDot = Vector3.Dot(m_velocity, -direction);

        if (forwardDot > 0)
        {
            // Decellerate
            if (forwardDot > 1)
            {
                m_velocity -= direction * m_decelleration * Time.deltaTime;
            }
            else
            {
                m_velocity -= direction * forwardDot;
            }
        }
        else if (backwardDot > 0)
        {
            if (backwardDot > 1)
            {
                m_velocity += direction * m_decelleration * Time.deltaTime;
            }
            else
            {
                m_velocity += direction * backwardDot;
            }
        }

    }

    void SetMainCamera()
    {
        if (!m_camera || !m_camera.CompareTag("MainCamera"))
        {
            m_camera = Camera.main;
        }
    }

    public void ChangeState(PlayerStateName newState)
    {
        if(m_playerState.state != newState)
        {
            // Clean up old state
            m_playerState.OnChangeFrom(this);

            switch(newState)
            {
                case PlayerStateName.NORMAL:
                    m_playerState = new PlayerStateNormal();
                    break;
                case PlayerStateName.HOLDING_ITEM:
                    m_playerState = new PlayerStateHoldingItem();
                    break;
            }

            // Initialise new state
            m_playerState.OnChangeTo(this);
        }
    }

    public bool SetHeldEntity(WorldEntity entity)
    {
        // Can only set held item data if an item isn't currently held
        if(!m_heldEntity)
        {
            m_heldEntity = entity;
            entity.gameObject.SetActive(false);
            SoftwareMouse.SetCursorImage(entity.m_data.sprite);
            PlayerController.instance.ChangeState(PlayerStateName.HOLDING_ITEM);
            return true;
        }

        return false;
    }
}
