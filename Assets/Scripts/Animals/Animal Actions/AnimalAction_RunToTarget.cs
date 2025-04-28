using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalAction_RunToTarget : AnimalAction_Base
{
    public override void Activate_AnimalAction(AnimalAction_ActivateData animalAction_ActivateData)
    {
        //get reference to the animal that this action belongs to
        Animal_AI_Base theAnimal = animalAction_ActivateData.TheAnimal;

        //get reference to the transform target
        Animal_AI_Base targetAnimal = animalAction_ActivateData.TheTargetAnimal;
        if(targetAnimal == null)
        {
            //target animal was not found, do not execute action
            Debug.LogWarning("[RunToTarget] no target animal found. Action is not executed");
            return;
        }

        Transform targetTransform = targetAnimal.transform;

        //store the reference to the target animal
        //theAnimal.SetCurrentTargetAnimal(targetAnimal);


        //if the animal is flying, don't track the Y pos, only follow the X pos
        if(targetAnimal.GetAnimalBehaviourType() != AnimalBehaviourType.Flyer)
        {
            //Follow X and Y pos
            //make this animal run to the target transform
            theAnimal.StartRunToTransform(targetTransform);
        }
        else
        {
            //Only follow the X pos
            theAnimal.StartRunToTransform_X(targetTransform);
        }
        
    }
}
