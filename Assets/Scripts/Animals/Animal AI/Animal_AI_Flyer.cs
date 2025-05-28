using System.Collections;
using System.Collections.Generic;
using HuntingGame;
using UnityEngine;

public class Animal_AI_Flyer : Animal_AI_Base
{
    [Space(10)]
    [SerializeReference] protected AnimalFinishAction_Base SeePrey_Action;  //should only activate if the prey is dead

    public override void StopAndDeleteAction()
    {
        //stop current action if the current action does not execute if the animal dies
        if(CurrentAnimalAction != null && CurrentAnimalAction.AnimalFinishAction != null && CurrentAnimalAction.AnimalFinishAction.ExecuteEvenIfDead == true)
        {
            //don't interrupt the move
        }
        else
        {
            //interrupt the move
            InterruptMove();

            //play the animal's idle animation if animal is alive
            if(AnimalState == AnimalStatus.Alive)
            {
                AnimationManager.Start_Animation(AnimalAnimationKeys.Idle);
            }
            

            //set current action to null
            CurrentAnimalAction = null;
        }
        
        
    }

    //function to react when prey dies, escaped, or eaten;  we'll make override of them in the child classes if needed
    protected override void Handle_MyPrey_Die(Animal_AI_Base theDeadPrey)
    {
        Debug.LogWarning($"[Flyer] this animal: {gameObject.name},  prey just died which is {theDeadPrey.gameObject.name}");
        //prey is dead; a dead prey a valid prey, so remove it from the "potentialPreys" list and add it to the "myPreys" list
        //and also call "React_PreyPredator_AddRemove" to potentially alter this animal's behaviour
        //BUT this only happens if the prey is stil in the "potentialPreys" list
        if(potentialPreys.Contains(theDeadPrey) == true)
        {
            //remove from "potentialPreys" list
            potentialPreys.Remove(theDeadPrey);
            //add to "myPreys" list
            myPreys.Add(theDeadPrey);
            //potentially alter behaviour
            React_PreyPredator_AddRemove();
        }
    }

    //check if prey should be added to potentialPreys list instead of myPreys
    //and if so, we don't want to call "React_PreyPredator_AddRemove" or the "React_Roar" function
    public override void AddPrey(Animal_AI_Base thePrey)
    {
        //this bool will tell if the prey should be added to the "myPreys" list
        //or the "potentialPreys" list. Either way, it still gets added to the "reactcollider_preys" list
        //The prey is currently being eaten, and since this animal has just received this prey,
        //it is not possible for that eater to be this animal.
        //So, the prey is currently being eaten by another animal
        //, making the prey not optimal
        bool isPreyBeingEaten = thePrey.Get_IsBeingEaten_Status() == true;
        //For the flyer type, a prey also need to be "dead" to be considered optimal
        bool isPreyDead = thePrey.GetAnimalStatus() == AnimalStatus.Dead;

        bool preyIsNotOptimal;
        if (isPreyBeingEaten == false && isPreyDead == true)
        {
            //prey is optimal
            preyIsNotOptimal = false;
        }
        else
        {
            //prey is not optimal
            preyIsNotOptimal = true;
        }

        if (preyIsNotOptimal == true)
        {
            //prey is not optimal
            //add prey to the potential prey list
            if (potentialPreys.Contains(thePrey) == false)
            {
                potentialPreys.Add(thePrey);
                //also add to the reactcollider_preys list
                reactCollider_Preys.Add(thePrey);
            }

            return;
        }

        //check if the prey has not already been added to the prey list
        if (myPreys.Contains(thePrey) == false)
        {
            //add the prey to the list of preys
            myPreys.Add(thePrey);
            //add prey to the reactcollider_preys
            reactCollider_Preys.Add(thePrey);

            React_PreyPredator_AddRemove();

        }
        
        if (reactCollider_Preys.Contains(thePrey) == false)
        {
            //add prey to the reactcollider_preys
            reactCollider_Preys.Add(thePrey);
        }
    }

    public override void React_PreyPredator_AddRemove()
    {
        if(AnimalState != AnimalStatus.Alive)
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
                Debug.LogWarning($"A this animal is {gameObject.name}, current prey just got updated which is {currentPrey.gameObject.name}");
                //determine prey behaviour type
                AnimalBehaviourType preyBehaviourType = currentPrey.GetAnimalBehaviourType();
                //set index
                MasterState_Index = Previous_MasterState_Index;
                State_Index = Previous_State_Index;
                //activate action depending on the current prey's behaviour type
                React_See_Prey(currentPrey, preyBehaviourType);
            }
            else
            {
                //check closest prey
                Animal_AI_Base closestPrey = GetClosestAnimal(myPreys);
                Debug.LogWarning($"B this animal is {gameObject.name}, current prey just got updated which is {currentPrey.gameObject.name}");
                if(closestPrey != currentPrey && isEating == false)
                {
                    //replace current prey with the closest prey
                    currentPrey = closestPrey;
                    //determine prey behaviour type
                    AnimalBehaviourType preyBehaviourType = currentPrey.GetAnimalBehaviourType();
                    //set index
                    MasterState_Index = Previous_MasterState_Index;
                    State_Index = Previous_State_Index;
                    //activate action depending on the current prey's behaviour type
                    React_See_Prey(currentPrey, preyBehaviourType);
                }
                else
                {
                    Debug.LogWarning($"this animal is {gameObject.name}, the closes prey is the same as current prey which is {closestPrey.gameObject.name}");
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
            Debug.LogWarning("[FlyerAI] no more prey, reloading previous state index");
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

        //execute "see prey" action
        SeePrey_Action.Activate_FinishAction(animalAction_ActivateData);
    }



    public override void UpdateAnimalStatus(AnimalStatus newStatus, bool instantDeath = false)
    {
        if(AnimalState == newStatus)
        {
            return;
        }

        AnimalState = newStatus;
        
        switch(newStatus)
        {
            case AnimalStatus.Alive:
                //nothing for now; Maybe set current hp to max hp?
                break;
            case AnimalStatus.Dead:
                //unsubscribe from event of prey and predators
                UnsubFromEvents_ThisAnimalDied();
                //stop the current animal action
                StopAndDeleteAction();
                //play dead animation
                //activate "OnDeath_Action" which should make:
                React_Die_Start();
                
                //play the "death" sound for the animal
                AudioManager.Instance.PlayAudio(AnimalData.AnimalSoundName.Death_SoundID);
                
                //0. disable the "Animal_Detected_Collider"
                //1. the animal fall to the ground
                //2. re-enable the "Animal_Detected_Collider"
                //3. spawn blood effect
                //4. invoke dead action (Raise_OnDeathEvent)
                //[942] show and play blood animation

                //Old start, should remove later
                // if(instantDeath == false)
                // {
                //     AnimationManager.Start_Animation(AnimalAnimationKeys.Die);
                //     AnimalDieBlood_Effect.gameObject.SetActive(true);
                //     AnimalDieBlood_Effect.PlayEffectAnim();
                // }
                // else
                // {
                //     AnimationManager.Start_Animation_JumpToEnd(AnimalAnimationKeys.Die);
                //     AnimalDieBlood_Effect.gameObject.SetActive(true);
                //     AnimalDieBlood_Effect.PlayEffectAnim_JumpToEnd();
                // }
                // //invoke dead action
                // Raise_OnDeathEvent();
                //Old end

                //report status to the case manager
                huntingCaseManager.ReportStatus(AnimalIdentity, AnimalState);
                break;
            case AnimalStatus.Escaped:
                //stop the current animal action
                StopAndDeleteAction();
                //stop animation
                AnimationManager.Stop_Animation();
                //hide animal sprite
                AnimationManager.ToggleAnimalVisualVisibility(false);
                //invoke escape action
                Raise_OnEscapeEvent();
                //report status to the case manager
                huntingCaseManager.ReportStatus(AnimalIdentity, AnimalState);
                break;
            case AnimalStatus.Eaten:
                //update is being eaten status
                isBeingEaten = false;
                //stop animation
                AnimationManager.Stop_Animation();
                //hide animal sprite
                AnimationManager.ToggleAnimalVisualVisibility(false);
                //hide blood
                AnimalDieBlood_Effect.gameObject.SetActive(true);
                //invoke was eaten action
                Raise_OnEatenEvent();
                //report status to the case manager
                huntingCaseManager.ReportStatus(AnimalIdentity, AnimalState);
                break;
        }


    }

    protected void React_Die_Start()
    {
        //0. disable the "Animal_Detected_Collider"
        Animal_Detected_Collider.gameObject.SetActive(false);
        //1. the animal fall to the ground
        AnimalAction_ActivateData animalAction_ActivateData = new AnimalAction_ActivateData{
            TheAnimal = this,
        };
        OnDeath_Action.Activate_FinishAction(animalAction_ActivateData);
        //2. re-enable the "Animal_Detected_Collider"
        //3. spawn blood effect
        //4. invoke dead action (Raise_OnDeathEvent)
    }


}
