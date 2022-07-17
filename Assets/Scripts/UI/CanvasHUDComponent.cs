using UnityEngine;

public class CanvasHUDComponent : MonoBehaviour
{
    public RectTransform gainPopupTransform;

    private void Start()
    {
        GameManager.Instance.HUDCanvas = GetComponent<Canvas>();
        GameManager.Instance.gainPopupTransform = gainPopupTransform;
    }
}
