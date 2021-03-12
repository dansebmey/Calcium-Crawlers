using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [HideInInspector] public Item item;
    private int _index;
    
    private Image _image;
    private Image _outline;
    private Image _equippedOutline;

    private void Awake()
    {
        _image = GetComponentsInChildren<Image>()[1];
        _equippedOutline = GetComponentsInChildren<Image>()[2];
        _outline = GetComponentsInChildren<Image>()[3];
        Outline(false);
        
        _equippedOutline.gameObject.SetActive(false);
    }

    public void SetIndex(int i)
    {
        _index = i;
    }

    public void AssignTo(Item newItem, Combatant owner)
    {
        item = newItem;
            
        _image.sprite = newItem != null ? newItem.sprite : null;
        _image.color = newItem == null ? new Color(0, 0, 0, 0)
            : item.isQueued ? new Color(1, 1, 1, 0.33f) : Color.white;

        _equippedOutline.gameObject.SetActive(item != null && owner != null && owner.loadout.IsItemEquipped(item));
    }

    public void Outline(bool show)
    {
        _outline.gameObject.SetActive(show);
    }
}