using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CharacterProfile))]
[RequireComponent(typeof(Loadout))]
[RequireComponent(typeof(Inventory))]
public abstract class Combatant : GridObject
{
    // meta
    [HideInInspector] public CharacterProfile profile;
    [HideInInspector] public Loadout loadout;
    [HideInInspector] public Inventory inventory;

    // components
    private SpriteRenderer _spRenderer;
    public Sprite Sprite => _spRenderer.sprite;
    private Color _originalSpriteColour;

    private ResourcePanel _resourcePanel;
    private LayeredText _speedIndicatorText;
    private Animator _animator;
    public Animator Animator => _animator;
    private SpriteRenderer _turnIndicator;
    [HideInInspector] public Pathfinder pathfinder;
    
    private GridPos _dir;
    private SpriteRenderer _dirPointer;
    public GridPos Dir
    {
        get => _dir;
        set
        {
            int dirX = Math.Sign(value.x); 
            int dirY = Math.Sign(value.y); 
            _dir = new GridPos(dirX, dirY);
            
            int angle = 0; 
            if (dirX == 0 && dirY == -1) angle = -90;
            else if (dirX == -1 && dirY == 0) angle = -180;
            else if (dirX == 0 && dirY == 1) angle = -270;
            
            _dirPointer.transform.localRotation = Quaternion.Euler(0, 0, angle);
            
            // Debug.Log("dir set to [" + _dir.x +"," + _dir.y + "]");
        }
    }
    
    // states
    public enum State { Idle, Blocking, Channelling, Fallen }
    public State state;
    
    // parameters
    [Range(10, 1000)] public int maxHP = 100;
    
    private int _hitpoints;

    public virtual int Hitpoints
    {
        get => _hitpoints;
        set
        {
            _hitpoints = Mathf.Clamp(value, 0, maxHP);

            if (_hitpoints == 0)
            {
                OnDisable();
            }
            else
            {
                gm.hudManager?.UpdateHP(this);
                _resourcePanel.healthBar.value = _hitpoints;   
            }
        }
    }


    private int _energy;

    public int Energy
    {
        get => _energy;
        set
        {
            _energy = Mathf.Clamp(value, 0, MaxEnergy);
            gm.hudManager?.UpdateEnergy(this);
            _resourcePanel.energyBar.value = _energy;
        }
    }

    public int manuallySetMaxEnergy = -1;
    public int MaxEnergy => manuallySetMaxEnergy > 0 ? manuallySetMaxEnergy : 75 + profile.DexLvl * 25;

    public int Speed => 10 - (int)CarriedWeight() + profile.EndLvl;
        // + (int)((10.0f / MaxEnergy) * Energy);

    public int baseMovRange = 3;
    public int MovRange
    {

        get
        {
            int result = baseMovRange;
            if (loadout.trinket != null)
            {
                result += loadout.trinket.movRangeModifier;
            }
            
            return result;
        }
    }
    
    public int attackRange = 1;
    
    // turn-specific data
    public GridPos StartingPoint { get; private set; }
    
    private float _consecutiveHitMultiplier = 1;

    protected override void Awake()
    {
        base.Awake();

        profile = GetComponent<CharacterProfile>();
        loadout = GetComponent<Loadout>();
        inventory = GetComponent<Inventory>();
        inventory.AssignTo(this);

        _spRenderer = GetComponentInChildren<SpriteRenderer>();
        _resourcePanel = GetComponentInChildren<ResourcePanel>();
        _speedIndicatorText = GetComponentInChildren<LayeredText>(true);
        _animator = GetComponentInChildren<Animator>();
        _turnIndicator = GetComponentsInChildren<SpriteRenderer>(true)[1];
        _dirPointer = GetComponentsInChildren<SpriteRenderer>(true)[2];
        pathfinder = FindObjectOfType<Pathfinder>();

        _originalSpriteColour = _spRenderer.color;
    }

    protected override void Start()
    {
        base.Start();
        
        _resourcePanel.healthBar.maxValue = maxHP;
        Hitpoints = maxHP;
        
        _resourcePanel.energyBar.maxValue = MaxEnergy;
        Energy = MaxEnergy;
        
        OnTurnStart();
    }

    public List<AbilityCommand> GetCurrentlyPerformableAbilities()
    {
        List<AbilityCommand> result = new List<AbilityCommand>();

        if (loadout.weapon != null)
        {
            result.AddRange(profile.learnedAbilities.Where(a => loadout.weapon.CanPerformAbility(a)));
        }
        if (loadout.shield != null)
        {
            result.AddRange(profile.learnedAbilities.Where(a => loadout.shield.CanPerformAbility(a)));
        }
        
        return result;
    }

    public void ShowMovePriority()
    {
        _speedIndicatorText.Text = Speed.ToString();
        _turnIndicator.gameObject.SetActive(gm.combatManager.CurrentActor == null || gm.combatManager.CurrentActor == this);
    }
    
    public virtual void OnTurnStart()
    {
        ShowMovePriority();
        
        StartingPoint = Point;
        traversibleTiles = gm.tileManager.GetTilesWithinMovementRange(this);
    }

    public void PerformAbility(AbilityDetails details)
    {
        if (details.command.scope == AbilityCommand.Scope.SingleEnemy)
        {
            details.performer.LookAt(details.targets[0].point);
        }
        details.OnPerform();
        ConsumeEnergy(details.command);
    }

    public void DealDamage(AbilityDetails abilityDetails)
    {
        foreach (GridObject target in abilityDetails.targets)
        {
            target.TakeDamageFromAttack(abilityDetails);
        }
    }
    
    public override void TakeDamageFromAttack(AbilityDetails details)
    {
        float netDamage = details.command.CalculateNetDamageOutput(details);
        float minDamageMultiplier = Mathf.Clamp(0.625f + (loadout.weapon.accuracy * 0.075f) * _consecutiveHitMultiplier, 0, 1);

        float bonusDamage = details.command.GetBonusDamage(netDamage, details, this);
        netDamage += bonusDamage;
        
        netDamage *= (1.05f + loadout.weapon.damage * 0.15f) * _consecutiveHitMultiplier * Random.Range(minDamageMultiplier, 1);
        if (netDamage <= 0)
        {
            return;
        }
        
        HandleConsecutiveHitRegistration(details);
        
        netDamage -= CalculateDamageReduction(netDamage, details);
        RemoveStatusEffect(typeof(OffBalance));

        if (netDamage > 0)
        {
            Hitpoints -= (int) netDamage;

            // if (netDamage >= Hitpoints * 0.25f && Random.Range(0.0f, 1.0f) < 0.25f)
            // {
            //     Debug.Log("Bleed applied!");
            //     ApplyStatusEffect(new Bleed(this, 2));
            // }

            overheadHud.hitsplat.Show((int) netDamage, (int) details.performance);
            gm.hudManager.ShakeScreen(0.3f + (1.0f + (int) details.performance) / maxHP * netDamage);
        }

        // Debug.Log(details.performance + " strike" +
        //           (blocked ? " -vs " + details.GetDefPerformance(this) + " block" : "") +
        //           "! " + profile.characterName + " took " + (int)netDamage + " damage (" + shieldDmgReduction + " blocked, " + armourDmgReduction + " absorbed by armour).");
    }

    private float CalculateDamageReduction(float baseDamage, AbilityDetails details)
    {
        float damageReduction = 0;
        
        bool canDefend = HasStatusEffect(typeof(OffBalance), typeof(Stun));
        if (!canDefend)
        {
            TimedActionController.Performance defPerformance = details.GetDefPerformance(this);
            bool dodged = loadout.shield == null &&
                          defPerformance == TimedActionController.Performance.Perfect;
            if (dodged)
            {
                _animator.Play("Dodge");
                overheadHud.hitsplat.Show("Dodged!", new Color(0.43f, 0.7f, 1));
                return damageReduction = baseDamage;
            }

            bool blocked = loadout.shield != null && state == State.Blocking && (int)defPerformance > 0;
            if (blocked)
            {
                LookAt(details.performer.point);

                float blockPerformanceMultiplier = 0.2f + (int)defPerformance * 0.2f;
                int shieldDmgReduction = (int) Mathf.Clamp(
                    baseDamage * loadout.shield.DmgAbsorbRatio() * blockPerformanceMultiplier,
                    0, loadout.shield.sturdiness);

                overheadHud.blockSplat.ShowShield(shieldDmgReduction);
                damageReduction += shieldDmgReduction;
            }
            else
            {
                _animator.Play("Hit");
            }
        }
            
        if (loadout.armour != null)
        {
            int armourDmgReduction = (int) (baseDamage * Mathf.Clamp(loadout.armour.DmgAbsorbRatio()
                                                                 - details.performer.loadout.DetermineFlatArmourPen(
                                                                     details.performance), 0, 1));
            damageReduction += armourDmgReduction;
        }

        return damageReduction;
    }

    public void TakeDOTDamage(int damage)
    {
        Hitpoints -= damage;
        overheadHud.hitsplat.Show(damage, 0); // currently does not play because Hitsplat animation is still playing at the end of turn
    }

    private void HandleConsecutiveHitRegistration(AbilityDetails details)
    {
        if (details.performance == TimedActionController.Performance.Perfect)
        {
            _consecutiveHitMultiplier += 0.125f;
        }
        else if (details.performance == TimedActionController.Performance.Good)
        {
            _consecutiveHitMultiplier = Mathf.Clamp(_consecutiveHitMultiplier - 0.125f, 1, 1.5f);
        }
        else if (details.performance < TimedActionController.Performance.Good)
        {
            _consecutiveHitMultiplier = 1;
        }
    }

    private void ResetConsecutivePerfectStrikesTaken()
    {
        _consecutiveHitMultiplier = 1;
    }

    private void ConsumeEnergy(Command command)
    {
        Energy -= command.GetNetEnergyCost(this);
    }
    

    private float CarriedWeight()
    {
        float total = inventory.items.Sum(item => item.weight);

        return total;
    }

    public void PlayAbilityAnimation()
    {
        _animator.Play("Attack"); // TODO: replace with _pendingAbility.abilityName
    }

    public bool HasAbilityThatCanReach(GridPos from, GridPos to)
    {
        return GetCurrentlyPerformableAbilities().Any(ability => ability.CanReach(from, to, loadout.weapon));
    }
    
    public void OnTurnEndAsActor()
    {
        _spRenderer.color = new Color(0.5f, 0.5f, 0.5f);
        ShowMovePriority();

        gm.hudManager.inputInfoInterface.Clear();
    }

    public void OnTurnEndAsNonActor()
    {
        ShowMovePriority();
        ResetConsecutivePerfectStrikesTaken();
    }

    public void OnRoundEnd()
    {
        _spRenderer.color = _originalSpriteColour;
        RegenEnergy();
        for (int i = _statusEffects.Count-1; i >= 0; i--)
        {
            if (_statusEffects[i].OnRoundEnd(this))
            {
                RemoveStatusEffect(_statusEffects[i]);
            }
        }
    }

    private readonly List<StatusEffect> _statusEffects = new List<StatusEffect>();
    public List<Tile> traversibleTiles;

    public void ApplyStatusEffect(StatusEffect statusEffect)
    {
        Debug.Log("StatusEffect [" + statusEffect + "] added to [" + profile.characterName + "]");
        _statusEffects.Add(statusEffect);
    }

    private void RemoveStatusEffect(Type type)
    {
        if (_statusEffects.Any(effect => effect.GetType() == type))
        {
            RemoveStatusEffect(_statusEffects.First(effect => effect.GetType() == type));
        }
    }

    private void RemoveStatusEffect(StatusEffect effect)
    {
        Debug.Log("StatusEffect [" + effect + "] removed from [" + profile.characterName + "]");
        _statusEffects.Remove(effect);
    }

    private void RegenEnergy()
    {
        Energy += 25;
    }

    public bool HasStatusEffect(params Type[] types)
    {
        foreach (Type type in types)
        {
            if (_statusEffects.Any(statusEffect => statusEffect.GetType() == type))
            {
                return true;
            }
        }

        return false;
    }

    public bool HasAccessToAbilityPool(List<AbilityCommand.AbilityPool> abilityPools)
    {
        foreach (Command command in GetCurrentlyPerformableAbilities())
        {
            foreach (AbilityCommand.AbilityPool pool in abilityPools)
            {
                if (!(command is AbilityCommand ac) || ac.compatibleWeaponTypes.Contains(pool))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void LookAt(GridPos targetPos)
    {
        Dir = new GridPos(Tile.point.x - targetPos.x, Tile.point.y - targetPos.y);
    }

    public void LearnAbility(AbilityCommand ability)
    {
        if (!profile.learnedAbilities.Contains(ability))
        {
            profile.learnedAbilities.Add(ability);   
        }
    }

    public void UnlearnAbility(AbilityCommand ability)
    {
        if (profile.learnedAbilities.Contains(ability))
        {
            profile.learnedAbilities.Remove(ability);
        }
    }

    public void AccessInventory()
    {
        gm.SwitchState(typeof(State_PlayerTurn_Inventory));
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        state = State.Fallen;
        _animator.Play("Fallen");
        gm.hudManager.OnCombatantHasFallen(this);
        _resourcePanel.gameObject.SetActive(false);
        _spRenderer.sortingOrder = -10;
        _dirPointer.gameObject.SetActive(false);
    }
}
