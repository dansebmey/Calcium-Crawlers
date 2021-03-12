public class OffBalance : StatusEffect
{
    public OffBalance(Combatant combatant, int duration) : base(combatant, duration)
    {
    }

    protected override void OnRoundEndEffect(Combatant combatant)
    {
        
    }
}