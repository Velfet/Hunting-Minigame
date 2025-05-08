using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal_AI_Prey : Animal_AI_Base
{
    [Space(10)]
    [SerializeReference] protected AnimalFinishAction_Base SeePredator_Action;
    [SerializeReference] protected AnimalFinishAction_Base SeePredator_PredatorAtLeft_Action;
    [SerializeReference] protected AnimalFinishAction_Base SeePredator_PredatorAtRight_Action;
    [SerializeReference] protected AnimalFinishAction_Base NoMorePredator_Action;

    public override void React_PreyPredator_AddRemove()
    {
        if(AnimalState != AnimalStatus.Alive)
        {
            return;
        }
        
        AnimalReactState currentReactState = ReactState;

        //potentially alter the animal react state of this animal
        AnimalReactState newReactState = ReactState;

        //local index and state to avoid tampering
        int local_Previous_MasterState_Index = MasterState_Index;
        int local_Previous_State_Index = State_Index;
        AnimalReactState local_Previous_ReactState = ReactState;

        //Not used by prey
        //priority A: prey -> is previous prey valid? If not, replace with new prey and enact action
        //Enact action if previous prey (can be null) is not the same as current prey (should not be null)

        //priority B: predator -> is previous predator valid? If not, replace with new predator
        //Enact action if previous predator (can be null) is not the same as the current predator (should not be null)
        if(myPredators.Count > 0)
        {
            //check current predator
            if(currentPredator == null)
            {
                //get closest predator to this animal
                currentPredator = GetClosestAnimal(myPredators);
                //determine predator behaviour type
                AnimalBehaviourType predatorBehaviourType = currentPredator.GetAnimalBehaviourType();
                //activate action depending on the current predator's behaviour type
                React_See_Predator(currentPredator, predatorBehaviourType);
            }
            else
            {
                //check closest predator
                Animal_AI_Base closestPredator = GetClosestAnimal(myPredators);
                if(closestPredator != currentPredator)
                {
                    //replace current predator with the closest predator
                    currentPredator = closestPredator;
                    //determine predator behaviour type
                    AnimalBehaviourType predatorBehaviourType = currentPredator.GetAnimalBehaviourType();
                    //activate action depending on the current predator's behaviour type
                    React_See_Predator(currentPredator, predatorBehaviourType);
                }
            }
            
            //update state
            newReactState = AnimalReactState.RunFromPredator;
        }
        else
        {
            //update state
            newReactState = Previous_ReactState;
        }

        

        //if going from default or arrowhit to chaseprey or RunFromPredator
        //, then store the previous masterstate_index and state_index in their previous variable counterpart
        bool isCurrentState_ReactToPreyOrPredator = currentReactState == AnimalReactState.ChasePrey || currentReactState == AnimalReactState.RunFromPredator;
        bool isNewState_ReactToPreyOrPredator = newReactState == AnimalReactState.ChasePrey || newReactState == AnimalReactState.RunFromPredator;
        if(isCurrentState_ReactToPreyOrPredator == false && isNewState_ReactToPreyOrPredator == true)
        {
            //going from not reacting to prey/predator TO reacting to prey/predator
            //store previous masterstate_index and state_index in their previous variable counterpart, also the react state
            Previous_MasterState_Index = local_Previous_MasterState_Index;
            Previous_State_Index = local_Previous_State_Index;
            Previous_ReactState = local_Previous_ReactState;
        }

        //priority C: no prey, no predator -> use previous master state index and previous state index to go back
        //to doing what the animal was doing before it reacted to prey/predator
        //If going from prey -> no prey, might start custom action
        //If going from predator -> no predator, might start custom action
        if(myPredators.Count == 0)
        {
            //update state
            newReactState = Previous_ReactState;

            //check if state is: reacting to predator -> no predator
            if(isCurrentState_ReactToPreyOrPredator == true && isNewState_ReactToPreyOrPredator == false)
            {
                //reacting to predator -> no predator
                React_NoMore_Predator();
            }
            else
            {
                //no predator -> no predator
                //load previous index and start action
                MasterState_Index = Previous_MasterState_Index;
                State_Index = Previous_State_Index;
                ActivateCurrentAction();
            }
           
        }

        //set current state
        ReactState = newReactState;
    }

    protected void React_See_Predator(Animal_AI_Base thePredator, AnimalBehaviourType behaviourType)
    {
        AnimalAction_ActivateData animalAction_ActivateData = new AnimalAction_ActivateData{
            TheAnimal = this,
            ThePredatorAnimal = thePredator
        };

        Vector3 moveDirection = thePredator.transform.position - Animal_GO.transform.position;
        

        //execute different action depending on the behaviour type of the predator; for now, it's all the same
        switch(behaviourType)
        {
            case AnimalBehaviourType.Predator:
                if(SeePredator_Action != null)
                {
                    if(moveDirection.x <= 0)
                    {
                        //run away to the right
                        SeePredator_PredatorAtLeft_Action.Activate_FinishAction(animalAction_ActivateData);
                    }
                    else if(moveDirection.x > 0)
                    {
                        //run away to the left
                        SeePredator_PredatorAtRight_Action.Activate_FinishAction(animalAction_ActivateData);
                    }

                    //old
                    //SeePredator_Action.Activate_FinishAction(animalAction_ActivateData);
                }
                break;
            case AnimalBehaviourType.Scavenger:
                if(SeePredator_Action != null)
                {
                    if(moveDirection.x <= 0)
                    {
                        //run away to the right
                        SeePredator_PredatorAtLeft_Action.Activate_FinishAction(animalAction_ActivateData);
                    }
                    else if(moveDirection.x > 0)
                    {
                        //run away to the left
                        SeePredator_PredatorAtRight_Action.Activate_FinishAction(animalAction_ActivateData);
                    }

                    //old
                    //SeePredator_Action.Activate_FinishAction(animalAction_ActivateData);
                }
                break;
            default:
                break;
        }
    }

    //Going from reacting to predator -> no predator
    protected void React_NoMore_Predator()
    {
        AnimalAction_ActivateData animalAction_ActivateData = new AnimalAction_ActivateData{
            TheAnimal = this,
        };

        //execute the action
        if(NoMorePredator_Action != null)
        {
            NoMorePredator_Action.Activate_FinishAction(animalAction_ActivateData);
        }
    }
}
