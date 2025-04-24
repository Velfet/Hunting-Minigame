using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimalSpawnData
{
    public GameObject Animal_Prefab;
    public float SpawnTime;
    public Vector3 SpawnPosition;
    public bool DeadOnSpawn = false;    //if true, the animal is already dead when it is spawned
}
