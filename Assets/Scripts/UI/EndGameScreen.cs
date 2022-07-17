using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGameScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text textResult;
    [SerializeField] private TMP_Text textComment;
    [SerializeField] private TMP_Text textGain;
    [SerializeField] private TMP_Text textTotal;

    [SerializeField] private TMP_Text textEnemyName;
    [SerializeField] private TMP_Text textEnemyDescription;

    [SerializeField] private Button nextButton;

    public int gainPerSecond = 1000;
    private int totalGain = 0;

    private enum EndGameState
    {
        DisplayStart,
        RaisingMoney,
        DisplayNextEnemyInfo
    }
    private EndGameState state = EndGameState.DisplayStart;

    private void Start()
    {
        state = EndGameState.DisplayStart;

        nextButton.onClick.AddListener(() => Next());

        if (GameManager.Instance.GetPlayerHasWin())
        {
            AudioManager.PlaySound("Win");
        }
        else
        {
            AudioManager.PlaySound("Loose");
        }

        UpdateStatus();
    }

    private void UpdateStatus()
    {
        if (state <= EndGameState.RaisingMoney)
        {
            textResult.text = GameManager.Instance.GetPlayerHasWin() ? "Win" : "Loose";
        }
        else if (GameManager.Instance.GetPlayerHasWin())
        {
            textResult.text = "Next enemy";
        }
        else
        {
            textResult.text = "Retry";
        }

        textComment.gameObject.SetActive(state <= EndGameState.RaisingMoney);
        string comment;
        if (GameManager.Instance.GetPlayerHasWin())
        {
            comment = GameManager.Instance.enemies[GameManager.Instance.currentEnemyIndex].enemyEndPhrasesLoose[Random.Range(0, GameManager.Instance.enemies[GameManager.Instance.currentEnemyIndex].enemyEndPhrasesLoose.Count)];
        }
        else
        {
            comment = GameManager.Instance.enemies[GameManager.Instance.currentEnemyIndex].enemyEndPhrasesWin[Random.Range(0, GameManager.Instance.enemies[GameManager.Instance.currentEnemyIndex].enemyEndPhrasesWin.Count)];
        }
        textComment.text = comment;

        textGain.gameObject.SetActive(state <= EndGameState.RaisingMoney);
        if (state == EndGameState.DisplayStart)
        {
            string gainString = "Gain: " + GameManager.Instance.GetPlayerGain().ToString();
            if (GameManager.Instance.GetPlayerHasWin())
            {
                gainString += " x " + GameManager.Instance.winFactor;
            }
            else
            {
                gainString += " + " + GameManager.Instance.looseBonus;
            }
            textGain.text = gainString;
        }
        else if (state == EndGameState.RaisingMoney)
        {
            textGain.text = "Gain: " + totalGain.ToString();
        }

        textTotal.gameObject.SetActive(state == EndGameState.RaisingMoney);
        textTotal.text = GameManager.Instance.GetPlayerMoney().ToString();

        textEnemyName.gameObject.SetActive(state > EndGameState.RaisingMoney);
        textEnemyName.text = "Name: " + GameManager.Instance.enemies[GameManager.Instance.currentEnemyIndex].enemyName;

        textEnemyDescription.gameObject.SetActive(state > EndGameState.RaisingMoney);
        textEnemyDescription.text = GameManager.Instance.enemies[GameManager.Instance.currentEnemyIndex].enemyDescription;

        if (state <= EndGameState.RaisingMoney)
        {
            nextButton.GetComponentInChildren<TMP_Text>().text = "Next";
        }
        else if (GameManager.Instance.GetPlayerHasWin())
        {
            nextButton.GetComponentInChildren<TMP_Text>().text = "Next enemy";
        }
        else
        {
            nextButton.GetComponentInChildren<TMP_Text>().text = "Retry";
        }
    }

    private void Update()
    {
        if (totalGain > 0)
        {
            int gainThisFrame = Mathf.FloorToInt(gainPerSecond * Time.deltaTime);

            int value = (gainThisFrame >= totalGain) ? totalGain : gainThisFrame;
            totalGain -= value;
            GameManager.Instance.IncreasePlayerMoney(value);

            UpdateStatus();
        }
    }

    private void Next()
    {
        if (state == EndGameState.DisplayStart)
        {
            int resultFactor = GameManager.Instance.GetPlayerHasWin() ? GameManager.Instance.winFactor : 1;
            totalGain = GameManager.Instance.GetPlayerGain() * resultFactor;
            if (!GameManager.Instance.GetPlayerHasWin())
            {
                totalGain += GameManager.Instance.looseBonus;
            }

            if (totalGain > 0)
            {
                state = EndGameState.RaisingMoney;
            }
            else
            {
                state = EndGameState.DisplayNextEnemyInfo;
                if (GameManager.Instance.GetPlayerHasWin())
                {
                    GameManager.Instance.NextEnemy();
                }
            }
            UpdateStatus();
        }
        else if (state == EndGameState.RaisingMoney)
        {
            if (totalGain > 0)
            {
                GameManager.Instance.IncreasePlayerMoney(totalGain);
            }

            state = EndGameState.DisplayNextEnemyInfo;
            if (GameManager.Instance.GetPlayerHasWin())
            {
                GameManager.Instance.NextEnemy();
            }
            UpdateStatus();
        }
        else if (state == EndGameState.DisplayNextEnemyInfo)
        {
            nextButton.gameObject.GetComponent<SimpleSceneLoaderComponent>().LoadScene();
        }
    }
}
