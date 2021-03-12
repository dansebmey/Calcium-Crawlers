using UnityEngine;
using UnityEngine.SceneManagement;

public class State_GameOver_Win : State
{
    private Camera _camera;

    public override void OnEnter()
    {
        Gm.hudManager.OnGameWon();
        
        // Gm.inputController.BindActionToKey(Button.Interact, null, Gm.ResetScene);
    }

    public override void OnUpdate()
    {
        
    }

    public override void OnExit()
    {
        
    }
}