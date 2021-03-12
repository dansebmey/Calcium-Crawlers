using System;
using UnityEngine;

public class GmAwareObject : MonoBehaviour
{
    protected GameManager gm;
    
    protected virtual void Awake()
    {
        
    }

    protected virtual void Start()
    {
        gm = GameManager.Instance;
    }
}