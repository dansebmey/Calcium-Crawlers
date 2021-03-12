
using System;
using UnityEngine;
using UnityEngine.UI;

public class BlockSplat : MonoBehaviour
{
    private Animator _animator;
    
    private LayeredText _blockText;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _blockText = GetComponent<LayeredText>();
    }

    public void ShowShield(int blockValue)
    {
        _animator.Play("Block");
        _blockText.Text = "-" + blockValue;
    }
}