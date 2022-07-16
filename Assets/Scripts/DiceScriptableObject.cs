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
}
