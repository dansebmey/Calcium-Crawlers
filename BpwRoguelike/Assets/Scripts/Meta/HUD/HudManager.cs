using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HudManager : Manager
{
    [HideInInspector] public CombatantHUD combatantHudLeft, combatantHudRight;
    
    private LayeredText _roundTransitionText;
    private Animator _roundTransitionAnimator;
    public InventoryInterface inventoryInterface;
    public CommandInterface commandInterface;
    public InputInfoInterface inputInfoInterface;

    public CommandQueueInterface queueInterface;

    private CameraController _cameraController;
    public LootInterface lootInterface;
    public ItemDescriptionPanel itemDescriptionPanel;

    private GameOverOverlay _gameOverOverlay;

    protected override void Awake()
    {
        base.Awake();
        
        combatantHudLeft = GetComponentsInChildren<CombatantHUD>()[0];
        combatantHudRight = GetComponentsInChildren<CombatantHUD>()[1];
        _roundTransitionText = GetComponentInChildren<LayeredText>();
        _roundTransitionAnimator = _roundTransitionText.GetComponent<Animator>();
        inventoryInterface = GetComponentInChildren<InventoryInterface>(true);
        commandInterface = GetComponentInChildren<CommandInterface>(true);
        queueInterface = GetComponentInChildren<CommandQueueInterface>();
        lootInterface = GetComponentInChildren<LootInterface>();
        itemDescriptionPanel = GetComponentInChildren<ItemDescriptionPanel>();
        inputInfoInterface = GetComponentInChildren<InputInfoInterface>();
        
        inventoryInterface.Show(false);
        lootInterface.Show(false);
        itemDescriptionPanel.gameObject.SetActive(false);
        
        _cameraController = FindObjectOfType<CameraController>();

        _gameOverOverlay = GetComponentInChildren<GameOverOverlay>(true);
    }

    public void UpdateHP(Combatant combatant)
    {
        if (combatantHudLeft.combatant == combatant)
        {
            combatantHudLeft.UpdateHP();
        }
        else if (combatantHudRight.combatant == combatant)
        {
            combatantHudRight.UpdateHP();
        } 
    }

    public void UpdateEnergy(Combatant combatant)
    {
        if (combatantHudLeft.combatant == combatant)
        {
            combatantHudLeft.UpdateEnergy();
        }
        else if (combatantHudRight.combatant == combatant)
        {
            combatantHudRight.UpdateEnergy();
        }
    }

    public void ShakeScreen(float intensity)
    {
        _cameraController.ShakeScreen(intensity);
    }

    public void OnCombatantHasFallen(Combatant combatant)
    {
        if (combatantHudLeft.combatant == combatant)
        {
            combatantHudLeft.AssignTo(null);
        }
        else if (combatantHudRight.combatant == combatant)
        {
            combatantHudRight.AssignTo(null);
        }
    }

    public void TransitionToNextRound(int roundNo)
    {
        _roundTransitionText.Text = "Round " + roundNo;
        _roundTransitionAnimator.gameObject.SetActive(true);
        _roundTransitionAnimator.Play("NewRound");
    }

    private void HideAllHudElements()
    {
        combatantHudLeft.gameObject.SetActive(false);
        combatantHudRight.gameObject.SetActive(false);
        _roundTransitionText.gameObject.SetActive(false);
        _roundTransitionAnimator.gameObject.SetActive(false);
        inventoryInterface.gameObject.SetActive(false);
        commandInterface.gameObject.SetActive(false);
        queueInterface.gameObject.SetActive(false);
        lootInterface.gameObject.SetActive(false);
        itemDescriptionPanel.gameObject.SetActive(false);
        inputInfoInterface.gameObject.SetActive(false);
    }

    public void OnGameLost()
    {
        HideAllHudElements();
        _gameOverOverlay.Show(false);
    }

    public void OnGameWon()
    {
        HideAllHudElements();
        _gameOverOverlay.Show(true);
    }
}