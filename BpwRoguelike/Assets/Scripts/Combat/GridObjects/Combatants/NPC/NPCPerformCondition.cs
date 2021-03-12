using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class NPCPerformCondition
{
    [Header("Health-based")]
    [Range(0.05f, 0.9f)] public float ownHealthBelow = -1;
    [Range(0.05f, 0.9f)] public float targetHealthBelow = -1;

    [Header("Periodical")]
    [Range(2, 5)] public int turnInterval = -1;
    [Range(1, 5)] public int startingOnTurn = -1;

    [Header("Other")]
    [Range(0.1f, 1)] public float chance = 1;

    public bool IsMet(int currentTurn, Combatant source, GridObject target)
    {
        bool result =
            (ownHealthBelow <= 0 || source.Hitpoints <= source.maxHP * ownHealthBelow)
            && (targetHealthBelow <= 0 || !(target is Combatant combatant) || combatant.Hitpoints <= combatant.maxHP * targetHealthBelow)
            && (turnInterval + startingOnTurn <= 0 || currentTurn == startingOnTurn ||
                currentTurn % turnInterval == startingOnTurn)
            && Random.Range(0.0f, 1.0f) <= chance;

        return result;
    }
}