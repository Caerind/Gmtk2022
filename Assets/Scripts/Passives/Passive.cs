using System.Collections.Generic;
using UnityEngine;

public abstract class Passive : ScriptableObject
{
    public Sprite sprite;
    public string description;
    public DieComponent.Tier generalDieTier;

    public abstract void ApplyPassive(List<int> playerArmy, List<int> enemyArmy);
}
