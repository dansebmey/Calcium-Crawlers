using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class ItemInterface : GmAwareObject
{
    public ItemSlot itemSlotPrefab;
    [HideInInspector] public List<ItemSlot> itemSlots = new List<ItemSlot>();
    
    private RectTransform _bgRtx;
    
    public int horizontalItems = 4;
    public int verticalItems = 4;

    [SerializeField] private ItemDescriptionPanel itemDescriptionPanel;
    
    private ItemSlot _selectedItemSlot;

    public ItemSlot SelectedItemSlot
    {
        get => _selectedItemSlot;
        private set
        {
            _selectedItemSlot = value;
            itemDescriptionPanel.AssignTo(value.item);
            itemDescriptionPanel.SetInteractText("");
            // itemDescriptionPanel.SetInteractText(DetermineInteractText());

            foreach (ItemSlot itemSlot in itemSlots)
            {
                itemSlot.Outline(itemSlot == value);
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        
        _bgRtx = GetComponent<RectTransform>();
        float itemSlotSize = itemSlotPrefab.GetComponent<RectTransform>().rect.width;
        
        float bgWidth = horizontalItems * (16 + itemSlotSize) + 32;
        float bgHeight = verticalItems * (16 + itemSlotSize) + 32;
        _bgRtx.anchoredPosition = AnchoredPosition(bgWidth);
        _bgRtx.sizeDelta = new Vector2(bgWidth, bgHeight);

        for (int i = 0; i < verticalItems; i++)
        {
            for (int j = 0; j < horizontalItems; j++)
            {
                ItemSlot slot = Instantiate(itemSlotPrefab, Vector3.zero, Quaternion.identity);
                slot.transform.SetParent(transform);
                slot.transform.localPosition = new Vector3(-bgWidth / 2 + 72 + (16 + itemSlotSize) * j, bgHeight / 2 - 72 - (16 + itemSlotSize) * i);
                slot.transform.localScale = new Vector3(1, 1, 1);
                
                slot.SetIndex((i+1)*(j+1)-1);
                itemSlots.Add(slot);
            }
        }
    }

    protected abstract Vector2 AnchoredPosition(float bgWidth);

    public void ToPreviousItem()
    {
        SelectItem(-1);
    }

    public void ToNextItem()
    {
        SelectItem(+1);
    }

    public void ToLastItem()
    {
        List<ItemSlot> occupiedSlots = itemSlots.Where(slot => slot.item != null).ToList();
        SelectedItemSlot = occupiedSlots.Last();
    }

    private void SelectItem(int indexDelta)
    {
        List<ItemSlot> occupiedSlots = itemSlots.Where(slot => slot.item != null).ToList();
        if (occupiedSlots.Count == 0) return;
        
        SelectedItemSlot = occupiedSlots[(occupiedSlots.Count + occupiedSlots.IndexOf(SelectedItemSlot) + indexDelta) % occupiedSlots.Count];
    }

    public void ToFirstItem()
    {
        List<ItemSlot> occupiedSlots = itemSlots.Where(slot => slot.item != null).ToList();
        if (occupiedSlots.Count > 0)
        {
            SelectedItemSlot = occupiedSlots[0];
        }
    }

    protected abstract string DetermineInteractText();

    public void Show(bool show)
    {
        gameObject.SetActive(show);
        if (show)
        {
            foreach (ItemSlot slot in itemSlots)
            {
                slot.AssignTo(null, null);
            }
            
            List<Item> items = GetItems();
            for (int i = 0; i < items.Count; i++)
            {
                itemSlots[i].AssignTo(items[i], GameManager.Instance.combatManager.CurrentActor);
            }
        }
    }

    protected abstract List<Item> GetItems();
}