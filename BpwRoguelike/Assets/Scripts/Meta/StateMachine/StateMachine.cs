using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private readonly Dictionary<Type, State> _stateDictionary = new Dictionary<Type,State>();
    public State CurrentState;

    public StateMachine(GameManager gm, Type startState, params State[] states)
    {
        foreach (State state in states)
        {
            state.Init(gm);
            _stateDictionary.Add(state.GetType(), state);
        }
        SwitchState(startState);
    }

    public void SwitchState(Type newStateType)
    {
        CurrentState?.OnExit();
        CurrentState = _stateDictionary[newStateType];
        CurrentState?.OnEnter();
        
        // Debug.Log("State is now ["+CurrentState?.GetType()+"]");
    }
}