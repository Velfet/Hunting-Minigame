using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimalAction_FinalizeDeath : AnimalAction_Base
{
    public override void Activate_AnimalAction(AnimalAction_ActivateData animalAction_ActivateData)
    {
        //get reference to the animal that this action belongs to
        Animal_AI_Base theAnimal = animalAction_ActivateData.TheAnimal;

        theAnimal.FinalizeDeath();
    }
}
