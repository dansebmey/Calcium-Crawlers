using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Button
{
    Interact, ShowInventory, MoveLeft, MoveRight, MoveUp, MoveDown, Back, EndTurn,
    Confirm, AbilityInput1, AbilityInput2, AbilityInput3, AbilityInput4,
    Delete
}

public class InputController : MonoBehaviour
{
    private Dictionary<Button, KeyCode> _buttonMapping;
    private Dictionary<Button, InputAction> _actionDictionary;

    public KeyCode GetKeyCode(Button button)
    {
        return _buttonMapping[button];
    }

    private void Awake()
    {
        InitDictionary();
    }

    private void InitDictionary()
    {
        _actionDictionary = new Dictionary<Button, InputAction>();
        _buttonMapping = new Dictionary<Button, KeyCode>
        {
            {Button.MoveLeft, KeyCode.A},
            {Button.MoveRight, KeyCode.D},
            {Button.MoveUp, KeyCode.W},
            {Button.MoveDown, KeyCode.S},
            {Button.Interact, KeyCode.Space},
            {Button.ShowInventory, KeyCode.Tab},
            {Button.Confirm, KeyCode.Return},
            {Button.Back, KeyCode.E},
            {Button.EndTurn, KeyCode.Alpha8},
            {Button.Delete, KeyCode.Delete},
            {Button.AbilityInput1, KeyCode.U},
            {Button.AbilityInput2, KeyCode.I},
            {Button.AbilityInput3, KeyCode.O},
            {Button.AbilityInput4, KeyCode.P},
        };
    }

    private void Update()
    {
        // for (int i = _actionDictionary.Count; i--)
        
        foreach (KeyValuePair<Button, InputAction> action in _actionDictionary)
        {
            if (Input.GetKeyUp(_buttonMapping[action.Key]))
            {
                action.Value.actionUp?.Invoke();
                break;
            }
            if (Input.GetKeyDown(_buttonMapping[action.Key]))
            {
                action.Value.actionDown?.Invoke();
                break;
            }
        }
    }

    public void BindActionToKey(Button button, Action downAction = null, Action upAction = null)
    {
        if (!_actionDictionary.ContainsKey(button))
        {
            _actionDictionary.Add(button, new InputAction());
        }
        
        _actionDictionary[button].actionDown += downAction;
        _actionDictionary[button].actionUp += upAction;
    }

    public void UnbindActionToKey(Button button, Action downAction = null, Action upAction = null)
    {
        if (!_actionDictionary.ContainsKey(button))
        {
            return;
        }
        if (_actionDictionary[button].actionDown != null)
        {
            _actionDictionary[button].actionDown -= downAction;
        }
        if (_actionDictionary[button].actionUp != null)
        {
            _actionDictionary[button].actionUp -= upAction;
        }
    }

    public void Clear()
    {
        foreach (KeyValuePair<Button, InputAction> action in _actionDictionary)
        {
            _actionDictionary[action.Key].actionDown = null;
            _actionDictionary[action.Key].actionUp = null;
        }
        
        // EventManager<bool>.Invoke(EventType.OnInputCleared, true);
    }

    public Button[] AbilityInputButtons()
    {
        return new[] {Button.AbilityInput1, Button.AbilityInput2, Button.AbilityInput3, Button.AbilityInput4 };
    }

    public void PassInteractionsToInterface()
    {
        // EventManager<Dictionary<Button, InputAction>>.Invoke(EventType.OnNewInputAdded, _actionDictionary);
    }
}