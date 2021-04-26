using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NpcDifficulty
{
    Easy,
    Normal,
    Hard
}
public enum FactionType
{
    Human
}
[System.Serializable]
public class FactionSlot
{
    public bool isPlayer;

    public NpcDifficulty npcDifficulty;

    [SerializeField, Tooltip("Default faction type for this slot.")]
    private FactionTypeInfo factionTypeInfo = null; //Type of this faction (the type determines which extra buildings/units can this faction use).
    public FactionTypeInfo GetTFactionTypeInfo() { return factionTypeInfo; }

    public List<FactionLimit> factionLimits;
    public int maxPopulation;
}
