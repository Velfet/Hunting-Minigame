using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalAction_RunFasterToPoint : AnimalAction_Base
{
public Vector3 RunDestination;

    public override void Activate_AnimalAction(AnimalAction_ActivateData animalAction_ActivateData)
    {
        //get reference to the animal that this action belongs to
        Animal_AI_Base theAnimal = animalAction_ActivateData.TheAnimal;

        //make this animal run to a specific point
        theAnimal.StartRunFasterToPoint(RunDestination);
    }
}
