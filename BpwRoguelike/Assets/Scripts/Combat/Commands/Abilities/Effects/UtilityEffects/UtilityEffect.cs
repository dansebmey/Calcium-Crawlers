using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public abstract class UtilityEffect : ScriptableObject
{
    [Header("UtilityEffect")]
    [Range(0, 1.0f)] public float mehSuccessRatio = 0;
    [Range(0, 1.0f)] public float okaySuccessRatio = 0.5f;
    [Range(0, 1.0f)] public float goodSuccessRatio = 0.9f;
    [Range(0, 1.0f)] public float perfectSuccessRatio = 1;
    public bool isBlockable;
    [Range(0, 1.0f)] public float okayBlockMultiplier = 0.9f;
    [Range(0, 1.0f)] public float goodBlockMultiplier = 0.75f;
    [Range(0, 1.0f)] public float perfectBlockMultiplier = 0.5f;
    
    public List<AbilityCommand.AbilityPool> uniqueToEquipmentCategories;

    public void OnApply(AbilityDetails details)
    {
        if (uniqueToEquipmentCategories.Count == 0
            || uniqueToEquipmentCategories.Count > 0 && details.performer.HasAccessToAbilityPool(uniqueToEquipmentCategories))
        {
            foreach (GridObject gridObject in details.targets)
            {
                var target = (Combatant) gridObject;
                if (SuccessfullyApplied(target, details))
                {
                    OnApplyEffect(target, details);
                }
            }
        }
    }

    protected abstract void OnApplyEffect(Combatant primaryTarget, AbilityDetails details);

    protected bool SuccessfullyApplied(Combatant target, AbilityDetails details)
    {
        float randomNo = Random.Range(0, 1.0f);
        return randomNo <= GetSuccessRatio(target, details);
    }
    
    private float GetSuccessRatio(Combatant target, AbilityDetails details)
    {
        float result = mehSuccessRatio;
        if (details.performance == TimedActionController.Performance.Okay)
        {
            result =  okaySuccessRatio;
        }
        else if (details.performance == TimedActionController.Performance.Good)
        {
            result =  goodSuccessRatio;
        }
        else if (details.performance == TimedActionController.Performance.Perfect)
        {
            result =  perfectSuccessRatio;
        }

        float blockPerformanceSuccessMultiplier = 1;
        if (isBlockable)
        {
            if (details.GetDefPerformance(target) == TimedActionController.Performance.Okay)
            {
                blockPerformanceSuccessMultiplier = okayBlockMultiplier;
            }
            else if (details.GetDefPerformance(target) == TimedActionController.Performance.Good)
            {
                blockPerformanceSuccessMultiplier = goodBlockMultiplier;
            }
            else if (details.GetDefPerformance(target) == TimedActionController.Performance.Perfect)
            {
                blockPerformanceSuccessMultiplier = perfectBlockMultiplier;
            }
        }

        return result * blockPerformanceSuccessMultiplier;
    }
}