using UnityEngine;

[CreateAssetMenu]
public class DiceScriptableObject : SingletonScriptableObject<DiceScriptableObject>
{
    public GameObject prefabGeneral;

    public GameObject prefabTier1;
    public int priceTier1;

    public GameObject prefabTier2;
    public int priceTier2;

    public GameObject prefabTier3;
    public int priceTier3;

    public GameObject prefabTier4;
    public int priceTier4;

    public GameObject prefabTier5;
    public int priceTier5;

    public int GetPrice(DieComponent.Tier tier)
    {
        switch (tier)
        {
            case DieComponent.Tier.Tier1: return priceTier1;
            case DieComponent.Tier.Tier2: return priceTier2;
            case DieComponent.Tier.Tier3: return priceTier3;
            case DieComponent.Tier.Tier4: return priceTier4;
            case DieComponent.Tier.Tier5: return priceTier5;
        }
        return 0;
    }
}
