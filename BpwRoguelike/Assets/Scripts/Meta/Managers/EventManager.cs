using System;
using System.Collections.Generic;
using UnityEngine;

public enum EventType
{
    OnBattleStart = 0,
    OnPlayerDamageTaken = 1,
    OnAbilityQueueChanged,
    OnAbilityEnd,
    OnPerformanceMarked,
    OnAbilityCommandSelected,
    OnNewInputAdded,
    RegisterGridObject,
    OnAbilityInputButtonPressed,
    SetCameraFocus,
    OnDefencePerformanceMarked,
    OnEnemyAbilityEnd,
    OnInputCleared,
    OnEnemyAttackInitiated
}

public static class EventManager<T>
{
    private static readonly Dictionary<EventType, Action<T>> EventDictionary = new Dictionary<EventType, Action<T>>();

    public static void AddListener(EventType type, Action<T> action)
    {
        if (!EventDictionary.ContainsKey(type))
        {
            EventDictionary.Add(type, null);
        }

        // If an Exception has an EventManager<>.Invoke() call at its root, chances are a delegate
        // has been subscribed to the EventType twice, e.g. was not removed for every time it was added
        EventDictionary[type] += action;
    }

    public static void RemoveListener(EventType type, Action<T> action)
    {
        if (EventDictionary.ContainsKey(type) && EventDictionary[type] != null)
        {
            // ReSharper disable once DelegateSubtraction
            EventDictionary[type] -= action;
        }
    }

    public static void Invoke(EventType type, T arg1)
    {
        if (EventDictionary.ContainsKey(type))
        {
            EventDictionary[type]?.Invoke(arg1);
        }
        else
        {
            Debug.LogWarning("No listener registered for event ["+type+"]");
        }
    }
}