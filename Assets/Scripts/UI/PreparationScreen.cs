using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreparationScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text textMoney;

    [SerializeField] private Image imageT1;
    [SerializeField] private Image imageT2;
    [SerializeField] private Image imageT3;
    [SerializeField] private Image imageT4;
    [SerializeField] private Image imageT5;
    [SerializeField] private TMP_Text textT1;
    [SerializeField] private TMP_Text textT2;
    [SerializeField] private TMP_Text textT3;
    [SerializeField] private TMP_Text textT4;
    [SerializeField] private TMP_Text textT5;
    [SerializeField] private TMP_Text textPriceT1;
    [SerializeField] private TMP_Text textPriceT2;
    [SerializeField] private TMP_Text textPriceT3;
    [SerializeField] private TMP_Text textPriceT4;
    [SerializeField] private TMP_Text textPriceT5;
    [SerializeField] private Button increaseT1;
    [SerializeField] private Button increaseT2;
    [SerializeField] private Button increaseT3;
    [SerializeField] private Button increaseT4;
    [SerializeField] private Button increaseT5;
    [SerializeField] private Button decreaseT1;
    [SerializeField] private Button decreaseT2;
    [SerializeField] private Button decreaseT3;
    [SerializeField] private Button decreaseT4;
    [SerializeField] private Button decreaseT5;

    [SerializeField] private Button readyButton;

    private void Start()
    {
        increaseT1.onClick.AddListener(() => IncreaseTier(DieComponent.Tier.Tier1));
        increaseT2.onClick.AddListener(() => IncreaseTier(DieComponent.Tier.Tier2));
        increaseT3.onClick.AddListener(() => IncreaseTier(DieComponent.Tier.Tier3));
        increaseT4.onClick.AddListener(() => IncreaseTier(DieComponent.Tier.Tier4));
        increaseT5.onClick.AddListener(() => IncreaseTier(DieComponent.Tier.Tier5));

        decreaseT1.onClick.AddListener(() => DecreaseTier(DieComponent.Tier.Tier1));
        decreaseT2.onClick.AddListener(() => DecreaseTier(DieComponent.Tier.Tier2));
        decreaseT3.onClick.AddListener(() => DecreaseTier(DieComponent.Tier.Tier3));
        decreaseT4.onClick.AddListener(() => DecreaseTier(DieComponent.Tier.Tier4));
        decreaseT5.onClick.AddListener(() => DecreaseTier(DieComponent.Tier.Tier5));

        readyButton.onClick.AddListener(() => Ready());

        UpdateStatus();
    }

    private void UpdateStatus()
    {
        GameManager gm = GameManager.Instance;
        DiceScriptableObject dices = DiceScriptableObject.Instance;

        int playerMoney = gm.GetPlayerMoney();
        textMoney.text = playerMoney.ToString();

        textT1.text = gm.playerArmyScriptableObject.nbTier1.ToString();
        textT2.text = gm.playerArmyScriptableObject.nbTier2.ToString();
        textT3.text = gm.playerArmyScriptableObject.nbTier3.ToString();
        textT4.text = gm.playerArmyScriptableObject.nbTier4.ToString();
        textT5.text = gm.playerArmyScriptableObject.nbTier5.ToString();

        textPriceT1.text = dices.GetPrice(DieComponent.Tier.Tier1).ToString();
        textPriceT2.text = dices.GetPrice(DieComponent.Tier.Tier2).ToString();
        textPriceT3.text = dices.GetPrice(DieComponent.Tier.Tier3).ToString();
        textPriceT4.text = dices.GetPrice(DieComponent.Tier.Tier4).ToString();
        textPriceT5.text = dices.GetPrice(DieComponent.Tier.Tier5).ToString();

        increaseT1.interactable = playerMoney >= dices.GetPrice(DieComponent.Tier.Tier1);
        increaseT2.interactable = playerMoney >= dices.GetPrice(DieComponent.Tier.Tier2);
        increaseT3.interactable = playerMoney >= dices.GetPrice(DieComponent.Tier.Tier3);
        increaseT4.interactable = playerMoney >= dices.GetPrice(DieComponent.Tier.Tier4);
        increaseT5.interactable = playerMoney >= dices.GetPrice(DieComponent.Tier.Tier5);

        decreaseT1.interactable = gm.playerArmyScriptableObject.nbTier1 > 0;
        decreaseT2.interactable = gm.playerArmyScriptableObject.nbTier2 > 0;
        decreaseT3.interactable = gm.playerArmyScriptableObject.nbTier3 > 0;
        decreaseT4.interactable = gm.playerArmyScriptableObject.nbTier4 > 0;
        decreaseT5.interactable = gm.playerArmyScriptableObject.nbTier5 > 0;

        readyButton.interactable = gm.GetPlayerDiceCount() > 0;

        // Colors
        imageT1.color = dices.GetColor(DieComponent.Tier.Tier1);
        imageT2.color = dices.GetColor(DieComponent.Tier.Tier2);
        imageT3.color = dices.GetColor(DieComponent.Tier.Tier3);
        imageT4.color = dices.GetColor(DieComponent.Tier.Tier4);
        imageT5.color = dices.GetColor(DieComponent.Tier.Tier5);
        textT1.color = dices.GetColor(DieComponent.Tier.Tier1);
        textT2.color = dices.GetColor(DieComponent.Tier.Tier2);
        textT3.color = dices.GetColor(DieComponent.Tier.Tier3);
        textT4.color = dices.GetColor(DieComponent.Tier.Tier4);
        textT5.color = dices.GetColor(DieComponent.Tier.Tier5);
        textPriceT1.color = dices.GetColor(DieComponent.Tier.Tier1);
        textPriceT2.color = dices.GetColor(DieComponent.Tier.Tier2);
        textPriceT3.color = dices.GetColor(DieComponent.Tier.Tier3);
        textPriceT4.color = dices.GetColor(DieComponent.Tier.Tier4);
        textPriceT5.color = dices.GetColor(DieComponent.Tier.Tier5);
        increaseT1.gameObject.GetComponentInChildren<TMP_Text>().color = dices.GetColor(DieComponent.Tier.Tier1);
        increaseT2.gameObject.GetComponentInChildren<TMP_Text>().color = dices.GetColor(DieComponent.Tier.Tier2);
        increaseT3.gameObject.GetComponentInChildren<TMP_Text>().color = dices.GetColor(DieComponent.Tier.Tier3);
        increaseT4.gameObject.GetComponentInChildren<TMP_Text>().color = dices.GetColor(DieComponent.Tier.Tier4);
        increaseT5.gameObject.GetComponentInChildren<TMP_Text>().color = dices.GetColor(DieComponent.Tier.Tier5);
        decreaseT1.gameObject.GetComponentInChildren<TMP_Text>().color = dices.GetColor(DieComponent.Tier.Tier1);
        decreaseT2.gameObject.GetComponentInChildren<TMP_Text>().color = dices.GetColor(DieComponent.Tier.Tier2);
        decreaseT3.gameObject.GetComponentInChildren<TMP_Text>().color = dices.GetColor(DieComponent.Tier.Tier3);
        decreaseT4.gameObject.GetComponentInChildren<TMP_Text>().color = dices.GetColor(DieComponent.Tier.Tier4);
        decreaseT5.gameObject.GetComponentInChildren<TMP_Text>().color = dices.GetColor(DieComponent.Tier.Tier5);
    }

    private void Ready()
    {
        GameManager.Instance.SwitchGameState(GameManager.GameState.None);
        readyButton.gameObject.GetComponent<SimpleSceneLoaderComponent>().LoadScene();
    }

    private void IncreaseTier(DieComponent.Tier tier)
    {
        int tierPrice = DiceScriptableObject.Instance.GetPrice(tier);
        if (GameManager.Instance.GetPlayerMoney() >= tierPrice)
        {
            GameManager.Instance.DecreasePlayerMoney(tierPrice);

            switch (tier)
            {
                case DieComponent.Tier.Tier1: GameManager.Instance.playerArmyScriptableObject.nbTier1++; break;
                case DieComponent.Tier.Tier2: GameManager.Instance.playerArmyScriptableObject.nbTier2++; break;
                case DieComponent.Tier.Tier3: GameManager.Instance.playerArmyScriptableObject.nbTier3++; break;
                case DieComponent.Tier.Tier4: GameManager.Instance.playerArmyScriptableObject.nbTier4++; break;
                case DieComponent.Tier.Tier5: GameManager.Instance.playerArmyScriptableObject.nbTier5++; break;
            }

            UpdateStatus();
        }
    }

    private void DecreaseTier(DieComponent.Tier tier)
    {
        int count = 0;
        switch (tier)
        {
            case DieComponent.Tier.Tier1: count = GameManager.Instance.playerArmyScriptableObject.nbTier1; break;
            case DieComponent.Tier.Tier2: count = GameManager.Instance.playerArmyScriptableObject.nbTier2; break;
            case DieComponent.Tier.Tier3: count = GameManager.Instance.playerArmyScriptableObject.nbTier3; break;
            case DieComponent.Tier.Tier4: count = GameManager.Instance.playerArmyScriptableObject.nbTier4; break;
            case DieComponent.Tier.Tier5: count = GameManager.Instance.playerArmyScriptableObject.nbTier5; break;
        }

        if (count > 0)
        {
            int tierPrice = DiceScriptableObject.Instance.GetPrice(tier);

            switch (tier)
            {
                case DieComponent.Tier.Tier1: GameManager.Instance.playerArmyScriptableObject.nbTier1--; break;
                case DieComponent.Tier.Tier2: GameManager.Instance.playerArmyScriptableObject.nbTier2--; break;
                case DieComponent.Tier.Tier3: GameManager.Instance.playerArmyScriptableObject.nbTier3--; break;
                case DieComponent.Tier.Tier4: GameManager.Instance.playerArmyScriptableObject.nbTier4--; break;
                case DieComponent.Tier.Tier5: GameManager.Instance.playerArmyScriptableObject.nbTier5--; break;
            }

            GameManager.Instance.IncreasePlayerMoney(tierPrice);

            UpdateStatus();
        }
    }
}
