using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public static PlayerController instance;

    [Header("Movement")]
    public float m_maxSpeed = 2.0f;
    public float m_accelleration = 5.0f;
    public float m_decelleration = 10.0f;
    public float m_gravity = -10.0f;
    public float m_jumpAccelleration = 100.0f;

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

    // Use this for initialization
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
	
	// Update is called once per frame
	void Update () {
        SetMainCamera();
        HandleInput();
    }

    void HandleInput()
    {
        //HandleInteract();
        HandleMovementInput();
    }
    /*
    void HandleInteract()
    {
        if (Input.GetButtonDown("Primary Interact"))
        {
            switch(m_playerState.state)
            {
                case PlayerStateName.NORMAL:
                    InteractWithWorld(true);
                    break;
                case PlayerStateName.HOLDING_ITEM:
                    DropItem();
                    break;
            }
        }
        if (Input.GetButtonDown("Secondary Interact"))
        {
            switch (m_playerState.state)
            {
                case PlayerStateName.NORMAL:
                    InteractWithWorld(false);
                    break;
            }
        }
    }

    void InteractWithWorld(bool primary)
    {
        Vector3 mouseDir = SoftwareMouse.GetMouseWorldDirection(m_camera);
        //int layerMask = LayerMask.GetMask("Player", "UI");
        RaycastHit hit;
        if (Physics.Raycast(m_camera.transform.position, mouseDir, out hit, m_reach))
        {
            WorldEntity subject = hit.transform.GetComponent<WorldEntity>();
            if (subject)
            {
                if (primary)
                {
                    m_entity.PerformPrimaryActionOn(subject);
                }
                else
                {
                    m_entity.PerformSecondaryActionOn(subject);
                }
            }
        }
    }

    void DropItem()
    {
        Vector3 mouseDir = SoftwareMouse.GetMouseWorldDirection(m_camera);
        //int layerMask = LayerMask.GetMask("Player", "UI");
        RaycastHit hit;
        if (Physics.Raycast(m_camera.transform.position, mouseDir, out hit, m_dropDistance))
        {
            // An item can't be dropped here (e.g. wall is too close)
        }
        else
        {
            // Drop the item in front of the player
            Vector3 dropPos = m_camera.transform.position + (mouseDir * m_dropDistance);

            m_heldEntity.gameObject.SetActive(true);
            Transform droppedEntity = m_heldEntity.transform;
            droppedEntity.position = dropPos;

            // @TODO: Hold-down interact and release for a more powerful throw
            Rigidbody rb = droppedEntity.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.AddForce(mouseDir * m_dropForce);

            // Not holding an item anymore!
            m_heldEntity = null;
            ChangeState(PlayerStateName.NORMAL);
        }
    }
    */
    void HandleMovementInput()
    {
        // Get forward direction (without y component)
        Vector3 forward = m_camera.transform.forward;
        forward = new Vector3(forward.x, 0, forward.z);
        forward.Normalize();

        // Get right direction (without y component)
        Vector3 right = m_camera.transform.right;
        right = new Vector3(right.x, 0, right.z);
        right.Normalize();

        // Forwards
        if (Input.GetKey(KeyCode.W))
        {
            Accellerate(forward);
        }
        else
        {
            Decellerate(forward);
        }

        // Backwards
        if (Input.GetKey(KeyCode.S))
        {
            Accellerate(-forward);
        }
        else
        {
            Decellerate(-forward);
        }

        // Right
        if (Input.GetKey(KeyCode.D))
        {
            Accellerate(right);
        }
        else
        {
            Decellerate(right);
        }

        // Left
        if (Input.GetKey(KeyCode.A))
        {
            Accellerate(-right);
        }
        else
        {
            Decellerate(-right);
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


        if (m_velocity.magnitude > m_maxSpeed)
        {
            m_velocity.Normalize();
            m_velocity *= m_maxSpeed;
        }

        m_controller.SimpleMove(m_velocity);
    }

    void Accellerate(Vector3 direction)
    {
        m_velocity += direction * m_accelleration * Time.deltaTime;
    }

    void Decellerate(Vector3 direction)
    {
        // Check if moving in this direction
        float dot = Vector3.Dot(m_velocity, direction);

        if (dot > 0)
        {
            // Decellerate
            if (dot > 1)
            {
                m_velocity -= direction * m_decelleration * Time.deltaTime;
            }
            else
            {
                m_velocity -= direction * dot;
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
