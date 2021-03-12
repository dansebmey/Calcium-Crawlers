using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class CommandSlot : MonoBehaviour
{
    [HideInInspector] public Command command;
    
    [HideInInspector] public Image image;
    [CanBeNull] private Image _outline;
    
    [HideInInspector] public bool isSlotEnabled;
    [HideInInspector] public string warningText;

    private void Awake()
    {
        image = GetComponentInChildren<Image>();
        
        if (GetComponentsInChildren<Image>().Length > 1)
        {
            _outline = GetComponentsInChildren<Image>(true)[1];   
        }
    }

    private void SetWarningText(string text)
    {
        warningText = text;
    }

    public void AssignTo(Command cmd)
    {
        gameObject.SetActive(true);
        command = cmd;
        image.sprite = cmd.sprite;
    }

    public void Outline(bool show)
    {
        if (_outline != null)
        {
            _outline.gameObject.SetActive(show);   
        }
    }

    public void Enable(bool enable)
    {
        isSlotEnabled = enable;
        image.color = enable ? Color.white : new Color(255, 255, 255, 0.25f);
    }
}