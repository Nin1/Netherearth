using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIPanel : MonoBehaviour {

    enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    [SerializeField]
    private Direction m_slideInDirection;

    // Docked position
    Vector2 m_dockedPosition;
    // Undocked position
    [SerializeField]
    Vector2 m_undockedPosition;
    // Travel time
    float m_slideDuration = 0.3f;
    // KeyCode
    [SerializeField]
    public KeyCode m_key;

    bool m_visible = false;
    RectTransform m_transform;
    
	void Start () {
        // Calculate the shown/hidden positions for this panel
        m_transform = GetComponent<RectTransform>();
        m_dockedPosition = m_transform.anchoredPosition;
        // TODO: Using a hard-coded position for now, make adaptive later
        if (m_undockedPosition == Vector2.zero)
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            switch (m_slideInDirection)
            {
                case Direction.UP:
                    m_undockedPosition = new Vector2(m_dockedPosition.x, m_dockedPosition.y + m_transform.rect.height);
                    break;
                case Direction.DOWN:
                    m_undockedPosition = new Vector2(m_dockedPosition.x, m_dockedPosition.y - m_transform.rect.height);
                    break;
                case Direction.LEFT:
                    m_undockedPosition = new Vector2(m_dockedPosition.x - m_transform.rect.width, m_dockedPosition.y);
                    break;
                case Direction.RIGHT:
                    m_undockedPosition = new Vector2(m_dockedPosition.x + m_transform.rect.width, m_dockedPosition.y);
                    break;
            }
        }
        SetDeadZonesEnabled(false);
	}
	
    public void ToggleShow()
    {
        if(!m_visible)
        {
            SlideIn();
        }
        else
        {
            SlideOut();
        }
    }

    public void SlideIn()
    {
        m_visible = true;
        SetDeadZonesEnabled(true);
        DOTween.Sequence()
            .Append(m_transform.DOAnchorPos(m_undockedPosition, m_slideDuration)
            .SetEase(Ease.InOutSine));
    }

    public void SlideOut()
    {
        m_visible = false;
        SetDeadZonesEnabled(false);
        DOTween.Sequence()
            .Append(m_transform.DOAnchorPos(m_dockedPosition, m_slideDuration)
            .SetEase(Ease.InOutSine));
    }

    void SetDeadZonesEnabled(bool enabled)
    {
        // Enable any mouselook deadzones
        List<Collider2D> deadzones = new List<Collider2D>();
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>(true);
        foreach (Collider2D col in colliders)
        {
            if (col.gameObject.layer == 9)
            {
                col.gameObject.SetActive(enabled);
            }
        }
    }
}
