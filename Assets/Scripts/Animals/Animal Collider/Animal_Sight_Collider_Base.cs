using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal_Sight_Collider_Base : MonoBehaviour
{
    [SerializeField] protected List<AnimalType> reactSources;
    [SerializeField] protected Animal_AI_Base theAnimal;

    protected virtual void OnTriggerEnter(Collider other)
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
    }

    protected virtual void OnTriggerExit(Collider other)
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
    }

    
}
