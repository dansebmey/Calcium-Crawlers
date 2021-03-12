using System.Collections.Generic;

public class AbilityDetails
{
    public AbilityCommand command;
    
    public Combatant performer;
    public List<GridObject> targets;
    
    public TimedActionController.Performance performance = TimedActionController.Performance.Meh;
    private readonly Dictionary<GridObject, TimedActionController.Performance> _targetPerformances = new Dictionary<GridObject, TimedActionController.Performance>();

    public AbilityDetails(AbilityCommand command, Combatant performer)
    {
        this.command = command;
        this.performer = performer;
        
        targets = new List<GridObject>();
        if (command.scope == AbilityCommand.Scope.Self)
        {
            targets.Add(performer);
        }
    }
    
    public AbilityDetails(AbilityCommand command, Combatant performer, List<GridObject> targets)
    {
        this.command = command;
        this.performer = performer;
        this.targets = targets;
    }

    public void RegisterTargetPerformance(GridObject def, TimedActionController.Performance targetPerformance)
    {
        _targetPerformances[def] = targetPerformance;
    }

    public TimedActionController.Performance GetDefPerformance(Combatant def)
    {
        return _targetPerformances[def];
    }

    public void OnPerform()
    {
        if (command.dealsDamage)
        {
            performer.DealDamage(this);   
        }

        if (command.utilityEffects != null && command.utilityEffects.Count > 0)
        {
            foreach (UtilityEffect effect in command.utilityEffects)
            {
                effect.OnApply(this);
            }
        }

        _targetPerformances.Clear();
    }
}