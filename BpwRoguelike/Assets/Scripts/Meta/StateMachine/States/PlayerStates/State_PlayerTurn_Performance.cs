using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class State_PlayerTurn_Performance : State_PlayerTurn
{
    private TimedActionController.Performance _currentPerformanceThreshold;
    
    private bool _isButtonPressed;
    private bool _isButtonReleased;
    private float _idleCounter;

    private bool _freezeTime;
    private bool _endEarlyButtonPressed;
    private bool _correctInputButtonPressed;
    private Button _correctButton;
    private bool _performanceHasEnded;

    public override void OnEnter()
    {
        base.OnEnter();

        _currentPerformanceThreshold = TimedActionController.Performance.Meh;
        _isButtonPressed = false;
        _isButtonReleased = false;
        _idleCounter = 0;
        _endEarlyButtonPressed = false;
        _performanceHasEnded = false;

        EventManager<TimedActionController.Performance>.AddListener(EventType.OnPerformanceMarked, MarkPerformance);
        EventManager<TimedActionController.Performance>.AddListener(EventType.OnAbilityEnd, FinishAction);
        
        Button[] inputButtons = InputController.AbilityInputButtons();
        _correctButton = inputButtons[Random.Range(0, inputButtons.Length)];
        foreach (Button button in inputButtons)
        {
            Action downAction;
            if (button == _correctButton)
            {
                downAction = CorrectButtonPressed;
            }
            else
            {
                downAction = WrongButtonPressed;
            }
            InputController.BindActionToKey(button, downAction, RegisterPerformance);
        }
        CurrentActor.overheadHud.inputIndicator.Show(true, InputController.GetKeyCode(_correctButton));
        InputController.BindActionToKey(Button.Back, null, EndPerformanceEarly);
        
        Gm.hudManager.inputInfoInterface.ShowCommands(new Dictionary<string, string>
        {
            {InputController.GetKeyCode(_correctButton).ToString(), "(Hold) Execute command"},
            {"E", "(Twice) End turn early"}
        });
        
        Gm.tileManager.RemoveFlags(Tile.Flag.HighlightedForMovement, Tile.Flag.HighlightedForAttack);
        HighlightTargetTiles();
    }

    private void HighlightTargetTiles()
    {
        Gm.tileManager.RemoveFlags(Tile.Flag.SelectedForAttack);
        
        List<GridObject> targets = Gm.combatManager.CurrentAbilityDetails.targets;
        if (targets.Count > 0)
        {
            foreach (GridObject def in targets)
            {
                def.Tile.AddFlag(Tile.Flag.SelectedForAttack);
            }   
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (!_isButtonPressed)
        {
            _idleCounter += Time.deltaTime;
        }
    }

    private void CorrectButtonPressed()
    {
        if (!_isButtonPressed)
        {
            _correctInputButtonPressed = true;
            Gm.combatManager.CurrentActor.overheadHud.inputIndicator.Fade(new Color(0.5f, 1, 0.5f, 0.5f));
            StartAnimation();
        }
    }

    private void WrongButtonPressed()
    {
        if (!_isButtonPressed)
        {
            _correctInputButtonPressed = false;
            Gm.combatManager.CurrentActor.overheadHud.inputIndicator.Fade(new Color(1, 0.5f, 0.5f, 0.5f));
            StartAnimation();
        }
    }

    private void StartAnimation()
    {
        _isButtonPressed = true;
        _endEarlyButtonPressed = false;
        
        // CurrentActor.overheadHud.ShowInputIndicator(false);
        CurrentActor.PlayAbilityAnimation();
    }

    private void RegisterPerformance()
    {
        if (!_isButtonReleased && !_performanceHasEnded)
        {
            FreezeTime = false;
            _isButtonReleased = true;

            if (_correctInputButtonPressed)
            {
                Gm.combatManager.CurrentAbilityDetails.performance = _currentPerformanceThreshold;
            }
            else
            {
                Gm.combatManager.CurrentAbilityDetails.performance = TimedActionController.Performance.Meh;
            }
        }
    }

    private void EndPerformanceEarly()
    {
        if (_endEarlyButtonPressed)
        {
            Gm.combatManager.EndTurn();
            return;
        }
        _endEarlyButtonPressed = true;
    }

    public override void OnExit()
    {
        base.OnExit();
        _idleCounter = 0;
        _isButtonPressed = false;
        _isButtonReleased = false;
        
        EventManager<TimedActionController.Performance>.RemoveListener(EventType.OnPerformanceMarked, MarkPerformance);
        EventManager<TimedActionController.Performance>.RemoveListener(EventType.OnAbilityEnd, FinishAction);
    }

    private void MarkPerformance(TimedActionController.Performance performance)
    {
        if (Gm.combatManager.firstTimeAttacking && performance == TimedActionController.Performance.Perfect)
        {
            FreezeTime = true;
            Time.timeScale = 0;
        }
        
        _currentPerformanceThreshold = performance;
    }

    private bool FreezeTime
    {
        set
        {
            _freezeTime = value;
            Time.timeScale = _freezeTime ? 0 : 1;
            Gm.hudManager.inputInfoInterface.ShowCommands(new Dictionary<string, string>
            {
                {InputController.GetKeyCode(_correctButton).ToString(), "(Release) Execute command"}
            });
            
            Gm.combatManager.firstTimeAttacking = false;
        }
    }

    private void FinishAction(TimedActionController.Performance performance)
    {
        _performanceHasEnded = true;
        AbilityDetails details = Gm.combatManager.DequeueNextAbility();

        if (details.targets.Count > 0)
        {
            foreach (GridObject gridObject in details.targets)
            {
                if (gridObject is NPCombatant npc)
                {
                    float rand = Random.Range(0.0f, 1.0f);
                    npc.state = rand <= Mathf.Clamp(npc.chanceToBlock + _idleCounter * 0.6f, 0, 1) ? Combatant.State.Blocking : Combatant.State.Idle;

                    details.RegisterTargetPerformance(npc,
                        npc.state == Combatant.State.Blocking
                            ? npc.DetermineBlockPerformance()
                            : TimedActionController.Performance.Meh);
                }
            }
        }

        details.performer.PerformAbility(details);
    }
}