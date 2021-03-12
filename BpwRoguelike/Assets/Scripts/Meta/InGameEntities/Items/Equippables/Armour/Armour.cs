using UnityEngine;

[CreateAssetMenu(menuName = "Item/Armour")]
public class Armour : ArmourItem
{
    public override float DmgAbsorbRatio()
    {
        return coverage * 0.01f;
    }

    public override void OnUseAsAbility(Combatant user)
    {
        base.OnUseAsAbility(user);

        user.loadout.SetArmour(user.loadout.armour == this ? null : this);
    }
}