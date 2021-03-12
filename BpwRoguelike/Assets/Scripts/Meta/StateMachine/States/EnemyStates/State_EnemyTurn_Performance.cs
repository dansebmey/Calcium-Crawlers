
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class State_EnemyTurn_Performance : State_EnemyTurn
{
    private TimedActionController.Performance _currentPerformanceThreshold;
    private TimedActionController.Performance _defPerformance = TimedActionController.Performance.Meh;
    
    private bool _freezeTime;
    private bool _firstTimePlaying;
    private bool _performanceHasEnded;

    public override void OnEnter()
    {
        base.OnEnter();

        _performanceHasEnded = false;
        _defPerformance = TimedActionController.Performance.Meh;
        
        EventManager<TimedActionController.Performance>.AddListener(EventType.OnDefencePerformanceMarked, MarkPerformance);
        EventManager<TimedActionController.Performance>.AddListener(EventType.OnEnemyAbilityEnd, FinishAction);
        
        Button[] inputButtons = Gm.inputController.AbilityInputButtons();
        Button correctButton = inputButtons[Random.Range(0, inputButtons.Length)];
        foreach (Button button in inputButtons)
        {
            Action downAction;
            if (button == correctButton)
            {
                downAction = CorrectButtonPressed;
            }
            else
            {
                downAction = WrongButtonPressed;
            }
            Gm.inputController.BindActionToKey(button, downAction, null);
        }
        Gm.combatManager.CurrentAbilityDetails.targets[0].overheadHud.inputIndicator.Show(true, Gm.inputController.GetKeyCode(correctButton));
        
        Gm.hudManager.inputInfoInterface.ShowCommands(new Dictionary<string, string>
        {
            {Gm.inputController.GetKeyCode(correctButton).ToString(), "Block / Dodge"}
        });

        // TODO: Below code causes an EventManager-related exception in the live build
        // if (Gm.combatManager.CurrentAbilityDetails.targets.Any(target => target is PCombatant))
        // {
        //     EventManager<AbilityDetails>.Invoke(EventType.OnEnemyAttackInitiated, Gm.combatManager.CurrentAbilityDetails);
        // }
        
        CurrentActor.PlayAbilityAnimation();
    }

    private void CorrectButtonPressed()
    {
        FreezeTime(false);
        if (!_performanceHasEnded)
        {
            foreach (GridObject target in Gm.combatManager.CurrentAbilityDetails.targets)
            {
                if (target is Combatant combatant)
                {
                    combatant.state = Combatant.State.Blocking;
                    combatant.overheadHud.inputIndicator.Fade(new Color(0.5f, 1, 0.5f, 0.5f));
                }
            }
            _defPerformance = _currentPerformanceThreshold;
        }
    }

    private void WrongButtonPressed()
    {
        if (!_performanceHasEnded)
        {
            foreach (GridObject target in Gm.combatManager.CurrentAbilityDetails.targets)
            {
                if (target is Combatant combatant)
                {
                    combatant.overheadHud.inputIndicator.Fade(new Color(1, 0.5f, 0.5f, 0.5f));
                }
            }
            _defPerformance = TimedActionController.Performance.Meh;
        }
    }

    private void MarkPerformance(TimedActionController.Performance defPerformance)
    {
        if (Gm.combatManager.firstTimeDefending && Gm.combatManager.firstTimeAttacking && defPerformance == TimedActionController.Performance.Perfect)
        {
            FreezeTime(true);
        }
        
        _currentPerformanceThreshold = defPerformance;
    }

    private void FinishAction(TimedActionController.Performance defPerformance)
    {
        _performanceHasEnded = true;
        
        AbilityDetails details = Gm.combatManager.DequeueNextAbility();
        details.performance = ((NPCombatant)details.performer).DetermineNPCPerformance();
        details.targets[0].overheadHud.inputIndicator.Show(false);

        foreach (GridObject target in details.targets)
        {
            details.RegisterTargetPerformance(target, _defPerformance);   
        }

        details.performer.PerformAbility(details);
        foreach (GridObject target in details.targets)
        {
            if (target is Combatant combatant)
            {
                if (combatant.state == Combatant.State.Blocking)
                {
                    combatant.state = Combatant.State.Idle;   
                }
            }
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        Time.timeScale = 1;
        
        EventManager<TimedActionController.Performance>.RemoveListener(EventType.OnDefencePerformanceMarked, MarkPerformance);
        EventManager<TimedActionController.Performance>.RemoveListener(EventType.OnEnemyAbilityEnd, FinishAction);
    }
    
    private void FreezeTime(bool freeze)
    {
        Time.timeScale = freeze ? 0 : 1;
        Gm.combatManager.firstTimeDefending = false;
    }
}