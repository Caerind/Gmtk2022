using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public float minRollDiceTime = 1.0f;
    public float maxRollDiceTime = 2.0f;
    public float attackDieTime = 0.5f;

    public GameObject explosionPrefab;

    public List<Passive> playerPossiblePassives;
    public Passive GetPassiveN(int n)
    {
        return playerPossiblePassives[n - 1];
    }

    public int playerStartMoney = 2000;

    public Color hoverColor;
    public Color selectedColor;
    public Color boostColor;

    public CanvasWorldSpaceComponent canvasWorldSpace;
    public CanvasHUDComponent canvasHUD;
    public RectTransform gainPopupTransform;
    public Vector2 gainPopupRandomDev;
    public GameObject dieNumberPrefab;
    public GameObject gainPopupPrefab;

    public List<EnemyInfoScriptableObject> enemies;
    public int currentEnemyIndex = 0;

    public int diceSounds = 5;
    public void RollDiceSound()
    {
        AudioManager.PlaySound("Dice" + Random.Range(0, diceSounds));
    }


    public Passive playerPassive;

    public enum GameState
    {
        None,
        Begining,
        Rolling,
        Rerolling,
        Passives,
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

    private List<DieComponent> playerValues;
    private List<DieComponent> enemyValues;

    public int gainFactor = 5;
    public int winFactor = 2;
    public int looseBonus = 250;
    private int playerMoney = 0;
    private bool playerWin = false;
    private int playerGain = 0;

    private float passiveTimer;
    public float passivePhaseTime = 1.0f;

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
        popup.transform.SetParent(canvasHUD.transform);
        popup.GetComponentInChildren<TMP_Text>().text = "+" + gainAmount.ToString();
    }

    public TMP_Text CreateNumberText()
    {
        GameObject dieNumber = Instantiate(dieNumberPrefab);
        dieNumber.transform.SetParent(canvasWorldSpace.transform);
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
            playerArmy.InstantiateArmy(playerArmyScriptableObject, playerPassive);
        }
        else
        {
            enemyArmy = armyComponent;
            enemyArmy.InstantiateArmy(enemies[currentEnemyIndex].enemyArmy, enemies[currentEnemyIndex].enemyPassive);
        }

        if (playerArmy != null && enemyArmy != null)
        {
            if (playerPassive != null)
            {
                canvasWorldSpace.imagePassivePlayer.gameObject.SetActive(true);
                canvasWorldSpace.imagePassivePlayer.sprite = playerPassive.sprite;
            }
            else
            {
                canvasWorldSpace.imagePassivePlayer.gameObject.SetActive(false);
            }

            if (enemies[currentEnemyIndex].enemyPassive != null)
            {
                canvasWorldSpace.imagePassiveEnemy.gameObject.SetActive(true);
                canvasWorldSpace.imagePassiveEnemy.sprite = enemies[currentEnemyIndex].enemyPassive.sprite;
                canvasWorldSpace.imagePassiveEnemy.sprite = enemies[currentEnemyIndex].enemyPassive.sprite;
            }
            else
            {
                canvasWorldSpace.imagePassiveEnemy.gameObject.SetActive(false);
            }

            SwitchGameState(GameState.Begining);
        }
    }

    public void RegisterTargetGroupFinder(TargetGroupFinder targetGroupFinder)
    {
        this.targetGroupFinder = targetGroupFinder;
    }

    public void ShouldStopRerollState()
    {
        shouldStopRerollState = true;
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
            SwitchGameState(GameState.Passives);
        }
        else if (GetCurrentState() == GameState.Passives)
        {
            passiveTimer -= Time.deltaTime;
            if (passiveTimer <= 0.0f)
            {
                SwitchGameState(GameState.Resolving);
            }
        }
        else if (GetCurrentState() == GameState.Resolving)
        {
            Handle();
        }
    }

    private void UpdateUI()
    {
        if (canvasHUD != null)
        {
            if (canvasHUD.textGameState != null)
            {
                canvasHUD.textGameState.text = currentState.ToString();
            }

            if (canvasHUD.buttonRoll != null)
            {
                canvasHUD.buttonRoll.gameObject.SetActive(GetCurrentState() == GameState.Begining);
            }

            if (canvasHUD.buttonSkipReroll != null)
            {
                canvasHUD.buttonSkipReroll.gameObject.SetActive(GetCurrentState() == GameState.Rerolling && rerollCount > 0);
            }

            if (canvasHUD.textRerollCount != null)
            {
                canvasHUD.textRerollCount.gameObject.SetActive(GetCurrentState() == GameState.Rerolling && rerollCount > 0);
                canvasHUD.textRerollCount.text = rerollCount.ToString();
            }

            if (canvasHUD.imageReroll != null)
            {
                canvasHUD.imageReroll.gameObject.SetActive(GetCurrentState() == GameState.Rerolling && rerollCount > 0);
            }

            if (canvasHUD.textMoney != null)
            {
                canvasHUD.textMoney.text = playerGain.ToString();
            }

            if (canvasHUD.textEnemyName != null)
            {
                canvasHUD.textEnemyName.text = enemies[currentEnemyIndex].enemyName;
            }
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
                rerollCount = Random.Range(1, 4); // D3
                shouldStopRerollState = false;
                break;
            case GameState.Passives:
                currentState = newState; // hack for roll timer
                passiveTimer = (playerPassive != null || enemies[currentEnemyIndex].enemyPassive != null) ? passivePhaseTime : 0.0f;
                if (playerPassive != null)
                {
                    playerPassive.ApplyPassive(true, playerArmy.generalDie.value, playerArmy.dice, enemyArmy.dice);
                }
                if (enemies[currentEnemyIndex].enemyPassive != null)
                {
                    enemies[currentEnemyIndex].enemyPassive.ApplyPassive(false, enemyArmy.generalDie.value, playerArmy.dice, enemyArmy.dice);
                }
                break;
            case GameState.Resolving:
                playerArmy.UnhoverAll();
                enemyArmy.UnhoverAll();
                playerArmy.Replace(valueOrdered: true);
                enemyArmy.Replace(valueOrdered: true);
                playerValues = playerArmy.GetValues();
                enemyValues = enemyArmy.GetValues();
                // Already sorted
                /*
                playerValues.OrderBy(x => x.value);
                enemyValues.OrderBy(x => x.value);
                */
                /*
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
                    die.die.Hover(selectedColor);
                foreach (ArmyComponent.DieValue die in enemyValues)
                    die.die.Hover(selectedColor);
                */
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
            else if (playerValues == null || playerValues.Count == 0 || enemyValues == null || enemyValues.Count == 0)
            {
                SwitchGameState(GameState.Begining);
            }
            else
            {
                DieComponent playerDie = playerValues[0];
                DieComponent enemyDie = enemyValues[0];

                playerValues.RemoveAt(0);
                enemyValues.RemoveAt(0);

                Vector2 midPos = (playerDie.transform.position + enemyDie.transform.position) * 0.5f;

                // mdr names
                bool playerDieDie = false; 
                bool enemyDieDie = false;

                DieComponent otherDead = null;

                int cmp = playerDie.value.CompareTo(enemyDie.value);
                if (cmp > 0)
                {
                    enemyDieDie = true;
                    enemyArmy.RemoveDie(enemyDie);

                    if (enemyValues != null && enemyValues.Count > 0 && playerDie.value.CompareTo((enemyDie.value + enemyValues[0].value) * 2) > 0)
                    {
                        otherDead = enemyValues[0];
                        enemyValues.RemoveAt(0);
                        enemyArmy.RemoveDie(otherDead);
                    }
                }
                else if (cmp < 0)
                {
                    playerDieDie = true;
                    playerArmy.RemoveDie(playerDie);

                    if (playerValues != null && playerValues.Count > 0 && enemyDie.value.CompareTo((playerDie.value + playerValues[0].value) * 2) > 0)
                    {
                        otherDead = playerValues[0];
                        playerValues.RemoveAt(0);
                        playerArmy.RemoveDie(otherDead);
                    }
                }

                bool playEqualitySound = !playerDieDie && !enemyDieDie;
                playerDie.Attack(midPos, attackDieTime, playerDieDie, playEqualitySound, (otherDead != null && !otherDead.isPlayerDie) ? otherDead : null);
                enemyDie.Attack(midPos, attackDieTime, enemyDieDie, false, (otherDead != null && otherDead.isPlayerDie) ? otherDead : null);
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
