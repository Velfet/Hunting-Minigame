using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimalAction_Eat : AnimalAction_Base
{
    public float EatDuration;

    public override void Activate_AnimalAction(AnimalAction_ActivateData animalAction_ActivateData)
    {
        //get reference to the animal that this action belongs to
        Animal_AI_Base theAnimal = animalAction_ActivateData.TheAnimal;

        //make this animal eat for a specific amount of time
        theAnimal.StartEat(EatDuration);
    }
}
