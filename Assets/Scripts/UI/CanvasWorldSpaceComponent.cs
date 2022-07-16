using UnityEngine;

public class CanvasWorldSpaceComponent : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Instance.worldSpaceCanvas = GetComponent<Canvas>();
    }
}
