using UnityEngine;

public class State_EnemyTurn : State
{
    public override void OnEnter()
    {
        
    }

    public override void OnUpdate()
    {
        
    }

    public override void OnExit()
    {
        Gm.inputController.Clear();
    }
}