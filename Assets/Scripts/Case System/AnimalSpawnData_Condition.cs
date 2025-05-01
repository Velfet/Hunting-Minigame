using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimalSpawnData_Condition : AnimalSpawnData
{
    public HuntingCondition_MultiChain SpawnCondition_MultiChain;
}
