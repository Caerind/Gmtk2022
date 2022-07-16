using System.Collections.Generic;
using UnityEngine;

public abstract class Passive : ScriptableObject
{
    public abstract void ApplyPassive(List<int> playerArmy, List<int> enemyArmy);
}
