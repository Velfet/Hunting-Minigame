using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal_React_Collider_Prey : Animal_React_Collider_Base
{
    protected override void OnTriggerEnter(Collider other)
    {
        //check if the "other" is an animal
        Animal_AI_Base theOtherAnimal = other.GetComponent<Animal_AI_Base>();
        if(theOtherAnimal == null)
        {
            //"other" is not an animal
            return;
        }
        //check if the other animal makes this animal react or not
        AnimalType otherAnimalType = theOtherAnimal.GetAnimalType();
        if(reactSources.Contains(otherAnimalType) == false)
        {
            //the other animal's type does not make this animal react
            return;
        }

        //check prey's status
        AnimalStatus preyStatus = theOtherAnimal.GetAnimalStatus();
        if(preyStatus != AnimalStatus.Alive || preyStatus != AnimalStatus.Dead)
        {
            //prey has escaped or has been eaten, ignore it
            return;
        }

        //TODO add prey to the animal's list
    }

    //TODO add OnTriggerExit function


}
