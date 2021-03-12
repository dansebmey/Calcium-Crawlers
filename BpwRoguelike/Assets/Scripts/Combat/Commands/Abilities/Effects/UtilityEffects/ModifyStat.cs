using UnityEngine;

[CreateAssetMenu(menuName = "Ability Effects/Utility/Stat modifier")]
public class ModifyStat : UtilityEffect
{
    public enum Stat { End, Str, Dex, Wis }

    [Header("ModifyStat")]
    public Stat stat;
    [Range(-3, 3)] public int modifier = 1;
    
    protected override void OnApplyEffect(Combatant primaryTarget, AbilityDetails details)
    {
        foreach (GridObject target in details.targets)
        {
            if (target is Combatant combatant)
            {
                if (stat == Stat.End)
                {
                    combatant.profile.EndLvl += modifier;
                }
                else if (stat == Stat.Str)
                {
                    combatant.profile.StrLvl += modifier;
                }
                else if (stat == Stat.Dex)
                {
                    combatant.profile.DexLvl += modifier;
                }
                else if (stat == Stat.Wis)
                {
                    combatant.profile.WisLvl += modifier;
                }
            }
        }
    }
}