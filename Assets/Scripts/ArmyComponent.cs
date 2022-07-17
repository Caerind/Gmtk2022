using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArmyComponent : MonoBehaviour
{
    public Color color;
    public bool isPlayer;

    public Passive passive;

    [HideInInspector] public List<DieComponent> dice;
    [HideInInspector] public DieComponent generalDie;
    private Transform furtherMostPoint;

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

    public void InstantiateArmy(ArmyScriptableObject armyScriptableObject, Passive passive)
    {
        this.passive = passive;

        DiceScriptableObject diceScriptableObject = DiceScriptableObject.Instance;

        GameObject furtherMostGameObject = new GameObject();
        furtherMostGameObject.transform.SetParent(transform);
        furtherMostPoint = furtherMostGameObject.transform;

        if (passive != null)
        {
            GameObject generalDieObject = Instantiate(diceScriptableObject.GetPrefab(passive.generalDieTier));
            generalDieObject.transform.localScale = Vector2.one * 2.0f;
            generalDieObject.transform.Find("Square").GetComponent<SpriteRenderer>().color = color;
            generalDieObject.transform.SetParent(transform);
            generalDie = generalDieObject.GetComponent<DieComponent>();
            generalDie.isPlayerDie = isPlayer;
        }
        else
        {
            generalDie = null;
        }

        dice = new List<DieComponent>(armyScriptableObject.GetTotalDiceCount());

        for (int i = 0; i < armyScriptableObject.nbTier5; ++i)
        {
            GameObject die = Instantiate(diceScriptableObject.GetPrefab(DieComponent.Tier.Tier5));
            die.transform.Find("Square").GetComponent<SpriteRenderer>().color = color;
            die.transform.SetParent(transform);
            DieComponent dieComponent = die.GetComponent<DieComponent>();
            dieComponent.isPlayerDie = isPlayer;
            dice.Add(dieComponent);
        }
        for (int i = 0; i < armyScriptableObject.nbTier4; ++i)
        {
            GameObject die = Instantiate(diceScriptableObject.GetPrefab(DieComponent.Tier.Tier4));
            die.transform.Find("Square").GetComponent<SpriteRenderer>().color = color;
            die.transform.SetParent(transform);
            DieComponent dieComponent = die.GetComponent<DieComponent>();
            dieComponent.isPlayerDie = isPlayer;
            dice.Add(dieComponent);
        }
        for (int i = 0; i < armyScriptableObject.nbTier3; ++i)
        {
            GameObject die = Instantiate(diceScriptableObject.GetPrefab(DieComponent.Tier.Tier3));
            die.transform.Find("Square").GetComponent<SpriteRenderer>().color = color;
            die.transform.SetParent(transform);
            DieComponent dieComponent = die.GetComponent<DieComponent>();
            dieComponent.isPlayerDie = isPlayer;
            dice.Add(dieComponent);
        }
        for (int i = 0; i < armyScriptableObject.nbTier2; ++i)
        {
            GameObject die = Instantiate(diceScriptableObject.GetPrefab(DieComponent.Tier.Tier2));
            die.transform.Find("Square").GetComponent<SpriteRenderer>().color = color;
            die.transform.SetParent(transform);
            DieComponent dieComponent = die.GetComponent<DieComponent>();
            dieComponent.isPlayerDie = isPlayer;
            dice.Add(dieComponent);
        }
        for (int i = 0; i < armyScriptableObject.nbTier1; ++i)
        {
            GameObject die = Instantiate(diceScriptableObject.GetPrefab(DieComponent.Tier.Tier1));
            die.transform.Find("Square").GetComponent<SpriteRenderer>().color = color;
            die.transform.SetParent(transform);
            DieComponent dieComponent = die.GetComponent<DieComponent>();
            dieComponent.isPlayerDie = isPlayer;
            dice.Add(dieComponent);
        }
    }

    public void Replace(bool valueOrdered = false)
    {
        foreach (var die in dice)
            die.UpdateValue();
        if (generalDie != null)
            generalDie.UpdateValue();

        if (valueOrdered)
        {
            SortByValue();
        }
        else
        {
            SortByTier();
        }

        const float rowSize = 2.0f;
        const float colSize = 2.0f;
        const float noManLandSize = 1.0f;

        // Find how to compute this
        // -> Based on screen size
        // -> Based on army size
        int dicePerRow = 7;

        float center = (dicePerRow % 2 == 0) ? (dicePerRow - 1) / 2 + 0.5f : (dicePerRow - 1) / 2;
        int rowCount = dice.Count / dicePerRow;

        for (int i = dice.Count - 1; i >= 0; i--)
        {
            int row = i / dicePerRow;
            int col = i % dicePerRow;

            if (isPlayer)
            {
                col = dicePerRow - col - 1;
            }

            Vector2 diePosition = new Vector2((col - center) * colSize, (rowCount - row + noManLandSize) * rowSize * (isPlayer ? -1 : 1));
            dice[i].SetPosition(diePosition);
        }

        if (generalDie != null)
        {
            generalDie.SetPosition(new Vector2(0, (rowCount + 1.5f + noManLandSize) * rowSize * (isPlayer ? -1 : 1)));
            furtherMostPoint.position = new Vector2(0, (rowCount + 3 + noManLandSize) * rowSize * (isPlayer ? -1 : 1));

            if (isPlayer)
            {
                GameManager.Instance.canvasWorldSpace.imagePassivePlayer.transform.position = generalDie.transform.position + new Vector3(4, 0, 0);
            }
            else
            {
                GameManager.Instance.canvasWorldSpace.imagePassiveEnemy.transform.position = generalDie.transform.position + new Vector3(4, 0, 0);
            }
        }
        else
        {
            furtherMostPoint.position = new Vector2(0, (rowCount + 1.5f + noManLandSize) * rowSize * (isPlayer ? -1 : 1));
        }

    }

    private void SortByValue()
    {
        dice = dice.OrderBy(die => die.value).ThenBy(die => die.tier).ToList();
        dice.Reverse();
    }

    private void SortByTier()
    {
        dice.Sort((firstObj, secondObj) =>
        {
            return firstObj.tier.CompareTo(secondObj.tier);
        });
        dice.Reverse();
    }

    public Transform GetFurtherMostPoint()
    {
        return furtherMostPoint;
    }

    public void RollDice()
    {
        if (generalDie != null)
        {
            generalDie.Roll();
        }

        foreach (DieComponent die in dice)
        {
            die.Roll();
        }
    }

    public class DieValue
    {
        public DieComponent die;
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

    public List<DieComponent> GetValues()
    {
        return new List<DieComponent>(dice);
    }

    public void RemoveDie(DieComponent die)
    {
        dice.Remove(die);
    }

    public void UnhoverAll()
    {
        if (generalDie != null)
        {
            generalDie.Unhover();
        }

        foreach (var die in dice)
            die.Unhover();
    }
}
