using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TreasureChest : GridObject
{
    public int damageToBreak;
    
    [Range(1, 100)] public int pointBudget;
    public List<LootData> possibleLoot;

    private Animator _animator;

    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponentInChildren<Animator>();
    }

    public override void TakeDamageFromAttack(AbilityDetails details)
    {
        float damage = details.command.CalculateNetDamageOutput(details);
        if (damage >= damageToBreak)
        {
            GenerateLoot();
            gm.combatManager.ClearAbilityQueue();
            gm.hudManager.ShakeScreen(2);
            gm.SwitchState(typeof(State_PlayerTurn_Loot));

            OnDisable();
        }
    }
    
    private void GenerateLoot()
    {
        gm.hudManager.lootInterface.ClearLoot();
        
        int remainingPointBudget = pointBudget;
        for (int i = remainingPointBudget; i > 0;)
        {
            List<Item> loot = DetermineItems(remainingPointBudget, out remainingPointBudget);
            if (loot != null && loot.Count > 0)
            {
                foreach (Item lootedItem in loot)
                {
                    gm.hudManager.lootInterface.AddItem(lootedItem);
                }
            }
            i = remainingPointBudget;
        }
    }

    private List<Item> DetermineItems(int remainingPoints, out int remainingPointsOut)
    {
        List<LootData> eligibleObjects = possibleLoot.Where(spawnData => spawnData.pointCost <= remainingPoints).ToList();

        int collectiveSpawnWeight = eligibleObjects.Sum(lData => lData.spawnWeight);
        int randomNo = Random.Range(0, collectiveSpawnWeight);
        eligibleObjects.Sort((d1, d2) => d1.spawnWeight.CompareTo(d2.spawnWeight));

        int cumulativeSpawnWeight = 0;
        foreach (LootData lootData in eligibleObjects)
        {
            cumulativeSpawnWeight += lootData.spawnWeight;
            if (cumulativeSpawnWeight >= randomNo)
            {
                remainingPointsOut = remainingPoints - lootData.pointCost;
                return lootData.items;
            }
        }
        
        remainingPointsOut = 0;
        return null;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _animator.Play("opened");
    }

    [Serializable]
    public class LootData
    {
        public List<Item> items;
        [Range(1, 100)] public int pointCost;
        [Range(1, 100)] public int spawnWeight;
        public bool allowDuplicates = true;
    }
}