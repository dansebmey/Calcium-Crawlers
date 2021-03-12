using System;

[Serializable]
public class BaseDamageEffect : DamageEffect
{
    protected override int BaseDamage(AbilityDetails details)
    {
        return 15 + details.performer.profile.StrLvl * 5;
    }
}