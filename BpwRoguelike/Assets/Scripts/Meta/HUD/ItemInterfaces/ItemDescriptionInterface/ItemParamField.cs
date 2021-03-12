using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class ItemParamField : MonoBehaviour
{
    private Text _keyText;
    private Text _valueText;

    public void SetValue(float value, string suffix = "")
    {
        gameObject.SetActive(true);
        
        _valueText.text = (value > 0 ? "+" + value : value.ToString(CultureInfo.InvariantCulture)) + suffix;
        if (Math.Abs(value) < 0.01f || suffix != "")
        {
            _valueText.color = new Color(1, 0.77f, 0, 1);
        }
        else if (value < 0)
        {
            _valueText.color = new Color(1, 0.24f, 0, 1);
        }
        else if (value > 0)
        {
            _valueText.color = new Color(0.19f, 1, 0, 1);
        }
    }

    private void Awake()
    {
        _keyText = GetComponentsInChildren<Text>()[0];
        _valueText = GetComponentsInChildren<Text>()[1];
    }

    public void AssignTo(string key, string value)
    {
        _keyText.text = key;
        _valueText.text = value;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}