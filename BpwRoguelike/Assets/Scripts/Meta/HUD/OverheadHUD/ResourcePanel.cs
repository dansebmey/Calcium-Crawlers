using System;
using UnityEngine;
using UnityEngine.UI;

public class ResourcePanel : MonoBehaviour
{
    public Slider healthBar;
    public Slider energyBar;

    private void Awake()
    {
        healthBar = GetComponentsInChildren<Slider>(true)[0];
        energyBar = GetComponentsInChildren<Slider>(true)[1];
    }
}