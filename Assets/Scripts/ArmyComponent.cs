using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArmyComponent : MonoBehaviour
{
    public Color color;
    public bool isPlayer;

    private List<DieComponent> dice;
    private GeneralDieComponent generalDie;

    private void Start()
    {
        GameManager.Instance.RegisterArmyComponent(this);
    }

    public bool HasLost()
    {
        return dice.Count <= 0;
    }

    public int GetDiceCount()
    {
        return dice.Count;
    }

    public void InstantiateArmy(ArmyScriptableObject armyScriptableObject)
    {
        DiceScriptableObject diceScriptableObject = DiceScriptableObject.Instance;

        GameObject generalDieObject = Instantiate(diceScriptableObject.prefabGeneral);
        generalDieObject.GetComponent<SpriteRenderer>().color = color;
        generalDieObject.transform.parent = transform;
        generalDie = generalDieObject.GetComponent<GeneralDieComponent>();
        generalDie.SetPassives(armyScriptableObject.passive1, armyScriptableObject.passive2, armyScriptableObject.passive3);
        generalDie.isPlayerDie = isPlayer;

        dice = new List<DieComponent>(armyScriptableObject.GetTotalDiceCount());

        for (int i = 0; i < armyScriptableObject.nbTier5; ++i)
        {
            GameObject die = Instantiate(diceScriptableObject.prefabTier5);
            die.GetComponent<SpriteRenderer>().color = color;
            die.transform.parent = transform;
            DieComponent dieComponent = die.GetComponent<DieComponent>();
            dieComponent.isPlayerDie = isPlayer;
            dice.Add(dieComponent);
        }
        for (int i = 0; i < armyScriptableObject.nbTier4; ++i)
        {
            GameObject die = Instantiate(diceScriptableObject.prefabTier4);
            die.GetComponent<SpriteRenderer>().color = color;
            die.transform.parent = transform;
            DieComponent dieComponent = die.GetComponent<DieComponent>();
            dieComponent.isPlayerDie = isPlayer;
            dice.Add(dieComponent);
        }
        for (int i = 0; i < armyScriptableObject.nbTier3; ++i)
        {
            GameObject die = Instantiate(diceScriptableObject.prefabTier3);
            die.GetComponent<SpriteRenderer>().color = color;
            die.transform.parent = transform;
            DieComponent dieComponent = die.GetComponent<DieComponent>();
            dieComponent.isPlayerDie = isPlayer;
            dice.Add(dieComponent);
        }
        for (int i = 0; i < armyScriptableObject.nbTier2; ++i)
        {
            GameObject die = Instantiate(diceScriptableObject.prefabTier2);
            die.GetComponent<SpriteRenderer>().color = color;
            die.transform.parent = transform;
            DieComponent dieComponent = die.GetComponent<DieComponent>();
            dieComponent.isPlayerDie = isPlayer;
            dice.Add(dieComponent);
        }
        for (int i = 0; i < armyScriptableObject.nbTier1; ++i)
        {
            GameObject die = Instantiate(diceScriptableObject.prefabTier1);
            die.GetComponent<SpriteRenderer>().color = color;
            die.transform.parent = transform;
            DieComponent dieComponent = die.GetComponent<DieComponent>();
            dieComponent.isPlayerDie = isPlayer;
            dice.Add(dieComponent);
        }
    }

    public void Replace(bool valueOrdered = false)
    {
        if (valueOrdered)
        {
            SortByValue();
        }
        else
        {
            SortByTier();
        }

        const float rowSize = 2f;
        const float colSize = 2f;

        // Find how to compute this
        // -> Based on screen size
        // -> Based on army size
        int dicePerRow = 7;

        float center = (dicePerRow % 2 == 0) ? (dicePerRow - 1) / 2 + 0.5f : (dicePerRow - 1) / 2;
        int rowCount = dice.Count / dicePerRow;

        for (int i = dice.Count - 1; i >= 0; i--)
        {
            // DEBUG
            dice[i].value = i;

            int row = i / dicePerRow;
            int col = i % dicePerRow;

            if (isPlayer)
            {
                col = dicePerRow - col - 1;
            }

            dice[i].transform.position = new Vector2((col - center) * colSize, (rowCount - row) * rowSize * (isPlayer ? 1 : -1));
        }

        Vector2 armyPosition = transform.position;
        generalDie.transform.position = armyPosition;
    }

    private void SortByValue()
    {
        dice = dice.OrderBy(die => die.value).ThenBy(die => die.tier).ToList();
    }

    private void SortByTier()
    {
        dice.Sort((firstObj, secondObj) =>
        {
            return firstObj.tier.CompareTo(secondObj.tier);
        });
    }

    public GeneralDieComponent GetGeneralDie()
    {
        return generalDie;
    }

    public void RollDice()
    {
        generalDie.Roll();
        foreach (DieComponent die in dice)
        {
            die.Roll();
        }
    }

    public class DieValue
    {
        public DieComponent.Tier tier;
        public int value;
    }
    public class DieValueComparer : IComparer<DieValue>
    {
        public int Compare(DieValue x, DieValue y)
        {
            return x.value.CompareTo(y.value);
        }
    }

    public List<DieValue> GetValues()
    {
        List<DieValue> values = new List<DieValue>(dice.Count);
        foreach (DieComponent die in dice)
        {
            DieValue dieValue = new DieValue();
            dieValue.tier = die.tier;
            dieValue.value = die.value;
            values.Add(dieValue);
        }
        return values;
    }

    public void ExecuteLoss(int[] loss)
    {
        for (int tierIndex = 0; tierIndex < loss.Length; ++tierIndex)
        {
            DieComponent.Tier tier = (DieComponent.Tier)tierIndex;
            for (int j = 0; j < loss[tierIndex]; ++j)
            {
                for (int i = 0; i < dice.Count; ++i)
                {
                    if (dice[i].tier == tier)
                    {
                        Destroy(dice[i].gameObject);
                        dice.RemoveAt(i);
                        break;
                    }
                }
            }
        }
    }
}
