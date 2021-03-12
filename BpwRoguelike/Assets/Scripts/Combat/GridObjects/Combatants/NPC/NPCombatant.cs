using System.Runtime.InteropServices;
using UnityEngine;

public class NPCombatant : Combatant
{
    public enum Difficulty { Easy, Medium, Hard, Brutal }
    public Difficulty difficulty;

    [HideInInspector] public float chanceOfGoodPerformance;
    [HideInInspector] public float chanceOfPerfectPerformance;
    [HideInInspector] public float chanceToBlock;

    protected override void Start()
    {
        base.Start();

        chanceOfGoodPerformance = 0.25f + (int) difficulty * 0.15f;
        chanceOfPerfectPerformance = 0.1f + (int) difficulty * 0.05f;
        chanceToBlock = 0.1f + (int) difficulty * 0.1f;
    }

    public TimedActionController.Performance DetermineBlockPerformance()
    {
        float rand = Random.Range(0.0f, 1.0f);

        if (rand <= chanceOfPerfectPerformance)
        {
            return TimedActionController.Performance.Perfect;
        }
        if (rand <= chanceOfGoodPerformance)
        {
            return TimedActionController.Performance.Good;
        }

        return TimedActionController.Performance.Okay;
    }

    public bool WantsToUseAbility(Command command)
    {
        // foreach (NPCPerformCondition condition in ability.performConditions)
        // {
        //     // if (!condition.IsMet(gm.combatManager.CurrentRound, this))
        //     // {
        //     //     return false;
        //     // }
        // }

        return true;
    }

    public TimedActionController.Performance DetermineNPCPerformance()
    {
        float chanceForPerfect = 0.1f;
        float chanceForGood = 0.1f;
        float chanceForOkay = 0.1f;

        if (difficulty == Difficulty.Easy)
        {
            chanceForPerfect = 0.1f;
            chanceForGood = 0.25f;
            chanceForOkay = 0.75f;
        }
        else if (difficulty == Difficulty.Medium)
        {
            chanceForPerfect = 0.25f;
            chanceForGood = 0.4f;
            chanceForOkay = 0.75f;
        }
        else if (difficulty == Difficulty.Hard)
        {
            chanceForPerfect = 0.4f;
            chanceForGood = 0.75f;
            chanceForOkay = 0.875f;
        }
        else if (difficulty == Difficulty.Brutal)
        {
            chanceForPerfect = 0.75f;
            chanceForGood = 0.85f;
            chanceForOkay = 0.9f;
        }

        float randomNo = Random.Range(0, 1.0f);
        if (randomNo <= chanceForPerfect)
        {
            return TimedActionController.Performance.Perfect;
        }
        if (randomNo <= chanceForGood)
        {
            return TimedActionController.Performance.Good;
        }
        if (randomNo <= chanceForOkay)
        {
            return TimedActionController.Performance.Okay;
        }
        else
        {
            return TimedActionController.Performance.Meh;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        Trinket killersTrinket = gm.combatManager.CurrentActor.loadout.trinket;
        if (killersTrinket != null && killersTrinket is Nocimonollerom nocimonollerom)
        {
            nocimonollerom.Resurrect(this, point);
        }
    }
}