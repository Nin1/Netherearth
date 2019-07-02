using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIElement
{
    NONE,
    GAME_VIEW,
    INVENTORY,
}

public class UIElementDetect : MonoBehaviour {

    public static UIElement mouseOverElement = UIElement.NONE;

    [SerializeField]
    UIElement m_thisElement = UIElement.NONE;

    private bool m_isMouseOver = false;

    public void OnMouseEnter()
    {
        //Debug.Log("Mouse over " + m_thisElement);
        mouseOverElement = m_thisElement;
        m_isMouseOver = true;
    }

    public bool IsMouseOver()
    {
        return m_isMouseOver;
    }
}
