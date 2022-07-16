using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreparationScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text textT1;
    [SerializeField] private TMP_Text textT2;
    [SerializeField] private TMP_Text textT3;
    [SerializeField] private TMP_Text textT4;
    [SerializeField] private TMP_Text textT5;

    [SerializeField] private Button increaseT1;
    [SerializeField] private Button increaseT2;
    [SerializeField] private Button increaseT3;
    [SerializeField] private Button increaseT4;
    [SerializeField] private Button increaseT5;

    [SerializeField] private Button readyButton;

    private void Start()
    {
        increaseT1.onClick.AddListener(() => IncreaseTier(DieComponent.Tier.Tier1));
        increaseT2.onClick.AddListener(() => IncreaseTier(DieComponent.Tier.Tier2));
        increaseT3.onClick.AddListener(() => IncreaseTier(DieComponent.Tier.Tier3));
        increaseT4.onClick.AddListener(() => IncreaseTier(DieComponent.Tier.Tier4));
        increaseT5.onClick.AddListener(() => IncreaseTier(DieComponent.Tier.Tier5));
        readyButton.onClick.AddListener(() => Ready());

        UpdateTexts();
    }

    private void UpdateTexts()
    {
        textT1.text = GameManager.Instance.playerArmyScriptableObject.nbTier1.ToString();
        textT2.text = GameManager.Instance.playerArmyScriptableObject.nbTier2.ToString();
        textT3.text = GameManager.Instance.playerArmyScriptableObject.nbTier3.ToString();
        textT4.text = GameManager.Instance.playerArmyScriptableObject.nbTier4.ToString();
        textT5.text = GameManager.Instance.playerArmyScriptableObject.nbTier5.ToString();
    }

    private void Ready()
    {
        readyButton.gameObject.GetComponent<SimpleSceneLoaderComponent>().LoadScene();
    }

    private void IncreaseTier(DieComponent.Tier tier)
    {
        if (true)
        {
            switch (tier)
            {
                case DieComponent.Tier.Tier1: GameManager.Instance.playerArmyScriptableObject.nbTier1++; break;
                case DieComponent.Tier.Tier2: GameManager.Instance.playerArmyScriptableObject.nbTier2++; break;
                case DieComponent.Tier.Tier3: GameManager.Instance.playerArmyScriptableObject.nbTier3++; break;
                case DieComponent.Tier.Tier4: GameManager.Instance.playerArmyScriptableObject.nbTier4++; break;
                case DieComponent.Tier.Tier5: GameManager.Instance.playerArmyScriptableObject.nbTier5++; break;
            }

            UpdateTexts();
        }
    }
}
