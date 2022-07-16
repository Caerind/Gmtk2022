using TMPro;
using UnityEngine;

public class TextEnemyNameComponent : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.RegisterTextEnemyName(GetComponent<TMP_Text>());
    }
}
