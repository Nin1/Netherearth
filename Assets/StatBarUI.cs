using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Displays one of a given entity's stat */
public class StatBarUI : MonoBehaviour
{
    [SerializeField]
    RectTransform m_background;
    [SerializeField]
    RectTransform m_foreground;
    [SerializeField]
    WorldEntity m_entity;
    [SerializeField]
    EntityStatType m_statType = EntityStatType.NONE;

    private void Start()
    {
        // Assume we are one of the player's stat bars if we don't have an entity set
        if (!m_entity)
        {
            Debug.LogWarning("No entity set for StatBarUI - Defaulting to player");
            m_entity = FindObjectOfType<PlayerEntity>();
        }
    }

    // TODO: Make these bars listen to particular numbers with delegates
    void FixedUpdate()
    {
        // Set the foreground bar's width relative to the background bar
        EntityStat stat = m_entity.GetStat(m_statType);
        float statPercent = stat.m_currentValue / stat.m_maxValue;
        float fullWidth = m_background.rect.width;
        float rightOffset = (fullWidth * statPercent) - fullWidth;
        m_foreground.offsetMax = new Vector2(rightOffset, 0);
    }
}
