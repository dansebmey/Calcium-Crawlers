using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InputInfoInterface : GmAwareObject
{
    private List<InputInfoText> _inputInfoList;

    protected override void Awake()
    {
        base.Awake();
        
        _inputInfoList = GetComponentsInChildren<InputInfoText>().ToList();
        _inputInfoList.Reverse();
    }

    protected override void Start()
    {
        base.Start();
        
        foreach (InputInfoText info in _inputInfoList)
        {
            info.Hide();
        }
    }

    public void ShowCommands(Dictionary<string, string> commands)
    {
        List<string> keys = new List<string>();
        List<string> values = new List<string>();
        foreach (KeyValuePair<string, string> kv in commands)
        {
            keys.Add(kv.Key);
            values.Add(kv.Value);
        }
        
        foreach (InputInfoText info in _inputInfoList)
        {
            info.Hide();
        }
        for (int i = 0; i < Math.Min(keys.Count, _inputInfoList.Count); i++)
        {
            _inputInfoList[i].AssignTo(keys[i], values[i]);
        }
    }

    public void Clear()
    {
        ShowCommands(new Dictionary<string, string>());
    }
}