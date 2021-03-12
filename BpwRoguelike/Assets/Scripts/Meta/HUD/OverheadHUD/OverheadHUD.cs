using System;
using UnityEngine;

public class OverheadHUD : HUD
{
    public LayeredText ixText;
    [HideInInspector] public Hitsplat hitsplat;
    [HideInInspector] public BlockSplat blockSplat;
    [HideInInspector] public InputIndicator inputIndicator;

    protected override void Awake()
    {
        base.Awake();
        
        hitsplat = GetComponentInChildren<Hitsplat>();
        blockSplat = GetComponentInChildren<BlockSplat>();
        
        inputIndicator = GetComponentInChildren<InputIndicator>();
        
        ShowText(false);
    }

    public void ShowText(bool show)
    {
        ixText.gameObject.SetActive(show);
    }
}