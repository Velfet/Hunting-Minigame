using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal_React_Collider_Base : MonoBehaviour, IAnimalPart_React
{
    [SerializeField] protected List<AnimalType> reactSources;
    [SerializeReference] protected AnimalFinishAction_Base reactAction;
    [SerializeField] protected Animal_AI_Base theAnimal;

    public List<AnimalType> ReactSources => reactSources;
    public AnimalFinishAction_Base ReactAction => reactAction;
    public Animal_AI_Base TheAnimal => theAnimal;

    protected Animal_AI_Base targetAnimal;

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

        //two child class of this class:
        //1. Animal_React_Collider_Prey -> detects prey of this animal
        //2. Animal_React_Collider_Predator -> detects predator of this animal

        //AnimalAction_RunToTarget no longer set the target animal

        //TODO add OnTriggerExit function

        //Start
        //depending on some field in this class, store "theOtherAnimal"
        //as either this animal's "target animal" or "predator animal". Also check if the prey or predator animal is in a valid state to be considered
        //predator: Alive
        //prey: Alive, Dead
        //TODO also, the animal need to subscribe to the death, escape, and eaten event of the prey/predatory animal
        //End
        //TODO probably want to put all of these (from Start to End) in 1 function over at "Animal_AI_Base"


        //TODO old, please delete later
        //store a reference to the target animal
        targetAnimal = theOtherAnimal;

        //make this animal react by calling the function "OnReactColliderTriggered"
        OnReactColliderTriggered();
        //end of old
    }


    //might not be needed
    public virtual void OnReactColliderTriggered()
    {
        //TODO might want to check if target animal is already eaten or not; the state can be
        //determined by some field in this class
        //tell the animal that the react collider was triggered
        theAnimal.Trigger_ReactCollider_Action(ReactAction, targetAnimal);
    }
}
