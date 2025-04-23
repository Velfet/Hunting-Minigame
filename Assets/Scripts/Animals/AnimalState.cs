using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimalState
{
    public string StateName;
    [SerializeReference] public List<AnimalAction_Base> AnimalActions;
    // [SerializeReference]
    // public List<AnimalAction_Base> AnimalActions;

    public AnimalAction_Base GetAnimalAction(int index)
    {
        return AnimalActions[index];
    }
}
