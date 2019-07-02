using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum EquipMask
{
    UNSPECIFIED = 0,
    // Bitmask for equipment slots
    HELD_ITEM = 1 << 0,
    BAG_SLOT = 1 << 1,
    HEAD = 1 << 2,
    BODY = 1 << 3,
    LEGS = 1 << 4,
    FEET = 1 << 5,
    HANDS = 1 << 6,
    MAIN_HAND = 1 << 7,
    OFF_HAND = 1 << 8,
    WRISTS = 1 << 9,
    BACK = 1 << 10,
    NECK = 1 << 11,
    RING = 1 << 12,
}
