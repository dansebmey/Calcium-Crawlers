using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputIndicator : MonoBehaviour
{
    private Image _image;
    [HideInInspector] public List<Sprite> performanceSprites;
    [HideInInspector] public LayeredText text;
    private Animator _animator;

    private void Awake()
    {
        _image = GetComponent<Image>();
        text = GetComponentInChildren<LayeredText>();
        _animator = GetComponent<Animator>();
    }
    
    public void Show(bool show, KeyCode keyCode = KeyCode.Alpha0)
    {
        _image.color = Color.white;
        text.Text = show ? keyCode.ToString() : "";
        _animator.Play(show ? "Flashing" : "input-indicator-invisible");
    }

    public void Fade(Color colour)
    {
        _image.color = colour;
        _animator.Play("input-indicator-fade");
    }

    public void ChangeSprite(int index)
    {
        _image.sprite = performanceSprites[index];
    }
}