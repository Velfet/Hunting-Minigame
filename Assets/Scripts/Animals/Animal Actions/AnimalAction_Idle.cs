using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimalAction_Idle : AnimalAction_Base
{
    public float IdleDuration;

    public override void Activate_AnimalAction(AnimalAction_ActivateData animalAction_ActivateData)
    {
        //get reference to the animal that this action belongs to
        Animal_AI_Base theAnimal = animalAction_ActivateData.TheAnimal;

        //TODO add the finish action to the parameter
        //make this animal idle for a specific amount of time
        theAnimal.StartIdle(IdleDuration);
    }
}
