using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal_React_Collider_Predator : Animal_React_Collider_Base
{
    protected override void OnTriggerEnter(Collider other)
    {
        //TODO check if the other has a specific component (detect roar collider)


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

        //check predator's status
        AnimalStatus predatorStatus = theOtherAnimal.GetAnimalStatus();
        if(predatorStatus != AnimalStatus.Alive)
        {
            //predator is not alive, ignore it
            return;
        }

        //add predator to the animal's list of predators
        theAnimal.AddPredator(theOtherAnimal);
    }

    protected override void OnTriggerExit(Collider other)
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

        //remove predator from the animal's list of predators
        //theAnimal.RemovePredator(theOtherAnimal);
    }

    
}
