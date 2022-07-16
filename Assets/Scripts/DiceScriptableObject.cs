using UnityEngine;

[CreateAssetMenu]
public class DiceScriptableObject : SingletonScriptableObject<DiceScriptableObject>
{
    public GameObject prefabGeneral;

    public GameObject prefabTier1;
    public int priceTier1;
    public Color colorTier1;

    public GameObject prefabTier2;
    public int priceTier2;
    public Color colorTier2;

    public GameObject prefabTier3;
    public int priceTier3;
    public Color colorTier3;

    public GameObject prefabTier4;
    public int priceTier4;
    public Color colorTier4;

    public GameObject prefabTier5;
    public int priceTier5;
    public Color colorTier5;

    public GameObject GetPrefab(DieComponent.Tier tier)
    {
        switch (tier)
        {
            case DieComponent.Tier.Tier1: return prefabTier1;
            case DieComponent.Tier.Tier2: return prefabTier2;
            case DieComponent.Tier.Tier3: return prefabTier3;
            case DieComponent.Tier.Tier4: return prefabTier4;
            case DieComponent.Tier.Tier5: return prefabTier5;
        }
        return null;
    }

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

    public Color GetColor(DieComponent.Tier tier)
    {
        switch (tier)
        {
            case DieComponent.Tier.Tier1: return colorTier1;
            case DieComponent.Tier.Tier2: return colorTier2;
            case DieComponent.Tier.Tier3: return colorTier3;
            case DieComponent.Tier.Tier4: return colorTier4;
            case DieComponent.Tier.Tier5: return colorTier5;
        }
        return Color.black;
    }
}
