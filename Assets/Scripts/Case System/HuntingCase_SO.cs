using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HuntingCase_SO", menuName = "ScriptableObjects/Hunting/HuntingCase_SO")]
public class HuntingCase_SO : ScriptableObject
{
    //1. animal spawn data: which animal (prefab), time when to spawn (float), location of spawn (vector 3)
    public List<AnimalSpawnData> All_AnimalSpawnData;
    //2. win condition class data (checks 3 lists of "AnimalIdentity", a list for the following status of the animal: die, escaped, eaten)
    public HuntingCondition WinCondition;
    //3. lose condition class data (can be null, similar to win condition class)
    public HuntingCondition LoseCondition;
    //4. timer duration (float)
    public float TimerDuration;

    

}
