using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives/RandomPassive")]
public class RandomPassive : Passive
{
    public override void ApplyPassive(bool player, int value, List<DieComponent> playerArmy, List<DieComponent> enemyArmy)
    {
        List<DieComponent> p = new List<DieComponent>(playerArmy);
        List<DieComponent> e = new List<DieComponent>(enemyArmy);

        p.Randomize();
        e.Randomize();

        int pv = (value > playerArmy.Count) ? playerArmy.Count : value;
        for (int i = 0; i < pv; ++i)
            p[i].Roll();

        int ev = (value > enemyArmy.Count) ? enemyArmy.Count : value;
        for (int i = 0; i < ev; ++i)
            e[i].Roll();
    }
}
