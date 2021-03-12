using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CommandQueueInterface : MonoBehaviour
{
    private List<CommandSlot> _abilitySlots;

    private void Awake()
    {
        _abilitySlots = GetComponentsInChildren<CommandSlot>(true).ToList();
        
        EventManager<List<AbilityDetails>>.AddListener(EventType.OnAbilityQueueChanged, UpdateSlots);
        gameObject.SetActive(false);
    }

    private void UpdateSlots(List<AbilityDetails> queuedAbilities)
    {
        bool isListEmpty = queuedAbilities == null || queuedAbilities.Count == 0;
        gameObject.SetActive(!isListEmpty);
        if (isListEmpty)
        {
            return;
        }
        
        for (int i = 0; i < queuedAbilities.Count; i++)
        {
            _abilitySlots[i].gameObject.SetActive(true);
            _abilitySlots[i].AssignTo(queuedAbilities[i].command);
        }
        for (int i = queuedAbilities.Count; i < _abilitySlots.Count; i++)
        {
            _abilitySlots[i].gameObject.SetActive(false);
        }
    }

    public bool IsQueueFull()
    {
        return _abilitySlots.All(slot => slot.command != null);
    }
}