using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal_Sight_Collider_Predator : Animal_Sight_Collider_Base
{
    protected override void OnTriggerEnter(Collider other)
    {
        Animal_AI_Base theOtherAnimal;

        //check if the other has a specific component (detect roar collider)
        Animal_Roar_Collider roar_Collider = other.GetComponent<Animal_Roar_Collider>();
        if(roar_Collider != null)
        {
            //roar collider is not null; get the animal from that roar collider
            theOtherAnimal = roar_Collider.TheAnimal;
        }
        else
        {
            //roar collider is null; try to get the animal from the collider itself
            //check if the "other" is an animal
            Animal_Detected_Collider animalBodyPart = other.GetComponent<Animal_Detected_Collider>();
            if(animalBodyPart == null)
            {
                //"other" is not an animal
                return;
            }
            
            //theOtherAnimal = other.GetComponent<Animal_AI_Base>();
            theOtherAnimal = animalBodyPart.TheAnimal;
        }

        
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

        //add predator to the animal's list of visible predators
        theAnimal.AddPredator_Visible(theOtherAnimal);
    }

    protected override void OnTriggerExit(Collider other)
    {
        Animal_AI_Base theOtherAnimal;

        //check if the other has a specific component (detect roar collider)
        Animal_Roar_Collider roar_Collider = other.GetComponent<Animal_Roar_Collider>();
        if(roar_Collider != null)
        {
            //roar collider is not null; get the animal from that roar collider
            theOtherAnimal = roar_Collider.TheAnimal;
        }
        else
        {
            //roar collider is null; try to get the animal from the collider itself
            //check if the "other" is an animal
            Animal_Detected_Collider animalBodyPart = other.GetComponent<Animal_Detected_Collider>();
            if(animalBodyPart == null)
            {
                //"other" is not an animal
                return;
            }

            //theOtherAnimal = other.GetComponent<Animal_AI_Base>();
            theOtherAnimal = animalBodyPart.TheAnimal;
        }

        
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

        //remove from list of visible predator
        theAnimal.RemovePredator_Visible(theOtherAnimal);
    }
}
