using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType
{
    PLAYER,
    NPC,
    ITEM,
    OTHER
}

public enum BasicActionType
{
    NONE,
    EXAMINE,
    TALKTO,
    ACTIVATE,
    USE,
    PICKUP,
    ATTACK
}

public enum ActionType
{
    NONE = BasicActionType.NONE,
    EXAMINE = BasicActionType.EXAMINE,
    TALKTO = BasicActionType.TALKTO,
    ACTIVATE = BasicActionType.ACTIVATE,
    USE = BasicActionType.USE,
    PICKUP = BasicActionType.PICKUP,
    ATTACK = BasicActionType.ATTACK,
    EQUIP,

}

public class WorldEntity : MonoBehaviour {

    [SerializeField]
    private EntityData _data;
    public EntityData m_data
    {
        get
        {
            if(!_data)
            {
                Debug.LogError("EntityData not set for WorldEntity " + name);
            }
            return _data;
        }
        protected set { _data = value; }
    }

    public EntityType m_entityType;

    public WorldEntity()
    {
        m_entityType = EntityType.OTHER;
        m_primaryAction = BasicActionType.NONE;
        m_secondaryAction = BasicActionType.EXAMINE;
        InitActions();
    }

    protected WorldEntity(BasicActionType primary, BasicActionType secondary, EntityType type)
    {
        m_entityType = type;
        m_primaryAction = primary;
        m_secondaryAction = secondary;
        InitActions();
    }

    // Do not override pls
    protected void Awake()
    {
        // Use a copy of the EntityData at runtime
        _data = Instantiate(_data);
    }

    /** On Destruct listener */
    // TODO: Make an "OnStateChange(oldState, newState)" listener instead?
    public delegate void OnDestruct();
    public event OnDestruct m_onDestruct;

    public void AddOnDestructListener(OnDestruct listener)
    {
        m_onDestruct += listener;
    }

    public void RemoveOnDestructListener(OnDestruct listener)
    {
        m_onDestruct -= listener;
    }

    void OnDestroy()
    {
        if (m_onDestruct != null)
        {
            m_onDestruct();
        }
    }

    /** Actions */

    public delegate bool Action(WorldEntity other);
    public delegate void Reaction(WorldEntity other);

    /** List of actions for this entity when it is the one performing the action */
    protected Dictionary<BasicActionType, Action> m_actions;
    /** List of actions for this entity when it is the one being acted upon */
    protected Dictionary<BasicActionType, Reaction> m_reactions;
    /** Default left-click action for this entity */
    private BasicActionType m_primaryAction;
    /** Default right-click action for this entity */
    private BasicActionType m_secondaryAction;

    /** Initialise the action dictionaries - UPDATE THIS whenever adding a new action! */
    private void InitActions()
    {
        m_actions = new Dictionary<BasicActionType, Action>();
        m_reactions = new Dictionary<BasicActionType, Reaction>();

        // Add new actions here.

        m_actions.Add(BasicActionType.EXAMINE, Examine);
        m_reactions.Add(BasicActionType.EXAMINE, OnExamined);
        m_actions.Add(BasicActionType.TALKTO, TalkTo);
        m_reactions.Add(BasicActionType.TALKTO, OnTalkedTo);
        m_actions.Add(BasicActionType.ACTIVATE, Activate);
        m_reactions.Add(BasicActionType.ACTIVATE, OnActivated);
        m_actions.Add(BasicActionType.PICKUP, PickUp);
        m_reactions.Add(BasicActionType.PICKUP, OnPickUp);
    }

    /** This entity performs the subject's default left-click action on it */
    public bool PerformPrimaryActionOn(WorldEntity subject)
    {
        return PerformBasicActionOn(subject.m_primaryAction, subject);
    }

    /** Make this entity perform the subject's default right-click action on it */
    public bool PerformSecondaryActionOn(WorldEntity subject)
    {
        return PerformBasicActionOn(subject.m_secondaryAction, subject);
    }

    /** This entity attempts to perform the given action on the subject */
    public bool PerformBasicActionOn(BasicActionType actionType, WorldEntity subject)
    {
        Action action = null;
        if (m_actions.TryGetValue(actionType, out action))
        {
            // Do we perform the action?
            if (action(subject))
            {
                Reaction reaction = null;
                if (subject.m_reactions.TryGetValue(actionType, out reaction))
                {
                    // The subject reacts to the action
                    reaction(this);
                }
                return true;
            }
        }
        return false;
    }

    /** Basic Actions */
    /** Actions with no parameters that return true if they are successful (e.g. the player is strong enough to pickup another object) */
    /** Reactions with no parameters that should only be called if the proceeding action is successful. */

    protected virtual bool Examine(WorldEntity other) { return true; }
    protected virtual void OnExamined(WorldEntity other)
    {
        if (other.m_entityType == EntityType.PLAYER)
        {
            TopLevelUI.notifyText.SetText("You see " + m_data.examineName + ".");
        }
    }

    protected virtual bool TalkTo(WorldEntity other) { return true; }
    protected virtual void OnTalkedTo(WorldEntity other)
    {
        if (other.m_entityType == EntityType.PLAYER) Debug.Log("You get no response");
    }

    protected virtual bool Activate(WorldEntity other) { return true; }
    protected virtual void OnActivated(WorldEntity other)
    {
        if (other.m_entityType == EntityType.PLAYER) Debug.Log("Nothing happens.");
    }

    protected virtual bool UseObjectWithObject(WorldEntity objA, WorldEntity objB) { objA.OnUsedWithObject(this, objB); objB.OnUsedWithObject(this, objA); return true; }
    protected virtual void OnUsedWithObject(WorldEntity usedBy, WorldEntity otherObject)
    {
        if (usedBy.m_entityType == EntityType.PLAYER) Debug.Log("Nothing happens.");
    }

    protected virtual bool PickUp(WorldEntity other) { return true; }
    protected virtual void OnPickUp(WorldEntity other)
    {
        Debug.Log("Lifted " + m_data.examineName);
    }

    /** Parametered Actions */
    /** Actions with specialised parameters have their own invoke calls */

    /** Call PerformEquipActionOn just before moving the item into the inventory slot. */
    public bool PerformEquipActionOn(WorldEntity other, InventorySlot slot)
    {
        if (Equip(other, slot))
        {
            other.OnEquipped(this, slot);
            return true;
        }
        return false;
    }
    protected virtual bool Equip(WorldEntity other, InventorySlot slot) { return slot.SetEntity(other); }
    protected virtual void OnEquipped(WorldEntity other, InventorySlot slot) { Debug.Log("Equipped something inconsequential."); }


    /** Passive Actions */

    protected virtual void OnWalk() { }

    protected virtual void Attack(WorldEntity other) { other.OnAttacked(this); }
    protected virtual void OnAttacked(WorldEntity other) { }

    // public void Something temperature related?


    public EntityStat GetStat(EntityStatType statType)
    {
        if (m_data && m_data.stats.ContainsKey(statType))
        {
            return m_data.stats[statType];
        }
        Debug.LogWarning("Attempted to get missing stat '" + statType + "' from entity '" + m_data.examineName + "'");
        return null;
    }
}
