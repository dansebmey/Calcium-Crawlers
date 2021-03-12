using System;
using UnityEngine;

[Serializable]
public abstract class DamageEffect
{
    protected virtual int BaseDamage(AbilityDetails details)
    {
        return 0;
    }

    protected virtual int BonusDamage(AbilityDetails details)
    {
        return 0;
    }
    
    public int RawDamage(AbilityDetails details)
    {
        return BaseDamage(details) + BonusDamage(details);
    }

    public int NetDamage(AbilityDetails details)
    {
        return (int) (RawDamage(details) * PerformanceMultiplier(details));
    }

    private float PerformanceMultiplier(AbilityDetails details)
    {
        switch (details.performance)
        {
            case TimedActionController.Performance.Meh:
                return details.command.mehMultiplier;
            case TimedActionController.Performance.Okay:
                return details.command.okayMultiplier;
            case TimedActionController.Performance.Good:
                return details.command.goodMultiplier;
            case TimedActionController.Performance.Perfect:
                return details.command.perfectMultiplier;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}