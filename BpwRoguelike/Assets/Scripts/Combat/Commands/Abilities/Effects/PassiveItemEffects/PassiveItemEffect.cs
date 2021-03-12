using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public abstract class PassiveItemEffects : ScriptableObject
{
    [Range(0, 1)] public float mehRatio;
    [Range(0, 1)] public float okayRatio;
    [Range(0, 1)] public float goodRatio;
    [Range(0, 1)] public float perfectRatio;

    public float maxBonusToBaseDamageRatio = 1;
    // a maxBonusToBaseDamageRatio of 1 means that the bonus damage has a cap of 100% of the ability's base damage  
    
    public abstract int ActivateEffect(AbilityDetails details, Combatant defender);
    
    protected float GetRatio(TimedActionController.Performance performance)
    {
        if (performance == TimedActionController.Performance.Meh)
        {
            return mehRatio;
        }
        if (performance == TimedActionController.Performance.Okay)
        {
            return okayRatio;
        }
        if (performance == TimedActionController.Performance.Good)
        {
            return goodRatio;
        }
        if (performance == TimedActionController.Performance.Perfect)
        {
            return perfectRatio;
        }

        Debug.LogWarning("Performance ["+performance+"] not found");
        return 0;
    }
}