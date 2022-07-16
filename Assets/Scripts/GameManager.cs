using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public float minRollDiceTime = 1.0f;
    public float maxRollDiceTime = 2.0f;
    public float attackDieTime = 0.5f;

    public ArmyScriptableObject playerStartArmyScriptableObject;
    public ArmyScriptableObject enemyArmyScriptableObject;

    public enum GameState
    {
        None,
        Begining,
        Rolling,
        Rerolling,
        Resolving,
        Ended
    }
    private GameState currentState = GameState.None;

    [HideInInspector] public ArmyScriptableObject playerArmyScriptableObject;

    private TargetGroupFinder targetGroupFinder;
    private ArmyComponent playerArmy;
    private ArmyComponent enemyArmy;
    private int rollingDiceCount = 0;
    private int rerollCount;
    private bool shouldStopRerollState = false;
    private float timeWaitBetweenDie;

    private List<ArmyComponent.DieValue> playerValues;
    private List<ArmyComponent.DieValue> enemyValues;

    public int gainFactor = 5;
    public int winFactor = 2;
    private int playerMoney = 0;
    private bool playerWin = false;
    private int playerGain = 0;

    private TMP_Text textGameState;
    private Button buttonRoll;
    private Button buttonSkipReroll;
    private TMP_Text textRerollCount;
    private Image imageReroll;
    private TMP_Text textMoney;

    public bool GetPlayerHasWin()
    {
        return playerWin;
    }

    public int GetPlayerGain()
    {
        return playerGain;
    }

    public int GetPlayerMoney()
    {
        return playerMoney;
    }

    public void IncreasePlayerMoney(int amount)
    {
        playerMoney += amount;
    }

    public void DecreasePlayerMoney(int amount)
    {
        playerMoney -= amount;
    }

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

    public void RegisterTextGameStateComponent(TMP_Text textGameState)
    {
        this.textGameState = textGameState;
    }

    public void RegisterButtonRollComponent(Button buttonRoll)
    {
        this.buttonRoll = buttonRoll;
        this.buttonRoll.onClick.AddListener(() => SwitchGameState(GameState.Rolling));
    }

    public void RegisterButtonSkipRerollComponent(Button buttonSkipReroll)
    {
        this.buttonSkipReroll = buttonSkipReroll;
        this.buttonSkipReroll.onClick.AddListener(() => shouldStopRerollState = true);
    }

    public void RegisterTextRerollCountComponent(TMP_Text textRerollCount)
    {
        this.textRerollCount = textRerollCount;
    }

    public void RegisterImageRerollComponent(Image imageReroll)
    {
        this.imageReroll = imageReroll;
    }

    public void RegisterTextMoneyComponent(TMP_Text textMoney)
    {
        this.textMoney = textMoney;
    }

    private void Update()
    {
        UpdateUI();

        if (GetCurrentState() == GameState.Rolling && !HasRollingDice())
        {
            SwitchGameState(GameState.Rerolling);
        }
        else if (GetCurrentState() == GameState.Rerolling && shouldStopRerollState && !HasRollingDice())
        {
            SwitchGameState(GameState.Resolving);
        }
        else if (GetCurrentState() == GameState.Resolving)
        {
            Handle();
        }
    }

    private void UpdateUI()
    {
        if (textGameState != null)
        {
            textGameState.text = currentState.ToString();
        }

        if (buttonRoll != null)
        {
            buttonRoll.gameObject.SetActive(GetCurrentState() == GameState.Begining);
        }

        if (buttonSkipReroll != null)
        {
            buttonSkipReroll.gameObject.SetActive(GetCurrentState() == GameState.Rerolling);
        }

        if (textRerollCount != null)
        {
            textRerollCount.gameObject.SetActive(GetCurrentState() == GameState.Rerolling);
            textRerollCount.text = rerollCount.ToString();
        }

        if (imageReroll != null)
        {
            imageReroll.gameObject.SetActive(GetCurrentState() == GameState.Rerolling);
        }

        if (textMoney != null)
        {
            textMoney.text = playerGain.ToString();
        }
    }

    public void SwitchGameState(GameState newState)
    {
        switch (newState)
        {
            case GameState.None:
                playerGain = 0;
                break;
            case GameState.Begining:
                playerArmy.Replace(valueOrdered: false);
                enemyArmy.Replace(valueOrdered: false);
                List<Transform> diceTransforms = new List<Transform>(2);
                diceTransforms.Add(playerArmy.GetFurtherMostPoint());
                diceTransforms.Add(enemyArmy.GetFurtherMostPoint());
                targetGroupFinder.SetGroup(diceTransforms);
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
            case GameState.Resolving:
                playerArmy.Replace(valueOrdered: true);
                enemyArmy.Replace(valueOrdered: true);
                playerValues = playerArmy.GetValues();
                enemyValues = enemyArmy.GetValues();
                ArmyComponent.DieValueComparer comparer = new ArmyComponent.DieValueComparer();
                playerValues.Sort(comparer);
                enemyValues.Sort(comparer);
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
                timeWaitBetweenDie = 0.0f;
                break;
            case GameState.Ended:
                playerWin = enemyArmy.HasLost();
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
        timeWaitBetweenDie -= Time.deltaTime;
        if (timeWaitBetweenDie <= 0.0f)
        {
            timeWaitBetweenDie = attackDieTime;

            // End of the game ?
            if (playerArmy.HasLost() || enemyArmy.HasLost())
            {
                SwitchGameState(GameState.Ended);
            }
            else if (playerValues == null || playerValues.Count == 0)
            {
                SwitchGameState(GameState.Begining);
            }
            else
            {
                ArmyComponent.DieValue playerDieValue = playerValues[0];
                ArmyComponent.DieValue enemyDieValue = enemyValues[0];
                playerValues.RemoveAt(0);
                enemyValues.RemoveAt(0);

                Vector2 midPos = (playerDieValue.die.transform.position + enemyDieValue.die.transform.position) * 0.5f;

                // mdr names
                bool playerDieDie = false; 
                bool enemyDieDie = false;

                ArmyComponent.DieValueComparer comparer = new ArmyComponent.DieValueComparer();
                int cmp = comparer.Compare(playerDieValue, enemyDieValue);
                if (cmp > 0)
                {
                    enemyDieDie = true;
                    enemyArmy.RemoveDie(enemyDieValue.die);
                }
                else if (cmp < 0)
                {
                    playerDieDie = true;
                    playerArmy.RemoveDie(playerDieValue.die);
                }

                bool playEqualitySound = !playerDieDie && !enemyDieDie;
                playerDieValue.die.Attack(midPos, attackDieTime, playerDieDie, playEqualitySound);
                enemyDieValue.die.Attack(midPos, attackDieTime, enemyDieDie, false);
            }
        }
    }

    public void IncreasePlayerGain(int price)
    {
        playerGain += price;
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
