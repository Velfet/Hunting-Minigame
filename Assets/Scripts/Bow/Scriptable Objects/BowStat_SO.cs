using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BowStat_SO", menuName = "ScriptableObjects/Weapon/BowStat_SO")]
public class BowStat_SO : ScriptableObject
{
    public List<BowStat_Single> BowStats;
}
