using UnityEngine;

[CreateAssetMenu]
public class ArmyScriptableObject : ScriptableObject
{
    public int nbTier1;
    public int nbTier2;
    public int nbTier3;
    public int nbTier4;
    public int nbTier5;

    public int GetTotalDiceCount()
    {
        return nbTier1
            + nbTier2
            + nbTier3
            + nbTier4
            + nbTier5;
    }

    public int GetTotalCost()
    {
        DiceScriptableObject dice = DiceScriptableObject.Instance;
        return nbTier1 * dice.priceTier1
            + nbTier2 * dice.priceTier2
            + nbTier3 * dice.priceTier3
            + nbTier4 * dice.priceTier4
            + nbTier5 * dice.priceTier5;
    }

    public int GetTotalGain()
    {
        GameManager[] assets = Resources.LoadAll<GameManager>("");
        if (assets != null && assets.Length == 1)
        {
            GameManager gm = assets[0];
            return (GetTotalCost() / gm.gainFactor);
        }
        return 0;
    }

    public int GetTotalGainWin()
    {
        GameManager[] assets = Resources.LoadAll<GameManager>("");
        if (assets != null && assets.Length == 1)
        {
            GameManager gm = assets[0];
            return (GetTotalCost() / gm.gainFactor) * gm.winFactor;
        }
        return 0;
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(ArmyScriptableObject))]
    internal class ArmyScriptableObjectEditor : UnityEditor.Editor
    {
        private ArmyScriptableObject army => target as ArmyScriptableObject;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Label("Total dice: " + army.GetTotalDiceCount());
            GUILayout.Label("Total price: " + army.GetTotalCost());
            GUILayout.Label("Total gain: " + army.GetTotalGain());
            GUILayout.Label("Total gain win: " + army.GetTotalGainWin());
        }
    }
#endif // UNITY_EDITOR
}
