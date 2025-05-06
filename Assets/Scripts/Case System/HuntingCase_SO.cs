using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HuntingCase_SO", menuName = "ScriptableObjects/Hunting/HuntingCase_SO")]
public class HuntingCase_SO : ScriptableObject
{
    //1. animal spawn data: which animal (prefab), time when to spawn (float), location of spawn (vector 3)
    public List<AnimalSpawnData> All_AnimalSpawnData;
    //list of animal spawn data and the condition that need to be fullfilled to spawn that animal
    public List<AnimalSpawnData_Condition> All_AnimalSpawnData_Condition;
    //list of animal spawn data and the weird condition that need to be fillfilled to spawn that animal
    public List<AnimalSpawnData_Condition> All_AnimalSpawnData_WeirdCondition;
    //2. win condition class data (checks 3 lists of "AnimalIdentity", a list for the following status of the animal: die, escaped, eaten)
    public HuntingCondition_MultiChain WinCondition_Immediate_MultiChain;
    //3. lose condition class data (can be null, similar to win condition class)
    public HuntingCondition_MultiChain LoseCondition_Immediate_MultiChain;
    //win and lose condition that are checked when timer runs out
    public HuntingCondition_MultiChain WinCondition_Timer_MultiChain;
    public HuntingCondition_MultiChain LoseCondition_Timer_MultiChain;
    //4. timer duration (float)
    public float TimerDuration;

    

}
