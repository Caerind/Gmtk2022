using UnityEngine;

public class GainPopupComponent : MonoBehaviour
{
    public float timer = 1.2f;

    private void Update()
    {
        if (timer >= 0.0f)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
