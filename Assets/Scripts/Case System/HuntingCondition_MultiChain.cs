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

    public bool Get_MultiChain_ConditionStatus(List<AnimalIdentity> animal_Die, List<AnimalIdentity> animal_Escape, List<AnimalIdentity> animal_Eaten, List<AnimalIdentity> animal_Killed)
    {
        //check if this condition uses the default value
        if(UsingDefaultCondition == true)
        {
            return DefaultCondition;
        }

        //check if at least one hunting condition chain is true; if so, we can return true
        for(int i = 0; i < AllHuntingCondition_Chains.Count; i++)
        {
            if(AllHuntingCondition_Chains[i].Get_Chain_ConditionStatus(animal_Die, animal_Escape, animal_Eaten, animal_Killed) == true)
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
