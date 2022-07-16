using UnityEngine;
using UnityEngine.UI;

public class ImageRerollComponent : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.RegisterImageRerollComponent(GetComponent<Image>());
    }
}
