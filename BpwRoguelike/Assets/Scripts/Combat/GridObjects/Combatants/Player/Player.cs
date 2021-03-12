using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Range(1, 2)] public int playerID;
    [HideInInspector] public InputController inputController;

    private void Awake()
    {
        inputController = FindObjectOfType<InputController>();
    }
}