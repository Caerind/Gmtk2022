using TMPro;
using UnityEngine;

public class DieComponent : BaseDieComponent
{
    public enum Tier
    {
        Tier1,
        Tier2,
        Tier3,
        Tier4,
        Tier5
    }

    public Tier tier = Tier.Tier1;
    public int value;

    private TrailRenderer trail;

    private Vector2 currentPos;
    private Vector2 targetPos;
    private bool movingToTarget = false;
    private float timeToMoveAcc;
    private float timeToMove;
    private bool shouldBeDestroyed = false;
    private bool playEqualitySound = false;

    private TMP_Text number;

    private void Start()
    {
        trail = GetComponent<TrailRenderer>();
        DisableTrail();

        number = GameManager.Instance.CreateNumberText();
        number.text = value.ToString();
        number.color = DiceScriptableObject.Instance.GetColor(tier);
        number.transform.position = transform.position;
    }

    private void Update()
    {
        if (movingToTarget)
        {
            timeToMoveAcc += Time.deltaTime;

            float factor = timeToMoveAcc / timeToMove;
            factor = Mathf.SmoothStep(0.0f, 1.0f, factor);

            transform.position = Vector2.Lerp(currentPos, targetPos, factor);
            number.transform.position = transform.position;

            if (timeToMoveAcc >= timeToMove)
            {
                movingToTarget = false;

                if (isPlayerDie)
                {
                    GameManager.Instance.cameraShake.Shake(timeToMove * 0.5f);

                    if (playEqualitySound)
                    {
                        AudioManager.PlaySound("AttackEquality");
                    }
                    else
                    {
                        AudioManager.PlaySound("AttackSuccess");
                    }
                }

                DisableTrail();

                if (shouldBeDestroyed)
                {
                    Destroy(gameObject);
                    Destroy(number.gameObject);

                    if (!isPlayerDie)
                    {
                        int gainAmount = DiceScriptableObject.Instance.GetPrice(tier) / GameManager.Instance.gainFactor;
                        GameManager.Instance.IncreasePlayerGain(gainAmount);
                        // TODO : Play coin sound
                        GameManager.Instance.CreateGainPopup(gainAmount);
                    }
                }

                transform.position = currentPos;
                number.transform.position = transform.position;
            }
        }
        else
        {
            UpdateBase();
        }
    }

    protected override void OnRollBegin()
    {
        value = GenerateValue();
    }

    protected override void OnRollEnd()
    {
        if (number != null)
        {
            number.text = value.ToString();
        }
    }
    protected override void OnRollContinue()
    {
        if (number != null)
        {
            number.text = GenerateValue().ToString();
        }
    }

    public void UpdateValue()
    {
        if (number != null)
        {
            number.text = value.ToString();
        }
    }

    protected override int GenerateValue()
    {
        switch (tier)
        {
            case Tier.Tier1: return Random.Range(1, 5); // D4
            case Tier.Tier2: return Random.Range(1, 7); // D6
            case Tier.Tier3: return Random.Range(1, 11); // D10
            case Tier.Tier4: return Random.Range(1, 13); // D12
            case Tier.Tier5: return Random.Range(1, 21); // D20
        }
        return 0;
    }

    public void Attack(Vector2 targetPos, float timeToReachTarget, bool shouldBeDestroyed, bool playEqualitySound)
    {
        currentPos = transform.position;
        this.targetPos = targetPos;
        movingToTarget = true;
        timeToMoveAcc = 0.0f;
        timeToMove = timeToReachTarget;
        this.shouldBeDestroyed = shouldBeDestroyed;
        this.playEqualitySound = playEqualitySound;
        EnableTrail();
    }

    public void EnableTrail()
    {
        trail.emitting = true;
    }

    public void DisableTrail()
    {
        trail.emitting = false;
    }

    public void SetPosition(Vector2 position)
    {
        currentPos = position;
        transform.position = position;
        if (number != null)
        {
            number.transform.position = position;
        }
    }
}
