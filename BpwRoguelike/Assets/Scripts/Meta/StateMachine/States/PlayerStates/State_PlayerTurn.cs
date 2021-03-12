public abstract class State_PlayerTurn : State
{
    protected InputController InputController => Gm.inputController;

    public override void OnEnter()
    {
        
    }
    
    public override void OnUpdate()
    {
        
    }

    public override void OnExit()
    {
        InputController.Clear();
    }
}