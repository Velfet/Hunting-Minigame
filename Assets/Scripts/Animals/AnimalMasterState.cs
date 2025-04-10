using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimalMasterState
{
    [SerializeReference] public List<AnimalState> AnimalStates;


    public AnimalAction_Base GetAnimalAction(int stateIndex, int actionindex)
    {
        return AnimalStates[stateIndex].GetAnimalAction(actionindex);
    }
}
