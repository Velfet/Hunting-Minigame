using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HuntingCondition_Chain
{
    public bool UsingDefaultCondition;
    public bool DefaultCondition;
    [SerializeReference] public List<HuntingCondition> AllHuntingConditions;

    public bool Get_Chain_ConditionStatus(HuntingCondition_Arguments theArgument)
    {
        //check if this condition uses the default value
        if(UsingDefaultCondition == true)
        {
            return DefaultCondition;
        }

        //check if all conditions in the "AllHuntingConditions" are true
        for(int i = 0; i < AllHuntingConditions.Count; i++)
        {
            if(AllHuntingConditions[i] == null)
            {
                continue;
            }

            if(AllHuntingConditions[i].Get_ConditionStatus(theArgument) == false)
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

    public bool Can_Chain_StillBeFulfilled(HuntingCondition_Arguments theArgument)
    {
        //check if all conditions in the "AllHuntingConditions" can still be fulfilled
        for(int i = 0; i < AllHuntingConditions.Count; i++)
        {
            if(AllHuntingConditions[i] == null)
            {
                continue;
            }

            if(AllHuntingConditions[i].CanConditionStillBeFulfilled(theArgument) == false)
            {
                //we have found at least 1 hunting condition that can no longer be fulfilled, return false
                return false;
            }
        }

        //finished checking all hunting conditions
        //If we got to this point, that means that all hunting conditions are still able to be fulfilled
        //so return true
        return true;
    }
}
