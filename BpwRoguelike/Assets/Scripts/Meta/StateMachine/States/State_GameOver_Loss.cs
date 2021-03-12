using UnityEngine;

public class State_GameOver_Loss : State
{
    private Camera _camera;

    public override void OnEnter()
    {
        Gm.hudManager.OnGameLost();
        
        // Gm.inputController.BindActionToKey(Button.Interact, null, Gm.ResetScene);
    }

    public override void OnUpdate()
    {
        
    }

    public override void OnExit()
    {
        
    }
}