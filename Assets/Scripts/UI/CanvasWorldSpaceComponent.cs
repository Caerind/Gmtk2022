using UnityEngine;
using UnityEngine.UI;

public class CanvasWorldSpaceComponent : MonoBehaviour
{
    public Image imagePassivePlayer;
    public Image imagePassiveEnemy;

    private void Awake()
    {
        GameManager.Instance.canvasWorldSpace = this;
    }
}
