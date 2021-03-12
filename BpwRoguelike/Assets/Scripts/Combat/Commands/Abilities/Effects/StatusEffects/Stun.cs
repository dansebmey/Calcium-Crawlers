using UnityEngine;

public class Stun : StatusEffect
{
    public Stun(Combatant combatant, int duration) : base(combatant, duration)
    {
    }

    protected override void OnRoundEndEffect(Combatant combatant)
    {
        
    }
}