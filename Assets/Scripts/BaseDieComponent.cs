using UnityEngine;

public class BaseDieComponent : MonoBehaviour
{
    private float factor;
    private bool isRolling = false;
    private float timeRollingRemaining;

    public bool isPlayerDie = false;

    private Collider2D myCollider2D;

    private void Awake()
    {
        myCollider2D = GetComponent<Collider2D>();
        OnRollBegin();
    }

    public void Roll()
    {
        factor = Random.Range(0.0f, 1.0f);
        isRolling = true;
        timeRollingRemaining = Random.Range(GameManager.Instance.minRollDiceTime, GameManager.Instance.maxRollDiceTime);
        GameManager.Instance.AddRollingDice();
        OnRollBegin();
    }

    protected void UpdateBase()
    {
        if (isRolling)
        {
            Vector3 euler = transform.localEulerAngles;
            euler.z += Random.Range(1.0f, 30.0f) * ((Mathf.PerlinNoise(Time.realtimeSinceStartup + factor, Time.realtimeSinceStartup + factor) * 2.0f) - 1.0f);
            transform.localEulerAngles = euler;

            timeRollingRemaining -= Time.deltaTime;
            if (timeRollingRemaining <= 0.0f)
            {
                GameManager.Instance.RemoveRollingDice();
                OnRollEnd();
                isRolling = false;
            }
        }
        else
        {
            if (isPlayerDie && GameManager.Instance.GetCurrentState() == GameManager.GameState.Rerolling && GameManager.Instance.CanRerollDie())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    if (myCollider2D.OverlapPoint(mousePosition))
                    {
                        Roll();
                        GameManager.Instance.RerollDie();
                    }
                }
            }
        }
    }

    protected virtual void OnRollBegin()
    {
    }

    protected virtual void OnRollEnd()
    {
    }
}
