using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HuntingCondition_MultiChain
{
    public bool UsingDefaultCondition;
    public bool DefaultCondition;
    public List<HuntingCondition_Chain> AllHuntingCondition_Chains;

    public bool Get_MultiChain_ConditionStatus(HuntingCondition_Arguments theArgument)
    {
        //check if this condition uses the default value
        if(UsingDefaultCondition == true)
        {
            return DefaultCondition;
        }

        //check if at least one hunting condition chain is true; if so, we can return true
        for(int i = 0; i < AllHuntingCondition_Chains.Count; i++)
        {
            if(AllHuntingCondition_Chains[i] == null)
            {
                continue;
            }
            
            if(AllHuntingCondition_Chains[i].Get_Chain_ConditionStatus(theArgument) == true)
            {
                //we have found at least 1 hunting condition chain that is true, return true
                return true;
            }
        }

        //finished checking all hunting condition chains
        //If we got to this point, that means that all hunting condition chains were false
        //so return false
        return false;
    }
}
