using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HuntingCondition
{
    public bool UsingDefaultCondition;
    public bool DefaultCondition;
    public List<AnimalIdentity> Must_Die;   //list of animals that need to die so that this condition can be met
    public List<AnimalIdentity> Must_Escape; //list of animals that need to escape so that this condition can be met
    public List<AnimalIdentity> Must_BeEaten;   //list of animals that need to be eaten so that this condition can be met
    public List<AnimalIdentity> Must_BeKilled;   //list of animals that need to be killed so that this condition can be met

    //accepts 3 lists of "AnimalIdentity" and determine whether this hunting condition is met or not
    public bool Get_ConditionStatus(List<AnimalIdentity> animal_Die, List<AnimalIdentity> animal_Escape, List<AnimalIdentity> animal_Eaten, List<AnimalIdentity> animal_Killed)
    {
        //check if this condition uses the default value
        if(UsingDefaultCondition == true)
        {
            return DefaultCondition;
        }

        bool dieCondition_Met = MyListUtils.Is_ListA_Inside_ListB(Must_Die, animal_Die);
        bool escapeCondition_Met = MyListUtils.Is_ListA_Inside_ListB(Must_Escape, animal_Escape);
        bool eatenCondition_Met = MyListUtils.Is_ListA_Inside_ListB(Must_BeEaten, animal_Eaten);
        bool killedCondition_Met = MyListUtils.Is_ListA_Inside_ListB(Must_BeKilled, animal_Killed);

        return dieCondition_Met && escapeCondition_Met && eatenCondition_Met && killedCondition_Met;
    }


}
