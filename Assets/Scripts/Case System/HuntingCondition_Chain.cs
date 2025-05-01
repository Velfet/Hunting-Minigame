using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HuntingCondition_Chain
{
    public bool UsingDefaultCondition;
    public bool DefaultCondition;
    public List<HuntingCondition> AllHuntingConditions;

    public bool Get_Chain_ConditionStatus(List<AnimalIdentity> animal_Die, List<AnimalIdentity> animal_Escape, List<AnimalIdentity> animal_Eaten, List<AnimalIdentity> animal_Killed)
    {
        //check if this condition uses the default value
        if(UsingDefaultCondition == true)
        {
            return DefaultCondition;
        }

        //check if all conditions in the "AllHuntingConditions" are true
        for(int i = 0; i < AllHuntingConditions.Count; i++)
        {
            if(AllHuntingConditions[i].Get_ConditionStatus(animal_Die, animal_Escape, animal_Eaten, animal_Killed) == false)
            {
                //we have found at least 1 hunting condition that is false, return false
                return false;
            }
        }

        //finished checking all hunting conditions
        //If we got to this point, that means that all hunting conditions were true
        //so return true
        return true;
    }
}
