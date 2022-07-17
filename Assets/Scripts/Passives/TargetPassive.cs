using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Passives/TargetPassive")]
public class TargetPassive : Passive
{
    public override void ApplyPassive(bool player, int value, List<DieComponent> playerArmy, List<DieComponent> enemyArmy)
    {
        if (player)
        {
            List<DieComponent> p = new List<DieComponent>(playerArmy);
            p = p.OrderBy(x => x.value).ToList();
            p.Reverse();
            int pv = (value > playerArmy.Count) ? playerArmy.Count : value;
            for (int i = 0; i < pv; ++i)
            {
                p[i].value++;
                p[i].UpdateValue();
                p[i].Hover(GameManager.Instance.boostColor);
            }
        }
        else
        {
            List<DieComponent> e = new List<DieComponent>(enemyArmy);
            e = e.OrderBy(x => x.value).ToList();
            e.Reverse();
            int ev = (value > enemyArmy.Count) ? enemyArmy.Count : value;
            for (int i = 0; i < ev; ++i)
            {
                e[i].value++;
                e[i].UpdateValue();
                e[i].Hover(GameManager.Instance.boostColor);
            }
        }
    }
}
