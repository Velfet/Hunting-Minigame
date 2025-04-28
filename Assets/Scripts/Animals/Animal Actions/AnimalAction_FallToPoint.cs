using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimalAction_FallToPoint : AnimalAction_Base
{
    public float yPosDestination;

    public override void Activate_AnimalAction(AnimalAction_ActivateData animalAction_ActivateData)
    {
        //get reference to the animal that this action belongs to
        Animal_AI_Base theAnimal = animalAction_ActivateData.TheAnimal;

        //make this animal walk to a specific point
        theAnimal.StartFallToPoint(yPosDestination);
    }
}
