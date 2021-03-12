using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hitsplat : MonoBehaviour
{
    private Combatant _combatant;
    
    private LayeredText _layeredText;
    private Animator _animator;

    public List<Color> colours;
    
    private void Awake()
    {
        _combatant = GetComponentInParent<Combatant>();
        
        _layeredText = GetComponent<LayeredText>();
        _animator = GetComponent<Animator>();
    }

    public void Show(int damage, int colourIndex)
    {
        _layeredText.Text = damage.ToString();
        _layeredText.Size = (int)(48 * (1 + (1.5f / _combatant.maxHP) * damage));
        _layeredText.Colour = colours[colourIndex];
        
        _animator.Play("hitsplat_active");
    }

    public void Show(string text, Color colour)
    {
        _layeredText.Text = text;
        _layeredText.Size = 48;
        // _layeredText.Size = (int)(48 * (1 + (1.5f / _combatant.maxHP) * damage));
        _layeredText.Colour = colour;
        
        _animator.Play("hitsplat_active");
    }
}