using TMPro;
using UnityEngine;

public class TextRerollCountComponent : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.RegisterTextRerollCountComponent(GetComponent<TMP_Text>());
    }
}
