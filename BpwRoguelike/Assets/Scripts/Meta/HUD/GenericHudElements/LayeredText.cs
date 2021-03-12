using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayeredText : MonoBehaviour
{
    private List<Text> texts;

    private void Awake()
    {
        texts = new List<Text>();
        foreach (Text text in GetComponentsInChildren<Text>(true))
        {
            texts.Add(text);
        }
    }

    public string Text
    {
        set
        {
            foreach (Text text in texts)
            {
                text.text = value;
            }
        }
    }

    public Color Colour
    {
        set => texts[1].color = value;
        // set
        // {
        //     foreach (Text text in texts)
        //     {
        //         // float textHue, textSat, textVal;
        //         // Color.RGBToHSV(text.color, out textHue, out textSat, out textVal);
        //         //
        //         // float targetHue, targetSat, targetVal;
        //         // Color.RGBToHSV(value, out targetHue, out targetSat, out targetVal);
        //         //
        //         // text.color = new Color();
        //         // text.color = value;
        //         text.color = value;
        //     }   
        // }
    }

    public int Size
    {
        set
        {
            foreach (Text text in texts)
            {
                text.fontSize = value;
            }
        }
    }
}