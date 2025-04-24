using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal_AI_Predator : Animal_AI_Base
{
    [Space(10)]
    [SerializeReference] protected AnimalFinishAction_Base SeePrey_Action;
    [SerializeReference] protected AnimalFinishAction_Base SeeScavengerOrFlyer_Action;
    [SerializeReference] protected AnimalFinishAction_Base Roar_Action; //can be null
    [Space(10)]
    [SerializeField] protected GameObject RoarCollider_GO;

    public override void AddPrey(Animal_AI_Base thePrey)
    {
        //check if the prey has not already been added to the prey list
        if(myPreys.Contains(thePrey) == false)
        {
            //add the prey to the list of preys
            myPreys.Add(thePrey);

            if(Roar_Action == null || thePrey.GetAnimalStatus() != AnimalStatus.Alive)
            {
                //potentially alter behaviour, call a function here
                React_PreyPredator_AddRemove();
            }
            else
            {
                //Do roar action here
                React_Roar(thePrey);
            }
            
        }
    }

    public virtual void ReturnFromRoar()
    {
        //Load previous state index and react state
        MasterState_Index = Previous_MasterState_Index;
        State_Index = Previous_State_Index;
        ReactState = Previous_ReactState;
        //run React_PreyPredator_AddRemove
        React_PreyPredator_AddRemove();
    }

    public override void React_PreyPredator_AddRemove()
    {
        //if predator is roaring, do not interrupt them
        if(RoarCollider_GO.activeInHierarchy == true)
        {
            return;
        }

        //local index and state to avoid tampering
        int local_Previous_MasterState_Index = MasterState_Index;
        int local_Previous_State_Index = State_Index;
        AnimalReactState local_Previous_ReactState = ReactState;

        AnimalReactState currentReactState = ReactState;

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

        
        //if going from default or arrowhit to chaseprey or RunFromPredator
        //, then store the previous masterstate_index and state_index in their previous variable counterpart
        bool isCurrentState_ReactToPreyOrPredator = currentReactState == AnimalReactState.ChasePrey || currentReactState == AnimalReactState.RunFromPredator;
        bool isNewState_ReactToPreyOrPredator = newReactState == AnimalReactState.ChasePrey || newReactState == AnimalReactState.RunFromPredator;
        if(isCurrentState_ReactToPreyOrPredator == false && isNewState_ReactToPreyOrPredator)
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
        if(myPreys.Count == 0)
        {
            //load previous index and start action
            MasterState_Index = Previous_MasterState_Index;
            State_Index = Previous_State_Index;
            //update state
            newReactState = Previous_ReactState;
            ActivateCurrentAction();
        }

        //set current state
        ReactState = newReactState;
    }

    protected void React_See_Prey(Animal_AI_Base thePrey, AnimalBehaviourType behaviourType)
    {
        AnimalAction_ActivateData animalAction_ActivateData = new AnimalAction_ActivateData{
            TheAnimal = this,
            TheTargetAnimal = thePrey
        };

        //execute different action depending on the behaviour type of the prey
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

    //will only run if the prey is still alive
    protected void React_Roar(Animal_AI_Base thePrey)
    {
        AnimalAction_ActivateData animalAction_ActivateData = new AnimalAction_ActivateData{
            TheAnimal = this,
            TheTargetAnimal = thePrey
        };

        //set current prey as null
        currentPrey = null;

        //store previous masterstate_index and state_index in their previous variable counterpart, also the react state
        Previous_MasterState_Index = MasterState_Index;
        Previous_State_Index = State_Index;
        Previous_ReactState = ReactState;

        //set react state as roaring
        ReactState = AnimalReactState.Roaring;

        Roar_Action.Activate_FinishAction(animalAction_ActivateData);
    }

    public void StartRoar(float roarDuration)
    {
        //stop the previous move coroutine if it exists
        InterruptMove();

        //start roar animation
        AnimationManager.Start_Animation(AnimalAnimationKeys.Roar);
        //start a coroutine to make this animal roar for the specified duration
        MoveCoroutine = RoarForSomeTime(roarDuration);
        //Debug.LogWarning("Start eat coroutine");
        StartCoroutine(MoveCoroutine);
    }

    public IEnumerator RoarForSomeTime(float roarDuration)
    {
        //activate roar collider
        RoarCollider_GO.SetActive(true);
        float currentTime = 0f;
        while(currentTime < roarDuration)
        {
            yield return null;
            currentTime += Time.deltaTime;
        }

        //Animal has finished roaring, deactivate the roar collider
        RoarCollider_GO.SetActive(false);
        
        //trigger finish action -> AnimalFinishAction_AfterRoar
        ActivateCurrentAction_FinishAction();
    }

}
