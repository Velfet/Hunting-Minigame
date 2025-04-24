using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimalAction_Roar : AnimalAction_Base
{
    public float RoarDuration;

    public override void Activate_AnimalAction(AnimalAction_ActivateData animalAction_ActivateData)
    {
        //get reference to the animal that this action belongs to
        Animal_AI_Base theAnimal = animalAction_ActivateData.TheAnimal;

        //check if the animal is a predator
        Animal_AI_Predator thePredator = theAnimal as Animal_AI_Predator;
        if(thePredator == null)
        {
            //animal is not a predator, cannot roar
            Debug.LogWarning("[RoarAction] animal is not a predator, cannot roar");
        }
        else
        {
            //make this animal roar for a specific amount of time
            thePredator.StartRoar(RoarDuration);
        }
    }
}
