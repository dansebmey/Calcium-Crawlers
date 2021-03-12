using UnityEngine;

[RequireComponent(typeof(Player))]
public class PCombatant : Combatant
{
    [HideInInspector] public Player player;
    
    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
    }

    public override void OnTurnStart()
    {
        base.OnTurnStart();
        gm.hudManager.inventoryInterface.AssignTo(this);
    }
    
    public override int Hitpoints
    {
        get => base.Hitpoints;
        set
        {
            int previousHP = base.Hitpoints;
            base.Hitpoints = value;

            int lostHP = previousHP - value;
            float percentLostHP = 1.0f / maxHP * lostHP;

            EventManager<float>.Invoke(EventType.OnPlayerDamageTaken, percentLostHP);
        }
    }
}