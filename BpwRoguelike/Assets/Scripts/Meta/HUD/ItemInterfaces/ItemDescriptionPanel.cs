
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemDescriptionPanel : MonoBehaviour
{
    private Image _itemIcon;
    private LayeredText _itemNameText;
    private Text _itemDescriptionText;

    private ItemParamField _weightValueField;
    
    private ItemParamField _accuracyValueField;
    private ItemParamField _damageValueField;
    private ItemParamField _lethalityValueField;
    private ItemParamField _mobilityValueField;
    
    private ItemParamField _coverageValueField;
    private ItemParamField _sturdinessValueField;

    private Text _onInteractText;

    private void Awake()
    {
        _itemIcon = GetComponentsInChildren<Image>()[1];
        _itemDescriptionText = GetComponentInChildren<Text>();
        _itemNameText = GetComponentInChildren<LayeredText>();

        ItemParamField[] itemParamFields = GetComponentsInChildren<ItemParamField>();
        _weightValueField = itemParamFields[0];
        _accuracyValueField = itemParamFields[1];
        _damageValueField = itemParamFields[2];
        _lethalityValueField = itemParamFields[3];
        _mobilityValueField = itemParamFields[4];
        _coverageValueField = itemParamFields[5];
        _sturdinessValueField = itemParamFields[6];

        _onInteractText = GetComponentsInChildren<Text>().Last();
    }

    public void SetInteractText(string text)
    {
        _onInteractText.text = text;
    }

    public void AssignTo(Item item)
    {
        gameObject.SetActive(true);
        
        _itemIcon.sprite = item.sprite;
        _itemNameText.Text = item.itemName;
        _itemDescriptionText.text = item.description;

        _weightValueField.SetValue(item.weight, " kg");
        if (item is Weapon weapon)
        {
            _coverageValueField.Hide();
            _sturdinessValueField.Hide();

            _accuracyValueField.SetValue(weapon.accuracy);
            _damageValueField.SetValue(weapon.damage);
            _lethalityValueField.SetValue(weapon.lethality);
            _mobilityValueField.SetValue(weapon.mobility);
        }
        else if (item is ArmourItem armour)
        {
            _accuracyValueField.Hide();
            _damageValueField.Hide();
            _lethalityValueField.Hide();
            _mobilityValueField.Hide();
            
            _coverageValueField.SetValue(armour.coverage);
            _sturdinessValueField.SetValue(armour.sturdiness);
        }
        else
        {
            _accuracyValueField.Hide();
            _damageValueField.Hide();
            _lethalityValueField.Hide();
            _mobilityValueField.Hide();
            _coverageValueField.Hide();
            _sturdinessValueField.Hide();
        }
    }
}