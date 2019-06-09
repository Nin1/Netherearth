using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/** Singleton for accessing elements on the Top Level UI */
public class TopLevelUI : MonoBehaviour
{
    private static TopLevelUI _instance;
    public static TopLevelUI instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(TopLevelUI)) as TopLevelUI;

                if (!_instance)
                {
                    Debug.LogError("There needs to be one active TopLevelUI script on a GameObject in your scene.");
                }
                else
                {
                    _instance.Init();
                }
            }

            return _instance;
        }
    }
    
    void Init()
    {
        Debug.Log("Initialising TLI");
        if (!m_notifyText)
        {
            Debug.Log("Initialising NotifyText");
            m_notifyText = FindObjectOfType(typeof(NotifyText)) as NotifyText;
            if (!m_notifyText) { Debug.LogError("NotifyText not found for TLI"); }
        }
        m_panels = new List<UIPanel>(GetComponentsInChildren<UIPanel>());
    }

    public List<Collider2D> GetDeadzoneColliders()
    {
        List<Collider2D> deadzones = new List<Collider2D>();
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            if (col.gameObject.layer == 9)
            {
                deadzones.Add(col);
            }
        }
        return deadzones;
    }

    public List<RectTransform> GetDeadzones()
    {
        List<RectTransform> deadzones = new List<RectTransform>();
        RectTransform[] transforms = GetComponentsInChildren<RectTransform>();
        foreach (RectTransform t in transforms)
        {
            if(t.gameObject.layer == 9)
            {
                deadzones.Add(t);
            }
        }
        return deadzones;
    }

    [SerializeField]
    NotifyText m_notifyText;
    public static NotifyText notifyText
    {
        get
        {
            return instance.m_notifyText;
        }
    }
    
    List<UIPanel> m_panels = new List<UIPanel>();

    private void Update()
    {
        HandlePanelKeys();
    }

    void HandlePanelKeys()
    {
        foreach (UIPanel panel in m_panels)
        {
            if (Input.GetKeyDown(panel.m_key))
            {
                panel.ToggleShow();
            }
        }
    }
}
