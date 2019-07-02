using System.Collections;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;
using UnityEngine.Assertions;

/** Identifiers for different stats - DO NOT REORDER (SerializedDictionary does not like it!) */
public enum EntityStatType
{
    NONE,
    HEALTH,
    DURABILITY,
    HUNGER,
    AWAKENSS,
    CHARGES
}

/** Base class for an entity stat e.g. Health, Durability, Strength, etc.
  * TODO: Let this handle negative values for Increase/Decrease
     */
[System.Serializable]
public class EntityStat
{
    [SerializeField]
    public float m_minValue = 0.0f;
    [SerializeField]
    public float m_maxValue = 100.0f;
    [SerializeField]
    public float m_currentValue = 100.0f;

    /** Increase this stat by the given amount.
     *  If allowOverflow == true, currentValue may exceed maxValue.
     *  If the stat has already overflowed, it won't be clamped back to m_maxValue even if this call doesn't allow overflow
     */
    public void Increase(float amount, bool allowOverflow = false)
    {
        // TODO: Let this handle negative values for Increase/Decrease
        Assert.IsTrue(amount >= 0, "EntityStat Increase amount is negative");

        // If we have already overflowed, we don't want to clamp back to m_maxValue
        bool alreadyOverflowed = m_currentValue > m_maxValue;

        if (alreadyOverflowed && allowOverflow == false)
        {
            // We are already over our max value, but this increase cannot overflow it any more
            return;
        }

        // Apply increase
        m_currentValue += amount;

        if (allowOverflow == false)
        {
            // Clamp to our max value
            m_currentValue = Mathf.Min(m_currentValue, m_maxValue);
        }
    }

    /** Decrease this stat by the given amount.
     *  If allowUnderflow == true, currentValue may go below minValue.
     *  If the stat has already underflowed, it won't be clamped back to m_minValue even if this call doesn't allow underflow
     */
    public void Decrease(float amount, bool allowUnderflow = false)
    {
        // TODO: Let this handle negative values for Increase/Decrease
        Assert.IsTrue(amount >= 0, "EntityStat Decrease amount is negative");

        bool alreadyUnderflowed = m_currentValue < m_minValue;

        if (alreadyUnderflowed && allowUnderflow == false)
        {
            // We are already under our min value, but this decrease cannot underflow it any more
            return;
        }

        // Apply decrease
        m_currentValue -= amount;

        if (allowUnderflow == false)
        {
            // Clamp to our min value
            m_currentValue = Mathf.Max(m_currentValue, m_minValue);
        }
    }
}

// Serializable dictionary for EntityStats
[System.Serializable]
public class EntityStatsDictionary : SerializableDictionaryBase<EntityStatType, EntityStat> { }
