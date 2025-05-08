using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntingCondition_AnimalBeginEat : HuntingCondition
{
    [SerializeField] private AnimalIdentity AnimalThatBeganToEat;

    public override bool Get_ConditionStatus(HuntingCondition_Arguments theArgument)
    {
        //check if this condition uses the default value
        if(UsingDefaultCondition == true)
        {
            return DefaultCondition;
        }

        bool animalEatingCondition_Met = false;
        if(theArgument.Animal_IsEating != null && theArgument.Animal_IsEating.Count > 0)
        {
            animalEatingCondition_Met = AnimalThatBeganToEat.Equals(theArgument.Animal_IsEating[0]);
        }

        return animalEatingCondition_Met;
    }

    public override bool CanConditionStillBeFulfilled(HuntingCondition_Arguments theArgument)
    {
        //if the animal that is supposed to began to eat is alredy dead, this condition can no longer be fulfilled
        if(theArgument.Animal_BeKilled.Contains(AnimalThatBeganToEat) == true)
        {
            //Debug.LogWarning("[TestCondition] condition can not be fulfilled");
            return false;
        }
        else
        {
            return true;
        }
    }
}
