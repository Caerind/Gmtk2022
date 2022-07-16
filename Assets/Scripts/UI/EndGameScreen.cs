using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGameScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text textResult;
    [SerializeField] private TMP_Text textComment;
    [SerializeField] private TMP_Text textGain;
    [SerializeField] private TMP_Text textTotal;

    [SerializeField] private Button nextButton;

    private bool nextOne = false;

    private void Start()
    {
        nextButton.onClick.AddListener(() => Next());

        if (GameManager.Instance.GetPlayerHasWin())
        {
            AudioManager.PlaySound("Win");
        }
        else
        {
            AudioManager.PlaySound("Loose");
        }

        UpdateTexts();
    }

    private void UpdateTexts()
    {
        textResult.text = GameManager.Instance.GetPlayerHasWin() ? "Win" : "Loose";

        textComment.text = "Wow, that was a cool game";

        if (!nextOne)
        {
            string gainString = "Gain: " + GameManager.Instance.GetPlayerGain().ToString();
            if (GameManager.Instance.GetPlayerHasWin())
            {
                gainString += " x " + GameManager.Instance.winFactor;
            }
            textGain.text = gainString;
        }
        else
        {
            textGain.text = "Gain: 0";
        }

        textTotal.text = GameManager.Instance.GetPlayerMoney().ToString();
    }

    private void Update()
    {
        // TODO : Animate gain to money
    }

    private void Next()
    {
        if (!nextOne)
        {
            nextOne = true;
            int resultFactor = GameManager.Instance.GetPlayerHasWin() ? GameManager.Instance.winFactor : 1;
            GameManager.Instance.IncreasePlayerMoney(GameManager.Instance.GetPlayerGain() * resultFactor);

            UpdateTexts();
        }
        else
        {
            nextButton.gameObject.GetComponent<SimpleSceneLoaderComponent>().LoadScene();
        }
    }
}
