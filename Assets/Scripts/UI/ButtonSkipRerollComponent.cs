using UnityEngine;
using UnityEngine.UI;

public class ButtonSkipRerollComponent : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.RegisterButtonSkipRerollComponent(GetComponent<Button>());
    }
}
