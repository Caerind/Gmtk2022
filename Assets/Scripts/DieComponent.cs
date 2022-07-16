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

    protected override void OnRollBegin()
    {
        value = GenerateValue();
    }

    private int GenerateValue()
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

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        UnityEditor.Handles.Label(transform.position, value.ToString());
    }
#endif // UNITY_EDITOR
}
