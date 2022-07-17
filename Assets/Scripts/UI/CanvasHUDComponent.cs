using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasHUDComponent : MonoBehaviour
{
    public RectTransform gainPopupTransform;

    public TMP_Text textGameState;
    public Button buttonRoll;
    public Button buttonSkipReroll;
    public TMP_Text textRerollCount;
    public Image imageReroll;
    public TMP_Text textMoney;
    public TMP_Text textEnemyName;

    private void Start()
    {
        GameManager.Instance.canvasHUD = this;
        GameManager.Instance.gainPopupTransform = gainPopupTransform;

        buttonRoll.onClick.AddListener(() => GameManager.Instance.SwitchGameState(GameManager.GameState.Rolling));
        buttonSkipReroll.onClick.AddListener(() => GameManager.Instance.ShouldStopRerollState());
    }
}
