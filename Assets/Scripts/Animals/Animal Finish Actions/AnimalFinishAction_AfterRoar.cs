using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalFinishAction_AfterRoar : AnimalFinishAction_Base
{
    public override void Activate_FinishAction(AnimalAction_ActivateData animalAction_ActivateData)
    {
        //get reference to the animal that this finish action belongs to
        Animal_AI_Base theAnimal = animalAction_ActivateData.TheAnimal;

        //check if the animal is a predator
        Animal_AI_Predator thePredator = theAnimal as Animal_AI_Predator;
        if(thePredator == null)
        {
            //animal is not a predator, cannot roar
            Debug.LogWarning("[RoarAction] animal is not a predator, after roar finish action cannot execute");
        }
        else
        {
            //call the "React_PreyPredator_AddRemove" of the animal
            thePredator.ReturnFromRoar();
        }

    }
}
