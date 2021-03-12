using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector] public TileManager tileManager;
    [HideInInspector] public CombatManager combatManager;

    private StateMachine stateMachine;
    public void SwitchState(Type stateType, float delay = 0)
    {
         StartCoroutine(SwitchStateAfterDelay(stateType, delay));
        // stateMachine.SwitchState(stateType);
    }

    private IEnumerator SwitchStateAfterDelay(Type stateType, float delay)
    {
        yield return new WaitForSeconds(delay);
        stateMachine.SwitchState(stateType);
    }

    public HudManager hudManager;
    public InputController inputController;
    [Range(0.1f, 1)] public float slowMotionRatio = 0.25f;
    [HideInInspector] public float delayBetweenNPCStateSwitches;
    public State CurrentState => stateMachine?.CurrentState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        
        tileManager = FindObjectOfType<TileManager>();
        combatManager = GetComponentInChildren<CombatManager>();
        inputController = FindObjectOfType<InputController>();
        hudManager = FindObjectOfType<HudManager>();
        
        InitStateMachine();
    }

    private void InitStateMachine()
    {
        stateMachine = new StateMachine(this, typeof(State_RoundStart), new State[]
        {
            new State_RoundStart(),
            
            new State_PlayerTurn_Move(),
            new State_PlayerTurn_PickAbility(),
            new State_PlayerTurn_PickTarget(),
            new State_PlayerTurn_Performance(),
            new State_PlayerTurn_Loot(),
            new State_PlayerTurn_Inventory(),
            
            new State_EnemyTurn_MoveToTarget(),
            new State_EnemyTurn_Performance(),
            
            new State_GameOver_Loss(),
            new State_GameOver_Win(),
        });
    }

    private void Update()
    {
        CurrentState?.OnUpdate();
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}