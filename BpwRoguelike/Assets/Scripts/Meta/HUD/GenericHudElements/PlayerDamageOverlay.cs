using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.UI;

public class PlayerDamageOverlay : MonoBehaviour
{
    private Image _image;
    private float _alpha;

    private void Awake()
    {
        _image = GetComponent<Image>();
        EventManager<float>.AddListener(EventType.OnPlayerDamageTaken, Flash);
    }

    private void Flash(float intensity)
    {
        _alpha = 0.25f + (intensity * 0.75f);
    }

    private void Update()
    {
        if (_alpha > 0.01f)
        {
            _alpha *= 0.975f;
            _image.color = _alpha > 0.01f ?
                new Color(1, 1, 1, _alpha)
                : new Color(1, 1, 1, 0);
        }
    }
}