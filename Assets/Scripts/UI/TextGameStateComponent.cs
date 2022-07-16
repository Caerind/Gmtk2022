using TMPro;
using UnityEngine;

public class TextGameStateComponent : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.RegisterTextGameStateComponent(GetComponent<TMP_Text>());
    }
}
