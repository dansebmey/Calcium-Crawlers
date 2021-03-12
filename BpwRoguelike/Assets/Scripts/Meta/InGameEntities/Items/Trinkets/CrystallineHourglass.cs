using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Trinkets/Crystalline Hourglass")]
public class CrystallineHourglass : Trinket
{
    [Range(0.1f, 0.5f)] public float slowmotionRatio;

    private void Awake()
    {
        EventManager<AbilityDetails>.AddListener(EventType.OnEnemyAttackInitiated, SlowDownTime);
    }

    private void SlowDownTime(AbilityDetails details)
    {
        Combatant combatantWithHourglass = details.targets.First(target => target is Combatant combatant
                                            && combatant.loadout.trinket != null
                                            && combatant.loadout.trinket is CrystallineHourglass
                                            && combatant.Hitpoints <= combatant.maxHP * 0.25f) as Combatant; 
        if (combatantWithHourglass != null)
        {
            Time.timeScale = slowmotionRatio;
            combatantWithHourglass.loadout.trinket = null;
        }
    }
}