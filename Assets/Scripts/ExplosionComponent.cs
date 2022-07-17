using UnityEngine;

public class ExplosionComponent : MonoBehaviour
{
    public float explosTimer = 1.0f;

    private void Start()
    {
        AudioManager.PlaySound("Explosion");
    }

    private void Update()
    {
        explosTimer -= Time.deltaTime;
        if (explosTimer < 0.0f)
        {
            Destroy(gameObject);
        }
    }
}
