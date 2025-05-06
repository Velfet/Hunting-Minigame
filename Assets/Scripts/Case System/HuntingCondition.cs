using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HuntingCondition
{
    public bool UsingDefaultCondition;
    public bool DefaultCondition;
    [SerializeField] private List<AnimalIdentity> Must_Die;   //list of animals that need to die so that this condition can be met
    [SerializeField] private List<AnimalIdentity> Must_Escape; //list of animals that need to escape so that this condition can be met
    [SerializeField] private List<AnimalIdentity> Must_BeEaten;   //list of animals that need to be eaten so that this condition can be met
    [SerializeField] private List<AnimalIdentity> Must_BeKilled;   //list of animals that need to be killed so that this condition can be met

    //accepts 3 lists of "AnimalIdentity" and determine whether this hunting condition is met or not
    public virtual bool Get_ConditionStatus(HuntingCondition_Arguments theArgument)
    {
        //check if this condition uses the default value
        if(UsingDefaultCondition == true)
        {
            return DefaultCondition;
        }

        bool dieCondition_Met = MyListUtils.Is_ListA_Inside_ListB(Must_Die, theArgument.Animal_Die);
        bool escapeCondition_Met = MyListUtils.Is_ListA_Inside_ListB(Must_Escape, theArgument.Animal_Escape);
        bool eatenCondition_Met = MyListUtils.Is_ListA_Inside_ListB(Must_BeEaten, theArgument.Animal_BeEaten);
        bool killedCondition_Met = MyListUtils.Is_ListA_Inside_ListB(Must_BeKilled, theArgument.Animal_BeKilled);

        return dieCondition_Met && escapeCondition_Met && eatenCondition_Met && killedCondition_Met;
    }


}
