using UnityEngine;

public abstract class State
{
    protected GameManager Gm;
    protected Combatant CurrentActor => Gm.combatManager.CurrentActor;
    
    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();
    
    public virtual void Init(GameManager gm)
    {
        Gm = gm;
    }

}