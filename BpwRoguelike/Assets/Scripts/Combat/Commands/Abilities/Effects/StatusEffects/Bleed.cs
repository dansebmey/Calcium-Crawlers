using UnityEngine;

public class Bleed : StatusEffect
{
    public Bleed(Combatant combatant, int duration) : base(combatant, duration)
    {
        
    }
    
    protected override void OnRoundEndEffect(Combatant combatant)
    {
        combatant.TakeDOTDamage((int)Mathf.Clamp(combatant.Hitpoints * 0.1f, combatant.maxHP * 0.05f, combatant.Hitpoints * 0.1f));
    }
}