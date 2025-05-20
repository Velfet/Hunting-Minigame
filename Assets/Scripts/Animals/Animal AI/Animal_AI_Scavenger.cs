using System.Collections;
using System.Collections.Generic;
using HuntingGame;
using UnityEngine;

public class Animal_AI_Scavenger : Animal_AI_Base
{
    [Space(10)]
    [SerializeReference] protected AnimalFinishAction_Base SeePrey_Action;  //should only activate if the prey is dead
    [SerializeReference] protected AnimalFinishAction_Base SeePredator_PredatorAtLeft_Action;
    [SerializeReference] protected AnimalFinishAction_Base SeePredator_PredatorAtRight_Action;
    [SerializeReference] protected AnimalFinishAction_Base NoMorePredator_Action;
    [Space(10)]
    [SerializeField] protected bool ReactToArrow;

    //call this function when the miss aura is hit
    public override void Trigger_MissAuraHit_Action()
    {
        //no reaction if the animal is not alive
        if(AnimalState != AnimalStatus.Alive)
        {
            return;
        }

        //trigger the miss aura hit action
        AnimalAction_ActivateData animalAction_ActivateData = new AnimalAction_ActivateData{
            TheAnimal = this
        };

        if(MissAuraHit_Action != null)
        {
            //update react state
            if(ReactToArrow == true)
            {
                ReactState = AnimalReactState.ArrowHit;
            }
            
            MissAuraHit_Action.Activate_FinishAction(animalAction_ActivateData);
        }
    }

    //call this function when the body is hit
    public override void Trigger_BodyHit_Action(HuntingAttackStats_SO attackData, Vector3 hitPosition)
    {
        //no reaction if the animal is not alive
        if(AnimalState != AnimalStatus.Alive)
        {
            return;
        }

        //move the hit particle position and play it
        WeaponHit_Effect.transform.position = hitPosition;
        WeaponHit_Effect.PlayEffectAnim();

        //play blood hit particle system
        Debug.LogWarning("Play blood hit effect");
        BloodHit_Particle.transform.position = hitPosition;
        BloodHit_Particle.Play();

        //play hit sound effect
        AudioManager.Instance.PlayAudio(AnimalData.AnimalSoundName.Hit_SoundID);

        //trigger the body hit action
        AnimalAction_ActivateData animalAction_ActivateData = new AnimalAction_ActivateData{
            TheAnimal = this,
            AttackData = attackData
        };

        //update react state
        if(ReactToArrow == true)
        {
            ReactState = AnimalReactState.ArrowHit;
        }
        BodyHit_Action.Activate_FinishAction(animalAction_ActivateData);
    }

    //call this function when the head is hit
    public override void Trigger_HeadHit_Action(HuntingAttackStats_SO attackData, Vector3 hitPosition)
    {
        //no reaction if the animal is not alive
        if(AnimalState != AnimalStatus.Alive)
        {
            return;
        }

        //move the hit particle position and play it
        WeaponHit_Effect.transform.position = hitPosition;
        WeaponHit_Effect.PlayEffectAnim();

        //play blood hit particle system
        Debug.LogWarning("Play blood hit effect 2");
        BloodHit_Particle.transform.position = hitPosition;
        BloodHit_Particle.Play();

        //play crit hit sound effect
        AudioManager.Instance.PlayAudio(AnimalData.AnimalSoundName.CritHit_SoundID);

        //Show crit text
        UnityEngine.Vector3 critText_WorldPos = transform.position;
        critText_WorldPos.y += 1.25f;
        ShowCritText(critText_WorldPos);
        
        //trigger the head hit action
        AnimalAction_ActivateData animalAction_ActivateData = new AnimalAction_ActivateData{
            TheAnimal = this,
            AttackData = attackData
        };

        //update react state
        if(ReactToArrow == true)
        {
            ReactState = AnimalReactState.ArrowHit;
        }
        HeadHit_Action.Activate_FinishAction(animalAction_ActivateData);
    }

    //function to react when prey dies, escaped, or eaten;  we'll make override of them in the child classes if needed
    protected override void Handle_MyPrey_Die(Animal_AI_Base theDeadPrey)
    {
        Debug.LogWarning("[Scavenger] prey just died");
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
        //For the scavenger type, a prey also need to be "dead" to be considered optimal
        bool isPreyDead = thePrey.GetAnimalStatus() == AnimalStatus.Dead;

        bool preyIsNotOptimal;
        if(isPreyBeingEaten == false && isPreyDead == true)
        {
            //prey is optimal
            preyIsNotOptimal = false;
        }
        else
        {
            //prey is not optimal
            preyIsNotOptimal = true;
        }

        Debug.LogWarning("[PreySensor] prey not optimal status: " + preyIsNotOptimal);
        Debug.LogWarning("[PreySensor] prey being eaten status: " + isPreyBeingEaten);
        Debug.LogWarning("[PreySensor] prey dead status: " + isPreyDead);

        if(preyIsNotOptimal == true)
        {
            //prey is not optimal
            //add prey to the potential prey list
            if(potentialPreys.Contains(thePrey) == false)
            {
                potentialPreys.Add(thePrey);
                //also add to the reactcollider_preys list
                reactCollider_Preys.Add(thePrey);
            }

            return;
        }

        //check if the prey has not already been added to the prey list
        if(myPreys.Contains(thePrey) == false)
        {
            //add the prey to the list of preys
            myPreys.Add(thePrey);
            //add prey to the reactcollider_preys
            reactCollider_Preys.Add(thePrey);

            React_PreyPredator_AddRemove();
            
        }
    }

    public override void React_PreyPredator_AddRemove()
    {
        if(AnimalState != AnimalStatus.Alive)
        {
            return;
        }
        
        Debug.LogWarning("[Scavenger] prey/predator was added");
        //if scavenger is running away from predator or reacting from being shot, don't interrupt them
        if(ReactState == AnimalReactState.RunFromPredator || ReactState == AnimalReactState.ArrowHit)
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

        
        bool isCurrentState_ReactToPreyOrPredator;
        bool isNewState_ReactToPreyOrPredator;
        if(myPredators.Count > 0)
        {
            //if going from default or arrowhit to chaseprey or RunFromPredator
            //, then store the previous masterstate_index and state_index in their previous variable counterpart
            isCurrentState_ReactToPreyOrPredator = currentReactState == AnimalReactState.ChasePrey || currentReactState == AnimalReactState.RunFromPredator;
            isNewState_ReactToPreyOrPredator = newReactState == AnimalReactState.ChasePrey || newReactState == AnimalReactState.RunFromPredator;
            if(isCurrentState_ReactToPreyOrPredator == false && isNewState_ReactToPreyOrPredator == true)
            {
                //going from not reacting to prey/predator TO reacting to prey/predator
                //store previous masterstate_index and state_index in their previous variable counterpart, also the react state
                Previous_MasterState_Index = local_Previous_MasterState_Index;
                Previous_State_Index = local_Previous_State_Index;
                Previous_ReactState = local_Previous_ReactState;
            }
        }
        

        //priority C: no prey, no predator -> use previous master state index and previous state index to go back
        //to doing what the animal was doing before it reacted to prey/predator
        //If going from prey -> no prey, might start custom action
        //If going from predator -> no predator, might start custom action
        if(myPredators.Count == 0)
        {
            //update state
            newReactState = Previous_ReactState;

            //priority A: prey -> is previous prey valid? If not, replace with new prey and enact action
            //Enact action if previous prey (can be null) is not the same as current prey (should not be null)
            //Do not react to prey if currently being chased by predator
            if(myPreys.Count > 0)
            {
                //check current prey
                if(currentPrey == null)
                {
                    //get closest prey to this animal
                    currentPrey = GetClosestAnimal(myPreys);
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
                }
                
                //update state
                newReactState = AnimalReactState.ChasePrey;
            }

            //if going from default or arrowhit to chaseprey or RunFromPredator
            //, then store the previous masterstate_index and state_index in their previous variable counterpart
            isCurrentState_ReactToPreyOrPredator = currentReactState == AnimalReactState.ChasePrey || currentReactState == AnimalReactState.RunFromPredator;
            isNewState_ReactToPreyOrPredator = newReactState == AnimalReactState.ChasePrey || newReactState == AnimalReactState.RunFromPredator;
            if(isCurrentState_ReactToPreyOrPredator == false && isNewState_ReactToPreyOrPredator == true)
            {
                //going from not reacting to prey/predator TO reacting to prey/predator
                //store previous masterstate_index and state_index in their previous variable counterpart, also the react state
                Previous_MasterState_Index = local_Previous_MasterState_Index;
                Previous_State_Index = local_Previous_State_Index;
                Previous_ReactState = local_Previous_ReactState;
            }

            //check if state is: reacting to predator -> no predator
            if(myPreys.Count == 0 && isCurrentState_ReactToPreyOrPredator == true && isNewState_ReactToPreyOrPredator == false)
            {
                bool isCurrentState_ReactToPredator = currentReactState == AnimalReactState.RunFromPredator;
                //reacting to predator -> no predator
                if(isCurrentState_ReactToPredator == true)
                {
                    React_NoMore_Predator();
                }
                else
                {
                    //reacting to prey -> no prey
                    MasterState_Index = Previous_MasterState_Index;
                    State_Index = Previous_State_Index;
                    ActivateCurrentAction();
                }
                
            }
            else if(myPreys.Count == 0)
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
                if(SeePredator_PredatorAtLeft_Action != null && SeePredator_PredatorAtRight_Action != null)
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

    protected void React_See_Prey(Animal_AI_Base thePrey, AnimalBehaviourType behaviourType)
    {
        AnimalAction_ActivateData animalAction_ActivateData = new AnimalAction_ActivateData{
            TheAnimal = this,
            TheTargetAnimal = thePrey
        };

        //execute "see prey" action
        SeePrey_Action.Activate_FinishAction(animalAction_ActivateData);
    }

}
