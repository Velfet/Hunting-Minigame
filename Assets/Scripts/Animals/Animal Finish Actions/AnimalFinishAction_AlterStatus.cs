using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalFinishAction_AlterStatus : AnimalFinishAction_Base
{
    public AnimalStatus NewStatus;

    public override void Activate_FinishAction(AnimalAction_ActivateData animalAction_ActivateData)
    {
        //get reference to the animal that this finish action belongs to
        Animal_AI_Base theAnimal = animalAction_ActivateData.TheAnimal;

        //update animal status
        theAnimal.UpdateAnimalStatus(NewStatus);
        
    }
}
