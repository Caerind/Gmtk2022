using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Passive : ScriptableObject
{
    public Sprite sprite;
    public string description;
    public DieComponent.Tier generalDieTier;

    public abstract void ApplyPassive(bool player, int value, List<DieComponent> playerArmy, List<DieComponent> enemyArmy);
}

public static class Extensions
{
    public static IEnumerable<T> Randomize<T>(this List<T> source)
    {
        System.Random rnd = new System.Random();
        return source.OrderBy((item) => rnd.Next()).ToList<T>();
    }
}