using UnityEngine;
using UnityEngine.UI;

public class ButtonRollComponent : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.RegisterButtonRollComponent(GetComponent<Button>());
    }
}
