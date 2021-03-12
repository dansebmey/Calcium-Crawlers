using System;
using UnityEngine;
using UnityEngine.UI;

public class InputInfoText : MonoBehaviour
{
    private Text _inputButton;
    private Text _inputDescription;

    private void Awake()
    {
        _inputButton = GetComponentsInChildren<Text>()[0];
        _inputDescription = GetComponentsInChildren<Text>()[1];
    }

    public void AssignTo(string button, string action)
    {
        _inputButton.text = "[" + button + "]";
        _inputDescription.text = action;
    }

    public void Hide()
    {
        _inputButton.text = "";
        _inputDescription.text = "";
    }

    public bool IsShown()
    {
        return _inputButton.text != "";
    }
}