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

public enum ActionType
{
    NONE,
    EXAMINE,
    TALKTO,
    ACTIVATE,
    USE,
    PICKUP
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

    /** Actions */

    public delegate bool Action(WorldEntity other);
    public delegate void Reaction(WorldEntity other);

    /** List of actions for this entity when it is the one performing the action */
    protected Dictionary<ActionType, Action> m_actions;
    /** List of actions for this entity when it is the one being acted upon */
    protected Dictionary<ActionType, Reaction> m_reactions;
    /** Default left-click action for this entity */
    private ActionType m_primaryAction;
    /** Default right-click action for this entity */
    private ActionType m_secondaryAction;

    public WorldEntity()
    {
        m_entityType = EntityType.OTHER;
        m_primaryAction = ActionType.NONE;
        m_secondaryAction = ActionType.EXAMINE;
        InitActions();
    }

    protected WorldEntity(ActionType primary, ActionType secondary)
    {
        m_primaryAction = primary;
        m_secondaryAction = secondary;
        InitActions();
    }

    /** Initialise the action dictionaries - UPDATE THIS whenever adding a new action! */
    private void InitActions()
    {
        m_actions = new Dictionary<ActionType, Action>();
        m_reactions = new Dictionary<ActionType, Reaction>();

        // Add new actions here.

        m_actions.Add(ActionType.EXAMINE, Examine);
        m_reactions.Add(ActionType.EXAMINE, OnExamined);
        m_actions.Add(ActionType.TALKTO, TalkTo);
        m_reactions.Add(ActionType.TALKTO, OnTalkedTo);
        m_actions.Add(ActionType.ACTIVATE, Activate);
        m_reactions.Add(ActionType.ACTIVATE, OnActivated);
        m_actions.Add(ActionType.PICKUP, PickUp);
        m_reactions.Add(ActionType.PICKUP, OnPickUp);
    }

    /** This entity performs the subject's default left-click action on it */
    public bool PerformPrimaryActionOn(WorldEntity subject)
    {
        return PerformActionOn(subject.m_primaryAction, subject);
    }

    /** Make this entity perform the subject's default right-click action on it */
    public bool PerformSecondaryActionOn(WorldEntity subject)
    {
        return PerformActionOn(subject.m_secondaryAction, subject);
    }

    /** This entity attempts to perform the given action on the subject */
    public bool PerformActionOn(ActionType actionType, WorldEntity subject)
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

    /** Actions */
    /** Actions return true if they are successful (e.g. the player is strong enough to pickup another object) */

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

    protected virtual bool Equip(WorldEntity other) { return true; /* other.IsEquippable */ }
    protected virtual void OnEquipped(WorldEntity other) { }

    /** Passive Actions */

    protected virtual void OnWalk() { }

    protected virtual void Attack(WorldEntity other) { other.OnAttacked(this); }
    protected virtual void OnAttacked(WorldEntity other) { }

    // public void Something temperature related?

}
