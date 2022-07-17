using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives/FullRerollPassive")]
public class FullRerollPassive : Passive
{
    public override void ApplyPassive(bool player, int value, List<DieComponent> playerArmy, List<DieComponent> enemyArmy)
    {
        if (player)
        {
            foreach (var die in playerArmy)
                die.Roll();
        }
        else
        {
            foreach (var die in enemyArmy)
                die.Roll();
        }
    }
}
