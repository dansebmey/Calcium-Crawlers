using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatManager : Manager
{
    [HideInInspector] public List<GridObject> gridObjects;
    [HideInInspector] public List<Combatant> combatants;
    
    public Queue<Combatant> actorQueue = new Queue<Combatant>();

    public Combatant CurrentActor
    {
        get
        {
            return actorQueue.Count > 0 ? actorQueue.Peek() : null;
        }
    }

    [HideInInspector] public bool firstTimeAttacking = true;
    [HideInInspector] public bool firstTimeDefending = true;

    private Queue<AbilityDetails> __abilityQueue = new Queue<AbilityDetails>();

    private Queue<AbilityDetails> _abilityQueue
    {
        get => __abilityQueue;
        set => __abilityQueue = value;
    }
    public AbilityDetails CurrentAbilityDetails => _abilityQueue.Count > 0 ? _abilityQueue.Peek() : null;
    public int CurrentRound { get; private set; } = 1;

    public AbilityCommand useItemAbility;
    public AbilityCommand discardItemAbility;

    protected override void Awake()
    {
        base.Awake();
        
        EventManager<GridObject>.AddListener(EventType.RegisterGridObject, RegisterCombatant);
    }

    private void RegisterCombatant(GridObject gridObject)
    {
        gridObjects.Add(gridObject);
        if (gridObject is Combatant combatant)
        {
            combatants.Add(combatant);
        }
    }

    private void DetermineMoveOrder()
    {
        List<Combatant> combatantsToSort = new List<Combatant>();
        foreach (GridObject gridObject in gridObjects)
        {
            if (gridObject is Combatant combatant)
            {
                combatantsToSort.Add(combatant);
            }
        }
        
        List<Combatant> orderedCombatantList = combatantsToSort;
        orderedCombatantList.Sort(
            delegate (Combatant c1, Combatant c2)
            {
                return c2.Speed.CompareTo(c1.Speed);
            }
        );

        actorQueue = new Queue<Combatant>();
        for (int i = 0; i < orderedCombatantList.Count; i++)
        {
            Combatant combatant = orderedCombatantList[i];
            actorQueue.Enqueue(combatant);
        }
    }

    public void RemoveGridObject(GridObject gridObject)
    {
        gridObject.Tile.Occupant = null;
        gridObjects.Remove(gridObject);
        if (gridObject is Combatant combatant)
        {
            combatants.Remove(combatant);
            CheckForGameOver(combatant);
        }
    }

    private void CheckForGameOver(Combatant removedCombatant)
    {
        if (removedCombatant.profile.characterName == "You")
        {
            gm.SwitchState(typeof(State_GameOver_Loss));
        }
        else if (combatants.Count(cbt => cbt is NPCombatant) == 0)
        {
            gm.SwitchState(typeof(State_GameOver_Win));
        } 
    }

    public void EndTurn()
    {
        gm.tileManager.RemoveFlags();
        foreach (Combatant combatant in combatants)
        {
            combatant.OnTurnEndAsNonActor();
        }
        
        Combatant actor = CurrentActor;
        actor.OnTurnEndAsActor();
        
        actorQueue.Dequeue();
        if (actorQueue.Count == 0)
        {
            DetermineMoveOrder(); // GDVDKK
            CurrentActor.OnTurnEndAsActor();
            gm.SwitchState(typeof(State_RoundStart));
        }
        else
        {
            StartNextTurn();
        }
        CurrentActor.ShowMovePriority();
    }

    public void PrepareNewRound()
    {
        CurrentRound++;
        Debug.Log("Round [" + CurrentRound + "] started");
        
        DetermineMoveOrder();
        foreach (Combatant combatant in combatants)
        {
            combatant.OnRoundEnd();
        }
    }

    public void StartNewRound()
    {
        StartNextTurn();
    }
    
    private void StartNextTurn()
    {
        if (CurrentActor.state == Combatant.State.Fallen)
        {
            actorQueue.Dequeue();
            if (actorQueue.Count == 0)
            {
                DetermineMoveOrder();
            }
        }
        
        ClearAbilityQueue();

        CurrentActor.OnTurnStart();
        EventManager<Transform>.Invoke(EventType.SetCameraFocus, CurrentActor.transform);

        if (CurrentActor is PCombatant)
        {
            gm.SwitchState(typeof(State_PlayerTurn_Move));
            gm.hudManager.combatantHudLeft.AssignTo(CurrentActor);
            gm.hudManager.combatantHudRight.AssignTo(null);
        }
        else if (CurrentActor is NPCombatant)
        {
            gm.SwitchState(typeof(State_EnemyTurn_MoveToTarget), 0.5f);
            gm.hudManager.combatantHudRight.AssignTo(CurrentActor);
        }
    }

    public void EnqueueCommand(AbilityDetails details)
    {
        _abilityQueue.Enqueue(details);
        EventManager<List<AbilityDetails>>.Invoke(EventType.OnAbilityQueueChanged, _abilityQueue.ToList());
    }

    public AbilityDetails DequeueNextAbility()
    {
        AbilityDetails result = _abilityQueue.Dequeue();
        EventManager<List<AbilityDetails>>.Invoke(EventType.OnAbilityQueueChanged, _abilityQueue.ToList());
        
        return result;
    }

    public void ClearAbilityQueue()
    {
        _abilityQueue.Clear();
        EventManager<List<AbilityDetails>>.Invoke(EventType.OnAbilityQueueChanged, _abilityQueue.ToList());
    }

    public bool IsAbilityQueueEmpty()
    {
        return _abilityQueue.Count == 0;
    }

    public List<AbilityDetails> GetCopyOfCommandQueue()
    {
        return new List<AbilityDetails>(_abilityQueue);
    }
    
    public void RemoveLastQueuedAbility()
    {
        _abilityQueue = new Queue<AbilityDetails>(_abilityQueue.Take(_abilityQueue.Count-1));
        EventManager<List<AbilityDetails>>.Invoke(EventType.OnAbilityQueueChanged, _abilityQueue.ToList());
    }

    public bool AreAllEnemiesDead()
    {
        foreach (Combatant combatant in combatants)
        {
            if (combatant is NPCombatant npc)
            {
                return false;
            }
        }

        return true;
    }
}