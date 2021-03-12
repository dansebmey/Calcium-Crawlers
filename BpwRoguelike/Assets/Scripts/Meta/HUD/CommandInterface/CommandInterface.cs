using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CommandInterface : GmAwareObject
{
    private CommandListView _commandList;

    private List<CommandSlot> _commandSlots = new List<CommandSlot>();

    private int _selectedSlotIndex;
    private CommandSlot _selectedCommandSlot;
    private CommandSlot SelectedSlot
    {
        get => _selectedCommandSlot;
        set
        {
            _selectedCommandSlot = value;
            _commandSlots[_commandSlots.IndexOf(_selectedCommandSlot)].AssignTo(_selectedCommandSlot.command);

            foreach (CommandSlot slot in _commandSlots)
            {
                slot.Outline(slot == _selectedCommandSlot);   
            }
            
            UpdateInterface();
        }
    }

    public Command SelectedCommand => _selectedCommandSlot.command;

    public CommandSlot commandSlotPrefab;
    private RectTransform _rtx;

    [Header("Components")]
    public Image abilityIcon;
    public LayeredText abilityName;
    public Text abilityDescription;

    public Image energyCostIcon;
    public Text energyCostText;
    
    public GameObject abilityTagPanel;
    public Text abilityTagText;

    public Text abilityCannotBeQueuedText;
    
    private Combatant _combatant;

    public List<Command> defaultCommands;

    protected override void Awake()
    {
        base.Awake();
        
        _commandList = GetComponentInChildren<CommandListView>(true);
        _rtx = GetComponent<RectTransform>();
        gameObject.SetActive(false);

        InitCommandSlots();
    }

    private void InitCommandSlots()
    {
        _commandSlots = new List<CommandSlot>();
        for (int i = 0; i < 9; i++)
        {
            CommandSlot slot = Instantiate(commandSlotPrefab, Vector3.zero, Quaternion.identity);
            slot.transform.SetParent(_commandList.transform);
            slot.transform.localPosition = new Vector3(-(_rtx.rect.width / 2) - -64 + 96 * i, 0);
            slot.transform.localScale = new Vector3(1, 1, 1);

            _commandSlots.Add(slot);
        }
    }

    public void ShowCompatibleCommands(CombatManager cm, Combatant combatant)
    {
        _combatant = combatant;
        gameObject.SetActive(true);

        List<AbilityCommand> compatibleAbilities = new List<AbilityCommand>();
        foreach (AbilityCommand ability in combatant.GetCurrentlyPerformableAbilities())
        {
            foreach (GridObject potentialTarget in cm.gridObjects)
            {
                if (potentialTarget != combatant)
                {
                    if (!compatibleAbilities.Contains(ability) && ability.CanReach(combatant.point, potentialTarget.point, combatant.loadout.weapon))
                    {
                        compatibleAbilities.Add(ability);
                    }   
                }
            }
        }
        compatibleAbilities.Sort((a1, a2) => a1.baseEnergyCost.CompareTo(a2.baseEnergyCost));

        foreach (CommandSlot slot in _commandSlots)
        {
            slot.gameObject.SetActive(false);   
        }
        for (int i = 0; i < Mathf.Min(_commandSlots.Count - defaultCommands.Count, compatibleAbilities.Count); i++)
        {
            _commandSlots[i].AssignTo(compatibleAbilities[i]);
            _commandSlots[i].Enable(_commandSlots[i].command.CanBeQueued(combatant, GameManager.Instance.combatManager.GetCopyOfCommandQueue()));
        }

        int defaultCmdIndex = 0;
        for (int i = _commandSlots.Count-1; i >= compatibleAbilities.Count; i--)
        {
            _commandSlots[i].AssignTo(defaultCommands[defaultCmdIndex]);
            _commandSlots[i].Enable(defaultCommands[defaultCmdIndex].CanBeQueued(combatant, GameManager.Instance.combatManager.GetCopyOfCommandQueue()));
            
            defaultCmdIndex++;
            if (defaultCmdIndex == defaultCommands.Count)
            {
                break;
            }
        }

        if (GameManager.Instance.combatManager.GetCopyOfCommandQueue().Count == 0)
        {
            ToFirstCommand();   
        }
    }
    
    public void ToPreviousCommand()
    {
        SelectCommandSlot(-1);
    }

    public void ToNextCommand()
    {
        SelectCommandSlot(+1);
    }

    private void SelectCommandSlot(int indexDelta)
    {
        List<CommandSlot> occupiedSlots = _commandSlots.Where(slot => slot.gameObject.activeSelf).ToList();
        SelectedSlot = occupiedSlots[(occupiedSlots.Count + occupiedSlots.IndexOf(SelectedSlot) + indexDelta) % occupiedSlots.Count];
    }

    public void UpdateInterface()
    {
        Command command = SelectedSlot.command;
        
        abilityIcon.sprite = command.sprite;
        
        abilityName.Text = command.commandName;
        abilityDescription.text = command.description;
        energyCostText.text = command.GetNetEnergyCost(_combatant).ToString();

        if (command is AbilityCommand ability)
        {
            energyCostIcon.gameObject.SetActive(true);
            energyCostText.color = ability.ActorHasEnoughEnergy(_combatant) ? Color.white : new Color(0.76f, 0.322f, 0.322f);
        }
        else
        {
            energyCostIcon.gameObject.SetActive(false);
            energyCostText.text = "";
        }
        
        abilityCannotBeQueuedText.text = command.cannotQueueReason;

        abilityTagPanel.SetActive(command.tags.Count > 0);
        if (command.tags.Count > 0)
        {
            abilityTagText.text = command.tags[0].ToString();
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ExecuteCommand()
    {
        _selectedCommandSlot.command.OnSelected(gm);
    }

    public void ToFirstCommand()
    {
        List<CommandSlot> occupiedSlots = _commandSlots.Where(slot => slot.gameObject.activeSelf).ToList();
        SelectedSlot = occupiedSlots[0];
    }

    public void ToLastCommand()
    {
        List<CommandSlot> occupiedSlots = _commandSlots.Where(slot => slot.gameObject.activeSelf).ToList();
        SelectedSlot = occupiedSlots[occupiedSlots.Count-1];
    }
}