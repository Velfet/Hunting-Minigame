using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal_React_Collider_Prey : Animal_React_Collider_Base
{
    protected override void OnTriggerEnter(Collider other)
    {
        //check if the "other" is an animal
        Animal_Detected_Collider animalBodyPart = other.GetComponent<Animal_Detected_Collider>();
        if(animalBodyPart == null)
        {
            //"other" is not an animal
            return;
        }

        //Animal_AI_Base theOtherAnimal = other.GetComponent<Animal_AI_Base>();
        Animal_AI_Base theOtherAnimal = animalBodyPart.TheAnimal;
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
        if(preyStatus == AnimalStatus.Escaped || preyStatus == AnimalStatus.Eaten)
        {
            //prey has escaped or has been eaten, ignore it
            return;
        }

        //add prey to the animal's list of preys
        Debug.LogWarning($"[PreySensor] {theAnimal.gameObject.name} is detecting prey: " + theOtherAnimal.GetAnimalType());
        theAnimal.AddPrey(theOtherAnimal);
    }

    protected override void OnTriggerExit(Collider other)
    {
        //check if the "other" is an animal
        Animal_Detected_Collider animalBodyPart = other.GetComponent<Animal_Detected_Collider>();
        if(animalBodyPart == null)
        {
            //"other" is not an animal
            return;
        }

        //Animal_AI_Base theOtherAnimal = other.GetComponent<Animal_AI_Base>();
        Animal_AI_Base theOtherAnimal = animalBodyPart.TheAnimal;
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

        //remove the prey from potential prey list AND reactcollider_preys list
        theAnimal.RemovePrey(theOtherAnimal);


        //remove prey from the animal's list of preys. No more, only remove prey from list if it is out of sight
        //theAnimal.RemovePrey(theOtherAnimal);
    }


}
