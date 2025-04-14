using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalAction_RunToPoint : AnimalAction_Base
{
    public Vector3 RunDestination;

    public override void Activate_AnimalAction(AnimalAction_ActivateData animalAction_ActivateData)
    {
        //get reference to the animal that this action belongs to
        Animal_AI_Base theAnimal = animalAction_ActivateData.TheAnimal;

        //TODO add the finish action to the parameter
        //make this animal run to a specific point
        theAnimal.StartRunToPoint(RunDestination);
    }
}
