using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public float minRollDiceTime = 1.0f;
    public float maxRollDiceTime = 2.0f;

    public ArmyScriptableObject playerStartArmyScriptableObject;
    public ArmyScriptableObject enemyArmyScriptableObject;

    public enum GameState
    {
        None,
        Begining,
        Rolling,
        Rerolling,
        Handling,
        Ended
    }
    private GameState currentState = GameState.None;

    public ArmyScriptableObject playerArmyScriptableObject;

    private TargetGroupFinder targetGroupFinder;
    private ArmyComponent playerArmy;
    private ArmyComponent enemyArmy;
    private int rollingDiceCount = 0;
    private int rerollCount;
    private bool shouldStopRerollState = false;

    private void Start()
    {
        SwitchGameState(GameState.None);

        if (playerArmyScriptableObject == null)
        {
            playerArmyScriptableObject = Instantiate(playerStartArmyScriptableObject);
        }
    }

    public void RegisterArmyComponent(ArmyComponent armyComponent)
    {
        if (armyComponent.isPlayer)
        {
            playerArmy = armyComponent;
            playerArmy.InstantiateArmy(playerArmyScriptableObject);
        }
        else
        {
            enemyArmy = armyComponent;
            enemyArmy.InstantiateArmy(enemyArmyScriptableObject);
        }

        if (playerArmy != null && enemyArmy != null)
        {
            SwitchGameState(GameState.Begining);
        }
    }

    public void RegisterTargetGroupFinder(TargetGroupFinder targetGroupFinder)
    {
        this.targetGroupFinder = targetGroupFinder;
    }

    private void Update()
    {
        if (GetCurrentState() == GameState.Rolling && !HasRollingDice())
        {
            SwitchGameState(GameState.Rerolling);
        }
        else if (GetCurrentState() == GameState.Rerolling && shouldStopRerollState && !HasRollingDice())
        {
            SwitchGameState(GameState.Handling);
        }
        else if (GetCurrentState() == GameState.Handling)
        {
            Handle();

            if (playerArmy.HasLost() || enemyArmy.HasLost())
            {
                SwitchGameState(GameState.Ended);
            }
            else
            {
                SwitchGameState(GameState.Begining);
            }
        }
    }

    private void OnGUI()
    {
        if (GetCurrentState() == GameState.Begining)
        {
            if (GUI.Button(new Rect(10, 10, 300, 100), "Roll"))
            {
                SwitchGameState(GameState.Rolling);
            }
        }
        else if (GetCurrentState() == GameState.Rerolling)
        {
            GUI.Label(new Rect(10, 10, 300, 100), "Reroll: " + rerollCount.ToString());
            if (GUI.Button(new Rect(10, 120, 300, 100), "End rolling"))
            {
                shouldStopRerollState = true;
            }
        }
    }

    public void SwitchGameState(GameState newState)
    {
        switch (newState)
        {
            case GameState.None: break;
            case GameState.Begining:
                playerArmy.Replace(valueOrdered: false);
                enemyArmy.Replace(valueOrdered: false);
                List<Transform> diceTransforms = new List<Transform>(2);
                diceTransforms.Add(playerArmy.GetGeneralDie().transform);
                diceTransforms.Add(enemyArmy.GetGeneralDie().transform);
                targetGroupFinder.SetGroup(diceTransforms);
                break;
            case GameState.Handling:
                playerArmy.Replace(valueOrdered: true);
                enemyArmy.Replace(valueOrdered: true);
                break;
            case GameState.Rolling:
                rollingDiceCount = 0;
                playerArmy.RollDice();
                enemyArmy.RollDice();
                break;
            case GameState.Rerolling:
                rollingDiceCount = 0;
                rerollCount = Random.Range(1, 5); // D4
                shouldStopRerollState = false;
                break;
            case GameState.Ended:
                SimpleSceneLoaderComponent sceneLoaderComponent = FindObjectOfType<SimpleSceneLoaderComponent>();
                sceneLoaderComponent.LoadSceneAdditive();
                playerArmy = null;
                enemyArmy = null;
                break;
        }

        currentState = newState;
    }

    public GameState GetCurrentState()
    {
        return currentState;
    }

    public void AddRollingDice()
    {
        rollingDiceCount++;
    }    
    public void RemoveRollingDice()
    {
        rollingDiceCount--;
    }
    public bool HasRollingDice()
    {
        return rollingDiceCount > 0;
    }

    public void RerollDie()
    {
        if (CanRerollDie())
        {
            rerollCount--;
            if (rerollCount <= 0)
            {
                shouldStopRerollState = true;
            }
        }
    }
    public bool CanRerollDie()
    {
        return rerollCount > 0;
    }

    private void Handle()
    {
        List<ArmyComponent.DieValue> playerValues = playerArmy.GetValues();
        List<ArmyComponent.DieValue> enemyValues = enemyArmy.GetValues();

        ArmyComponent.DieValueComparer comparer = new ArmyComponent.DieValueComparer();
        playerValues.Sort(comparer);
        enemyValues.Sort(comparer);

        // Reduce
        if (playerValues.Count != enemyValues.Count)
        {
            if (playerValues.Count > enemyValues.Count)
            {
                playerValues = playerValues.Skip(playerValues.Count - enemyValues.Count).Take(enemyValues.Count).ToList();
            }
            else
            {
                enemyValues = enemyValues.Skip(enemyValues.Count - playerValues.Count).Take(playerValues.Count).ToList();
            }
        }

        int[] playerLoss = new int[5];
        int[] enemyLoss = new int[5];

        for (int i = 0; i < playerValues.Count; ++i)
        {
            ArmyComponent.DieValue playerDieValue = playerValues[i];
            ArmyComponent.DieValue enemyDieValue = enemyValues[i];

            int cmp = comparer.Compare(playerDieValue, enemyDieValue);
            if (cmp > 0)
            {
                enemyLoss[(int)enemyDieValue.tier]++;
            }
            else if (cmp < 0)
            {
                playerLoss[(int)playerDieValue.tier]++;
            }
            else
            {
                // Do not kill both as we don't want to have a game where both players loose
                /*
                playerLoss[(int)playerDieValue.tier]++;
                enemyLoss[(int)enemyDieValue.tier]++;
                */
            }
        }

        playerArmy.ExecuteLoss(playerLoss);
        enemyArmy.ExecuteLoss(enemyLoss);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Boostrap()
    {
        var gameManager = Instantiate(Resources.Load("GameManager")) as GameObject;
        if (gameManager == null)
            throw new System.Exception();
        DontDestroyOnLoad(gameManager);
    }
}
