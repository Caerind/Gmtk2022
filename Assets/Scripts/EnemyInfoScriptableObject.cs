using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class EnemyInfoScriptableObject : ScriptableObject
{
    public string enemyName;
    public Passive enemyPassive;
    public Sprite enemyMap;
    public ArmyScriptableObject enemyArmy;
    public string enemyDescription;
    public List<string> enemyEndPhrasesWin;
    public List<string> enemyEndPhrasesLoose;
}
