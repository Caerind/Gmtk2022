using TMPro;
using UnityEngine;

public class TextMoneyComponent : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.RegisterTextMoneyComponent(GetComponent<TMP_Text>());
    }
}
