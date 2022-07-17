using UnityEngine;

public class BaseDieComponent : MonoBehaviour
{
    private float factor;
    private bool isRolling = false;
    private float timeRollingRemaining;

    public bool isPlayerDie = false;

    private Collider2D myCollider2D;
    protected SpriteRenderer myHoverSpriteRenderer;

    private void Awake()
    {
        myCollider2D = GetComponent<Collider2D>();
        myHoverSpriteRenderer = transform.Find("Square").GetComponent<SpriteRenderer>();
        myHoverSpriteRenderer.gameObject.SetActive(false);
        OnRollBegin();
    }

    public void Hover(Color color)
    {
        myHoverSpriteRenderer.gameObject.SetActive(true);
        myHoverSpriteRenderer.color = color;
    }

    public void Unhover()
    {
        myHoverSpriteRenderer.gameObject.SetActive(false);
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
        if (myHoverSpriteRenderer.color == Color.magenta)
        {
            Unhover();
        }

        if (isRolling)
        {
            Vector3 euler = transform.localEulerAngles;
            euler.z += Random.Range(10.0f, 50.0f) * ((Mathf.PerlinNoise(Time.realtimeSinceStartup + factor, Time.realtimeSinceStartup + factor) * 2.0f) - 1.0f);
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
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (myCollider2D.OverlapPoint(mousePosition))
                {
                    Hover(Color.magenta);

                    if (Input.GetMouseButtonDown(0))
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
