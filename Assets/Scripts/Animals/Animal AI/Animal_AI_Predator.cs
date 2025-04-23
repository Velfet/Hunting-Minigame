using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal_AI_Predator : Animal_AI_Base
{
    [Space(10)]
    [SerializeReference] protected AnimalFinishAction_Base SeePrey_Action;
    [SerializeReference] protected AnimalFinishAction_Base SeeScavengerOrFlyer_Action;

    public override void React_PreyPredator_AddRemove()
    {
        AnimalReactState previousReactState = ReactState;

        //potentially alter the animal react state of this animal
        AnimalReactState newReactState = ReactState;
        
        //priority A: prey -> is previous prey valid? If not, replace with new prey and enact action
        //Enact action if previous prey (can be null) is not the same as current prey (should not be null)
        if(myPreys.Count > 0)
        {
            //check current prey
            if(currentPrey == null)
            {
                //get closest prey to this animal
                currentPrey = GetClosestAnimal(myPreys);
                //determine prey behaviour type
                AnimalBehaviourType preyBehaviourType = currentPrey.GetAnimalBehaviourType();
                //activate action depending on the current prey's behaviour type
                React_See_Prey(currentPrey, preyBehaviourType);
            }
            else
            {
                //check closest prey
                Animal_AI_Base closestPrey = GetClosestAnimal(myPreys);
                if(closestPrey != currentPrey)
                {
                    //replace current prey with the closest prey
                    currentPrey = closestPrey;
                    //determine prey behaviour type
                    AnimalBehaviourType preyBehaviourType = currentPrey.GetAnimalBehaviourType();
                    //activate action depending on the current prey's behaviour type
                    React_See_Prey(currentPrey, preyBehaviourType);
                }
            }
            
            //update state
            newReactState = AnimalReactState.ChasePrey;
        }

        //Not used by the predator
        //priority B: predator -> is previous predator valid? If not, replace with new predator
        //Enact action if previous predator (can be null) is not the same as the current predator (should not be null)

        
        //priority C: no prey, no predator -> use previous master state index and previous state index to go back
        //to doing what the animal was doing before it reacted to prey/predator
        //If going from prey -> no prey, might start custom action
        //If going from predator -> no predator, might start custom action
        if(myPreys.Count == 0)
        {
            //load previous index and start action
            MasterState_Index = Previous_MasterState_Index;
            State_Index = Previous_State_Index;
            ActivateCurrentAction();

            //update state
            newReactState = AnimalReactState.Default;
        }

        //if going from default or arrowhit to chaseprey or RunFromPredator
        //, then store the previous masterstate_index and state_index in their previous variable counterpart
        bool isPreviousState_ReactToPreyOrPredator = previousReactState == AnimalReactState.ChasePrey || previousReactState == AnimalReactState.RunFromPredator;
        bool isNewState_ReactToPreyOrPredator = newReactState == AnimalReactState.ChasePrey || newReactState == AnimalReactState.RunFromPredator;
        if(isPreviousState_ReactToPreyOrPredator == false && isNewState_ReactToPreyOrPredator)
        {
            //going from not reacting to prey/predator TO reacting to prey/predator
            //store previous masterstate_index and state_index in their previous variable counterpart
            Previous_MasterState_Index = MasterState_Index;
            Previous_State_Index = State_Index;
        }
    }

    protected void React_See_Prey(Animal_AI_Base thePrey, AnimalBehaviourType behaviourType)
    {
        AnimalAction_ActivateData animalAction_ActivateData = new AnimalAction_ActivateData{
            TheAnimal = this,
            TheTargetAnimal = thePrey
        };

        switch(behaviourType)
        {
            case AnimalBehaviourType.Prey:
                SeePrey_Action.Activate_FinishAction(animalAction_ActivateData);
                break;
            case AnimalBehaviourType.Scavenger:
            case AnimalBehaviourType.Flyer:
                SeeScavengerOrFlyer_Action.Activate_FinishAction(animalAction_ActivateData);
                break;
            default:
                break;
        }


    }
}
