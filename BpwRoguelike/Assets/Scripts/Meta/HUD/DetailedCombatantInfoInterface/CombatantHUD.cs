
using UnityEngine;
using UnityEngine.UI;

public class CombatantHUD : HUD
{
    [HideInInspector] public Combatant combatant;
    
    private Image _portrait;
    private Text _nameText;
    
    private LayeredText _hpText;
    private LayeredText _energyText;
    private Slider _hpBar;
    private Slider _energyBar;

    protected override void Awake()
    {
        base.Awake();
        
        _portrait = GetComponentsInChildren<Image>()[1];
        _nameText = GetComponentsInChildren<Text>()[0];
        _hpText = GetComponentsInChildren<LayeredText>()[0];
        _energyText = GetComponentsInChildren<LayeredText>()[1];
        _hpBar = GetComponentsInChildren<Slider>()[0];
        _energyBar = GetComponentsInChildren<Slider>()[1];
    }

    public void AssignTo(Combatant cbt)
    {
        gameObject.SetActive(cbt != null);
        if (cbt == null) return;
        
        combatant = cbt;
        _portrait.sprite = cbt.Sprite;
        _nameText.text = cbt.profile.characterName;
            
        _hpBar.maxValue = cbt.maxHP;
        _energyBar.maxValue = cbt.MaxEnergy;
        
        UpdateHP();
        UpdateEnergy();
    }

    public void UpdateHP()
    {
        _hpText.Text = combatant.Hitpoints + " / " + combatant.maxHP;
        _hpBar.value = combatant.Hitpoints;
    }

    public void UpdateEnergy()
    {
        _energyText.Text = combatant.Energy + " / " + combatant.MaxEnergy;
        _energyBar.value = combatant.Energy;
    }

    public void Hide()
    {
        combatant = null;
        gameObject.SetActive(false);
    }
}