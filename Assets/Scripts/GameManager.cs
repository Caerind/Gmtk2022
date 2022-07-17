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

    public int playerStartMoney = 2000;

    public Canvas worldSpaceCanvas;
    public Canvas HUDCanvas;
    public RectTransform gainPopupTransform;
    public Vector2 gainPopupRandomDev;
    public GameObject dieNumberPrefab;
    public GameObject gainPopupPrefab;

    public List<EnemyInfoScriptableObject> enemies;
    public int currentEnemyIndex = 0;

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
    [HideInInspector] public CinemachineCameraShake cameraShake;

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
    public int looseBonus = 250;
    private int playerMoney = 0;
    private bool playerWin = false;
    private int playerGain = 0;

    private TMP_Text textGameState;
    private Button buttonRoll;
    private Button buttonSkipReroll;
    private TMP_Text textRerollCount;
    private Image imageReroll;
    private TMP_Text textMoney;
    private TMP_Text textEnemyName;

    public void NextEnemy()
    {
        if (currentEnemyIndex + 1 < enemies.Count)
        {
            currentEnemyIndex++;
            GetComponent<SpriteRenderer>().sprite = enemies[currentEnemyIndex].enemyMap;
        }
    }

    public void CreateGainPopup(int gainAmount)
    {
        Vector2 position = gainPopupTransform.position;
        position.x += Random.Range(-gainPopupRandomDev.x, gainPopupRandomDev.x);
        position.y += Random.Range(-gainPopupRandomDev.y, gainPopupRandomDev.y);
        GameObject popup = Instantiate(gainPopupPrefab, position, Quaternion.identity);
        popup.transform.SetParent(HUDCanvas.transform);
        popup.GetComponentInChildren<TMP_Text>().text = "+" + gainAmount.ToString();
    }

    public TMP_Text CreateNumberText()
    {
        GameObject dieNumber = Instantiate(dieNumberPrefab);
        dieNumber.transform.SetParent(worldSpaceCanvas.transform);
        return dieNumber.GetComponent<TMP_Text>();
    }

    public int GetPlayerDiceCount()
    {
        return playerArmyScriptableObject.GetTotalDiceCount();
    }

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
            playerArmyScriptableObject = ScriptableObject.CreateInstance<ArmyScriptableObject>();
            playerMoney = playerStartMoney;
        }
    }

    public void RegisterArmyComponent(ArmyComponent armyComponent)
    {
        if (armyComponent.isPlayer)
        {
            // Hack when starting the game without using the preparation menu in editor
#if UNITY_EDITOR
            if (playerArmyScriptableObject.GetTotalDiceCount() == 0)
            {
                playerArmyScriptableObject.nbTier1 = 15;
                playerArmyScriptableObject.nbTier2 = 2;
                playerMoney = 0;
            }
#endif // UNITY_EDITOR

            playerArmy = armyComponent;
            playerArmy.InstantiateArmy(playerArmyScriptableObject);
        }
        else
        {
            enemyArmy = armyComponent;
            enemyArmy.InstantiateArmy(enemies[currentEnemyIndex].enemyArmy);
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

    public void RegisterTextEnemyName(TMP_Text textEnemyName)
    {
        this.textEnemyName = textEnemyName;
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
            buttonSkipReroll.gameObject.SetActive(GetCurrentState() == GameState.Rerolling && rerollCount > 0);
        }

        if (textRerollCount != null)
        {
            textRerollCount.gameObject.SetActive(GetCurrentState() == GameState.Rerolling && rerollCount > 0);
            textRerollCount.text = rerollCount.ToString();
        }

        if (imageReroll != null)
        {
            imageReroll.gameObject.SetActive(GetCurrentState() == GameState.Rerolling && rerollCount > 0);
        }

        if (textMoney != null)
        {
            textMoney.text = playerGain.ToString();
        }

        if (textEnemyName != null)
        {
            textEnemyName.text = enemies[currentEnemyIndex].enemyName;
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
                playerArmy.UnhoverAll();
                enemyArmy.UnhoverAll();
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
                foreach (ArmyComponent.DieValue die in playerValues)
                    die.die.Hover(Color.yellow);
                foreach (ArmyComponent.DieValue die in enemyValues)
                    die.die.Hover(Color.yellow);
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
