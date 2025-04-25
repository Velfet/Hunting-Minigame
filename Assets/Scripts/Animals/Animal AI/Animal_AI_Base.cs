using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Animal_AI_Base : MonoBehaviour
{
    [SerializeField] protected GameObject Animal_GO;
    [SerializeField] protected AnimalData_SO AnimalData;
    [SerializeField] protected AnimalStatus AnimalState;
    [Space(10)]
    [SerializeField] protected AnimalIdentity AnimalIdentity;
    [Space(10)]
    [SerializeField] protected Animal_HP_UI Animal_HP_UI;
    [SerializeField] protected int CurrentHP;
    [SerializeField] protected int MaxHP;
    [Space(10)]
    [SerializeField] protected AnimalMasterState AnimalMasterState;
    [SerializeField] protected int MasterState_Index;
    [SerializeField] protected int State_Index;
    [SerializeReference] protected AnimalAction_Base CurrentAnimalAction;
    [Space(10)]
    [SerializeReference] protected AnimalFinishAction_Base MissAuraHit_Action;
    [SerializeReference] protected AnimalFinishAction_Base BodyHit_Action;
    [SerializeReference] protected AnimalFinishAction_Base HeadHit_Action;
    [Space(10)]
    [SerializeField] protected AnimalAnimationManager AnimationManager;
    [Space(10)]
    [SerializeField] protected ParticleSystem BloodHit_Particle;
    [SerializeField] protected Anim2DEffect WeaponHit_Effect;
    [SerializeField] protected Anim2DEffect AnimalDieBlood_Effect;

    protected HuntingCaseManager huntingCaseManager;
    //need to remove this; we'll use the list of prey or predator instead
    //protected Animal_AI_Base currentTargetAnimal;

    //list of prey and predator animals; gets emptied when setting up the animal
    protected List<Animal_AI_Base> myPreys;
    protected List<Animal_AI_Base> myPredators;
    //list of VISIBLE prey and predaor animals; gets emptied when setting up the animal
    protected List<Animal_AI_Base> myPreys_Visible;
    protected List<Animal_AI_Base> myPredators_Visible;
    //reset when setting up animal
    protected Animal_AI_Base currentPrey;
    protected Animal_AI_Base currentPredator;

    //TODO list of potential preys;
    //gets populated if the prey is not in an ideal state and it enters the prey react collider
    //gets emptied if the prey escapes/gets eaten or is out of sight (exit) sight collider; also when setting up animal
    //gets moved to the react list if the prey is now in a valid state
    //example: prey was being eaten (invalid) -> prey no longer being eaten but can still be eaten (valid)
    //in the example above, what happened was the animal that was eating the prey stopped eating
    //the prey before it could finish, therefore, the prey is now a valid target for some other animals
    protected List<Animal_AI_Base> potentialPreys;
    
    //TODO list of prey animals in the react collider
    //gets populated if the prey enters the prey react collider
    //gets emptied if the prey escapes/gets eaten or exits the react collider; also when setting up animal
    //this will be used when detecting if an animal
    //should be moved from react list to the potential list of preys
    //(should be moved if: the animal is still in the react collider)
    protected List<Animal_AI_Base> reactCollider_Preys;

    //invoke action
    public event Action<Animal_AI_Base> OnDeath;
    public event Action<Animal_AI_Base> OnEaten;    //gets called when the animal is done being eaten, no more corpse
    public event Action<Animal_AI_Base> OnEscape;
    //TODO invoke these actions; also, subscribe/unsubsribe from these actions
    public event Action<Animal_AI_Base, Animal_AI_Base> OnBeingEaten_Start;     //first parameter is the eat target, second parameter is the eater animal
    public event Action<Animal_AI_Base, Animal_AI_Base> OnBeingEaten_Interrupt; //so, only 1 of these two should run at a time: OnEaten and OnBeingEaten_Interrupt
    //first parameter is the eat target, second parameter is the previous eater animal that got interrupted

    //TODO bool fields to mark eaten and eating state; set to false at setup
    //update these fields accordingly
    protected bool isBeingEaten;
    protected bool isEating;

    //function: add or remove animal to prey or predator list, this function will call function to potentialy alter behaviour of animal
    //function: make functions to subscribe to prey or predator's:
    //1. death event
    //2. eaten event    (eat has ended properly, no corpse remains)
    //3. escape event
    //4. eaten start
    //5. eaten interrupt (eat ended prematurely, corpse does remain)
    //function: potentially alter behaviour of animal depending on current prey and predator list
    //TODO finish actions for the following (might not be needed right now):
    //1. react prey seen
    //2. react predator seen
    //3. react no more prey
    //4. react no more predator

    //enum AnimalReactState: Default, ArrowHit, ChasePrey, RunFromPredator; reset when setup animal
    //TODO change react state to arrow hit if hit by an arrow and previous state was default
    //(in other words, animal does not react to arrow if they are reacting to prey/predator)
    //But might not be needed right now
    [Space(10)]
    [SerializeField] protected AnimalReactState ReactState;
    //store master state index and state index before reacting to prey and predator
    //note: reset when setup animal
    [Space(10)]
    [SerializeField] protected int Previous_MasterState_Index;
    [SerializeField] protected int Previous_State_Index;
    [SerializeField] protected AnimalReactState Previous_ReactState;



    public void SetupAnimal_HP(bool isAlive)
    {
        if(isAlive)
        {
            MaxHP = AnimalData.Health;
            CurrentHP = MaxHP;
            //set animal state
            UpdateAnimalStatus(AnimalStatus.Alive);
            //setup the HP bar
            Animal_HP_UI.UpdateInstant_HP_Slider_Visual(1f);
        }
        else
        {
            //enable the animal
            gameObject.SetActive(true);
            MaxHP = AnimalData.Health;
            CurrentHP = 0;
            //set animal state, instant death
            UpdateAnimalStatus(AnimalStatus.Dead, true);
            //hide HP bar
            Animal_HP_UI.UpdateInstant_HP_Slider_Visual(0f);
        }
        
    }

    public void SetupAnimal_Complete(HuntingCaseManager newManager, AnimalSpawnData spawnData)
    {
        //store reference to the hunting case manager
        huntingCaseManager = newManager;

        //no target animal
        //currentTargetAnimal = null;

        //empty list of prey and predator
        myPreys = new List<Animal_AI_Base>();
        myPredators = new List<Animal_AI_Base>();
        myPreys_Visible = new List<Animal_AI_Base>();
        myPredators_Visible = new List<Animal_AI_Base>();
        potentialPreys = new List<Animal_AI_Base>();
        reactCollider_Preys = new List<Animal_AI_Base>();
        currentPrey = null;
        currentPredator = null;

        //set eat bool fields
        isBeingEaten = false;
        isEating = false;

        //stop previous action
        StopAndDeleteAction();

        //setup state index
        MasterState_Index = 0;
        State_Index = 0;

        //reset previous state index
        Previous_MasterState_Index = 0;
        Previous_State_Index = 0;
        Previous_ReactState = AnimalReactState.Default;

        //reset react state back to default
        ReactState = AnimalReactState.Default;

        //set animal hp
        SetupAnimal_HP(!spawnData.DeadOnSpawn);

        //enable animal visibility
        AnimationManager.ToggleAnimalVisualVisibility(true);

        //enable the animal
        gameObject.SetActive(true);

        if(spawnData.DeadOnSpawn == false)
        {
            //animal is alive, activate initial action
            ActivateCurrentAction();
        }
        
    }

    // public void Start()
    // {
    //     //only for testing
    //     //enable animal visibility
    //     // AnimationManager.ToggleAnimalVisualVisibility(true);

    //     // //set animal hp
    //     // SetupAnimal();

    //     // //activate initial action
    //     // ActivateCurrentAction();
    //     //end of testing
    // }

    protected IEnumerator MoveCoroutine;
    #region Animal Actions
    public void StartWalkToPoint(Vector3 walkDestination)
    {
        //stop the previous move coroutine if it exists
        InterruptMove();

        //start walk animation
        AnimationManager.Start_Animation(AnimalAnimationKeys.Walk);
        //start a coroutine to make this animal walk to the walk destination
        MoveCoroutine = MoveToPoint(walkDestination, AnimalData.WalkSpeed);
        //Debug.LogWarning("Start move coroutine");
        StartCoroutine(MoveCoroutine);
    }

    public void StartRunToPoint(Vector3 runDestination)
    {
        //stop the previous move coroutine if it exists
        InterruptMove();

        //start run animation
        AnimationManager.Start_Animation(AnimalAnimationKeys.Run);
        //start a coroutine to make this animal run to the run destination
        MoveCoroutine = MoveToPoint(runDestination, AnimalData.RunSpeed);
        //Debug.LogWarning("Start move coroutine");
        StartCoroutine(MoveCoroutine);
    }

    public void StartWalkToTransform(Transform runTarget)
    {
        //stop the previous move coroutine if it exists
        InterruptMove();

        //start walk animation
        AnimationManager.Start_Animation(AnimalAnimationKeys.Walk);
        //start a coroutine to make this animal walk to the walk target
        MoveCoroutine = MoveToTransform_XY(runTarget, AnimalData.WalkSpeed);
        //Debug.LogWarning("Start move coroutine");
        StartCoroutine(MoveCoroutine);
    }

    public void StartRunToTransform(Transform runTarget)
    {
        //stop the previous move coroutine if it exists
        InterruptMove();

        //start run animation
        AnimationManager.Start_Animation(AnimalAnimationKeys.Run);
        //start a coroutine to make this animal run to the run target
        MoveCoroutine = MoveToTransform_XY(runTarget, AnimalData.RunSpeed);
        //Debug.LogWarning("Start move coroutine");
        StartCoroutine(MoveCoroutine);
    }

    public void StartIdle(float idleDuration)
    {
        //stop the previous move coroutine if it exists
        InterruptMove();

        //start idle animation
        AnimationManager.Start_Animation(AnimalAnimationKeys.Idle);
        //start a coroutine to make this animal idle for the specified duration
        MoveCoroutine = IdleForSomeTime(idleDuration);
        //Debug.LogWarning("Start idle coroutine");
        StartCoroutine(MoveCoroutine);
    }

    public void StartEat(float eatDuration)
    {
        //check if the target animal to be eaten is dead or not
        if(currentPrey == null || currentPrey.GetAnimalStatus() != AnimalStatus.Dead)
        {
            //target animal does not exist or it is not dead, cannot eat the target animal
            Debug.LogWarning("[StartEat] target animal is null or it is not dead. Cannot eat the target animal");
            return;
        }

        //stop the previous move coroutine if it exists
        InterruptMove();

        //start eat animation
        AnimationManager.Start_Animation(AnimalAnimationKeys.Eat);
        //start a coroutine to make this animal eat for the specified duration
        MoveCoroutine = EatForSomeTime(eatDuration, currentPrey);
        //Debug.LogWarning("Start eat coroutine");
        StartCoroutine(MoveCoroutine);
        
    }

    public IEnumerator MoveToPoint(Vector3 destinationPos, float moveSpeed)
    {
        float distanceToTarget = Vector3.Distance(Animal_GO.transform.position, destinationPos);
        while(Vector3.Distance(Animal_GO.transform.position, destinationPos) > AnimalConst.CloseEnough_Distance)
        {
            yield return null;
            //animal is not close enough to its goal, keep moving
            Vector3 moveDirection = destinationPos - Animal_GO.transform.position;

            if(distanceToTarget <= moveSpeed * Time.deltaTime)
            {
                // animal is close enough to target, just snap to it
                Animal_GO.transform.position = destinationPos;
            }
            else
            {
                moveDirection = moveDirection.normalized;
                Animal_GO.transform.position += moveDirection * moveSpeed * Time.deltaTime;
            }

            //update distance
            distanceToTarget = Vector3.Distance(Animal_GO.transform.position, destinationPos);
        }

        //Animal has reached its destination, now do something after its done
        ActivateCurrentAction_FinishAction();

    }

    public IEnumerator MoveToTransform_XY(Transform targetTransform, float moveSpeed)
    {
        //Make a vector 3 for the destination position; The X and Y position will follow the "targetTransform",
        //but the Z will follow this animal's Z posiiton
        Vector3 destinationPos = targetTransform.position;
        destinationPos.z = Animal_GO.transform.position.z;

        //get reference to the target animal
        Animal_AI_Base targetAnimal = targetTransform.GetComponent<Animal_AI_Base>();
        if(targetAnimal == null)
        {
            //not moving towards an animal
            Debug.LogWarning("Not moving towards an animal, cannot proceed");
            yield break;
        }

        float distanceToTarget = Vector3.Distance(Animal_GO.transform.position, destinationPos);
        //keep running towards the target animal as long as they're alive, even if we have already reached the target animal
        while(Vector3.Distance(Animal_GO.transform.position, destinationPos) > AnimalConst.CloseEnough_Distance || targetAnimal.GetAnimalStatus() == AnimalStatus.Alive)
        {
            yield return null;
            //animal is not close enough to its goal, keep moving
            Vector3 moveDirection = destinationPos - Animal_GO.transform.position;

            if(distanceToTarget <= moveSpeed * Time.deltaTime)
            {
                // animal is close enough to target, just snap to it
                Animal_GO.transform.position = destinationPos;
            }
            else
            {
                moveDirection = moveDirection.normalized;
                Animal_GO.transform.position += moveDirection * moveSpeed * Time.deltaTime;
            }

            //update destinationPos
            destinationPos = targetTransform.position;
            destinationPos.z = Animal_GO.transform.position.z;
            //update distance
            distanceToTarget = Vector3.Distance(Animal_GO.transform.position, destinationPos);
            
        }

        //Animal has reached its destination, now do something after its done
        ActivateCurrentAction_FinishAction();
    }

    public IEnumerator IdleForSomeTime(float idleDuration)
    {
        float currentTime = 0f;
        while(currentTime < idleDuration)
        {
            yield return null;
            currentTime += Time.deltaTime;
        }

        //Animal has finished idling, now trigger current action's finish action
        ActivateCurrentAction_FinishAction();
    }

    public IEnumerator EatForSomeTime(float eatDuration, Animal_AI_Base theEatenAnimal)
    {
        //start eat here
        isEating = true;
        //inform the eaten animal that it is being eaten
        theEatenAnimal.BeingEaten_Begin(this);
        float currentTime = 0f;
        while(currentTime < eatDuration)
        {
            yield return null;
            currentTime += Time.deltaTime;
        }

        //finish eating here
        isEating = false;
        //trigger finish action
        ActivateCurrentAction_FinishAction();
        //Animal has finished eating, mark the eaten animal as "Eaten"
        theEatenAnimal.UpdateAnimalStatus(AnimalStatus.Eaten);
    }

    #endregion

    public void StopAndDeleteAction()
    {
        //stop current action
        InterruptMove();
        //play the animal's idle animation if animal is alive
        if(AnimalState == AnimalStatus.Alive)
        {
            AnimationManager.Start_Animation(AnimalAnimationKeys.Idle);
        }
        

        //set current action to null
        CurrentAnimalAction = null;
    }

    public void ActivateCurrentAction(Animal_AI_Base targetAnimal = null)
    {
        //animal can't act if it is not alive
        if(AnimalState != AnimalStatus.Alive)
        {
            Debug.LogWarning("Animal is not alive, it cannot activate current action. Animal: " + AnimalIdentity.Name);
            return;
        }

        AnimalAction_ActivateData animalAction_ActivateData = new AnimalAction_ActivateData{
            TheAnimal = this,
            TheTargetAnimal = targetAnimal
        };

        CurrentAnimalAction = AnimalMasterState.GetAnimalAction(MasterState_Index, State_Index);
        CurrentAnimalAction.Activate_AnimalAction(animalAction_ActivateData);
    }

    public void ActivateCurrentAction_FinishAction()
    {
        if(CurrentAnimalAction != null && CurrentAnimalAction.AnimalFinishAction != null)
        {
            AnimalAction_ActivateData animalAction_ActivateData = new AnimalAction_ActivateData{
                TheAnimal = this,
                TheTargetAnimal = currentPrey
            };
            Debug.LogWarning("action done");
            CurrentAnimalAction.AnimalFinishAction.Activate_FinishAction(animalAction_ActivateData);
        }
    }

    public void InterruptMove()
    {
        if(MoveCoroutine != null)
        {
            //check if animal is currently eating
            if(isEating == true)
            {
                //animal was eating but is interrupted, inform the eaten animal
                currentPrey.BeingEaten_Interrupt(this);
            }

            //interrupt current move coroutine
            StopCoroutine(MoveCoroutine);
            MoveCoroutine = null;
        }
    }
    
    //call this function when the miss aura is hit
    public void Trigger_MissAuraHit_Action()
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
            MissAuraHit_Action.Activate_FinishAction(animalAction_ActivateData);
        }
    }

    //call this function when the body is hit
    public void Trigger_BodyHit_Action(HuntingAttackStats_SO attackData, Vector3 hitPosition)
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

        //trigger the body hit action
        AnimalAction_ActivateData animalAction_ActivateData = new AnimalAction_ActivateData{
            TheAnimal = this,
            AttackData = attackData
        };

        BodyHit_Action.Activate_FinishAction(animalAction_ActivateData);
    }

    //call this function when the head is hit
    public void Trigger_HeadHit_Action(HuntingAttackStats_SO attackData, Vector3 hitPosition)
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
        
        //trigger the head hit action
        AnimalAction_ActivateData animalAction_ActivateData = new AnimalAction_ActivateData{
            TheAnimal = this,
            AttackData = attackData
        };

        HeadHit_Action.Activate_FinishAction(animalAction_ActivateData);
    }

    // public void Trigger_ReactCollider_Action(AnimalFinishAction_Base theReactAction, Animal_AI_Base targetAnimal)
    // {
    //     //no reaction if the animal is not alive
    //     if(AnimalState != AnimalStatus.Alive)
    //     {
    //         return;
    //     }

    //     //trigger the react collider hit action
    //     AnimalAction_ActivateData animalAction_ActivateData = new AnimalAction_ActivateData{
    //         TheAnimal = this,
    //         TheTargetAnimal = targetAnimal
    //     };

    //     theReactAction.Activate_FinishAction(animalAction_ActivateData);
    // }

    //call this function when the animal takes damage
    public void DamageAnimal(int damageAmount)
    {
        //reduce current hp of the animal
        CurrentHP = Math.Max(CurrentHP - damageAmount, 0);
        Animal_HP_UI.Update_HP_Slider_Visual((float)CurrentHP/MaxHP);
        //Update animal state only if animal was not already dead
        if(CurrentHP == 0 && AnimalState != AnimalStatus.Dead)
        {
            //update animal state
            UpdateAnimalStatus(AnimalStatus.Dead);
        }

    }

    //call this function, enter reach and prey is alive or dead
    public virtual void AddPrey(Animal_AI_Base thePrey)
    {
        //check if the prey has not already been added to the prey list
        if(myPreys.Contains(thePrey) == false)
        {
            //add the prey to the list of preys
            myPreys.Add(thePrey);
            //add prey to the reactcollider_preys
            reactCollider_Preys.Add(thePrey);
            //potentially alter behaviour, call a function here
            React_PreyPredator_AddRemove();
        }
    }

    //call this function, out of react, when prey escapes or gets eaten
    public virtual void RemovePrey(Animal_AI_Base thePrey)
    {
        //new: do nothing; only remove prey if it is out of sight

        //remove from potential preys
        potentialPreys.Remove(thePrey);

        //remove from reactcollider_preys
        reactCollider_Preys.Remove(thePrey);
    }

    //call this function, enter react and predator is alive
    public virtual void AddPredator(Animal_AI_Base thePredator)
    {
        //check if the predator has not yet already been added to the predator list
        if(myPredators.Contains(thePredator) == false)
        {
            //add the predator the the list of predators
            myPredators.Add(thePredator);
            //potentially alter behaviour, call a function here
            React_PreyPredator_AddRemove();
        }
    }

    //call this function, out of react, when prey dies, escapes, or gets eaten
    public virtual void RemovePredator(Animal_AI_Base thePredator)
    {
        //new: do nothing; only remove prey if it is out of sight
    }

    //enter sight
    public virtual void AddPrey_Visible(Animal_AI_Base thePrey)
    {
        //check if the prey has not already been added to the prey list
        if(myPreys_Visible.Contains(thePrey) == false)
        {
            //add the prey to the list of visible preys
            myPreys_Visible.Add(thePrey);
            //subscribe to the prey's events
            Sub_Or_Unsub_PreyEvents(thePrey, true);
        }
    }

    //exit sight
    public virtual void RemovePrey_Visible(Animal_AI_Base thePrey)
    {
        //check if the prey is in the prey list
        if(myPreys_Visible.Remove(thePrey) == true)
        {
            //also remove from react list
            myPreys.Remove(thePrey);

            //unsubscribe from the prey's events
            Sub_Or_Unsub_PreyEvents(thePrey, false);
            //check if removed prey is current prey; If so, current prey is null
            if(currentPrey == thePrey)
            {
                currentPrey = null;
            }
            //potentially alter behaviour, call a function here
            React_PreyPredator_AddRemove();
        }
        
    }

    //enter sight
    public virtual void AddPredator_Visible(Animal_AI_Base thePredator)
    {
        //check if the predator has not already been added to the predator list
        if(myPredators_Visible.Contains(thePredator) == false)
        {
            //add the predator to the list of visible predators
            myPredators_Visible.Add(thePredator);
            //subscribe to the predator's events
            Sub_Or_Unsub_PredatorEvents(thePredator, true);
        }
            
    }

    //exit sight
    public virtual void RemovePredator_Visible(Animal_AI_Base thePredator)
    {
        //check if the predator is in the predator list
        if(myPredators_Visible.Remove(thePredator) == true)
        {
            //predator was in the predator list, it has not been removed

            //also remove from react list
            myPredators.Remove(thePredator);
            
            //unsubscribe from the predator's events
            Sub_Or_Unsub_PredatorEvents(thePredator, false);
            //check if removed predator is current predator; If so, current predator is null
            if(currentPredator == thePredator)
            {
                currentPredator = null;
            }
            //potentially alter behaviour, call a function here
            React_PreyPredator_AddRemove();
        }

        
    }

    //function to potentially alter behaviour because a prey/predator have been added/removed
    public virtual void React_PreyPredator_AddRemove()
    {
        AnimalReactState previousReactState = ReactState;

        //potentially alter the animal react state of this animal
        AnimalReactState newReactState = ReactState;
        
        //TODO
        
        //priority A: prey -> is previous prey valid? If not, replace with new prey and enact action
        //Enact action if previous prey (can be null) is not the same as current prey (should not be null)

        //priority B: predator -> is previous predator valid? If not, replace with new predator
        //Enact action if previous predator (can be null) is not the same as the current predator (should not be null)

        //priority C: no prey, no predator -> use previous master state index and previous state index to go back
        //to doing what the animal was doing before it reacted to prey/predator
        //If going from prey -> no prey, might start custom action
        //If going from predator -> no predator, might start custom action



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

    //TODO when miss aura, body, or head is hit, might not react if animal is currently reacting to prey/predator

    //function to react when predator dies, escaped, or eaten; we'll make override of them in the child classes if needed
    protected virtual void Handle_MyPredator_Die(Animal_AI_Base theDeadPredator)
    {
        //remove from list of predator
        //unsubscribe from predator events
        //potentially alter behaviour

        //this function should already be doing all 3 of the things above
        RemovePredator(theDeadPredator);
        //also remove from visible list
        RemovePredator_Visible(theDeadPredator);
    }

    protected virtual void Handle_MyPredator_Escape(Animal_AI_Base theEscapePredator)
    {
        //remove from list of predator
        //unsubscribe from predator events
        //potentially alter behaviour

        //this function should already be doing all 3 of the things above
        RemovePredator(theEscapePredator);
        //also remove from visible list
        RemovePredator_Visible(theEscapePredator);
    }

    protected virtual void Handle_MyPredator_Eaten(Animal_AI_Base theEatenPredator)
    {
        //should not run because an animal needs to die before it is eaten
        //and after it died, we should already have unsubscribed from that dead animal's event actions
        Debug.LogWarning("Predator eaten handler is called. Should not happen because the handler should already be removed when the predator died");
    }
    
    //function to react when prey dies, escaped, or eaten;  we'll make override of them in the child classes if needed
    protected virtual void Handle_MyPrey_Die(Animal_AI_Base theDeadPrey)
    {
        //prey is dead; a dead prey is still a valid prey, so do nothing for now
    }

    protected virtual void Handle_MyPrey_Escape(Animal_AI_Base theEscapePrey)
    {
        //remove from list of prey
        //unsubscribe from prey events
        //potentially alter behaviour

        //this function should already be doing all 3 of the things above
        RemovePrey(theEscapePrey);
        //also remove from visible list
        RemovePrey_Visible(theEscapePrey);
    }

    protected virtual void Handle_MyPrey_Eaten(Animal_AI_Base theEatenPrey)
    {
        //remove from list of prey
        //unsubscribe from prey events
        //potentially alter behaviour

        //this function should already be doing all 3 of the things above
        RemovePrey(theEatenPrey);
        //also remove from visible list
        RemovePrey_Visible(theEatenPrey);
    }

    protected virtual void Handle_MyPrey_EatenBySomeoneElse(Animal_AI_Base theEatenPrey, Animal_AI_Base theEater)
    {
        //fing out if we are the eater or not
        if(theEater == this)
        {
            //we are the eater, no need to do anything else
            return;
        }

        //we are not the eater, our prey is being eaten by someone else

        //find out if the eaten prey is in the reactcollider_prey list
        if(reactCollider_Preys.Contains(theEatenPrey) == true)
        {
            //the eaten prey is still in the react collider of this animal
            //remove the eaten prey from the "myPreys" list
            myPreys.Remove(theEatenPrey);
            //and add it to the "potentialPreys" list
            potentialPreys.Add(theEatenPrey);
        }
        else
        {
            //the eaten prey is no longer is the react collider of this animal
            //just remove the eaten prey from the "mypreys" list
            myPreys.Remove(theEatenPrey);
        }

        //potentially alter behaviour
        React_PreyPredator_AddRemove();
    }

    //gets called when the prey was getting eaten but the eater was interrupted, so the prey's corpse remains
    protected virtual void Handle_MyPrey_EatenGotInterrupted(Animal_AI_Base theEatenPrey, Animal_AI_Base thePreviousEater)
    {
        //fing out if we are the previous eater or not
        if(thePreviousEater == this)
        {
            //we are the previous eater, no need to do anything else
            return;
        }

        //we are not the previous eater, try to move the eaten prey from the "potentialPreys" list
        //to the "myPreys" list
        if(potentialPreys.Contains(theEatenPrey) == true)
        {
            //remove from "potentialPreys" list
            potentialPreys.Remove(theEatenPrey);
            //add to "myPreys" list
            myPreys.Add(theEatenPrey);
            //potentially alter behaviour
            React_PreyPredator_AddRemove();
        }

    }

    public void UpdateAnimalStatus(AnimalStatus newStatus, bool instantDeath = false)
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
                //stop the current animal action
                StopAndDeleteAction();
                //play dead animation
                //show and play blood animation
                if(instantDeath == false)
                {
                    AnimationManager.Start_Animation(AnimalAnimationKeys.Die);
                    AnimalDieBlood_Effect.gameObject.SetActive(true);
                    AnimalDieBlood_Effect.PlayEffectAnim();
                }
                else
                {
                    AnimationManager.Start_Animation_JumpToEnd(AnimalAnimationKeys.Die);
                    AnimalDieBlood_Effect.gameObject.SetActive(true);
                    AnimalDieBlood_Effect.PlayEffectAnim_JumpToEnd();
                }
                //invoke dead action
                OnDeath?.Invoke(this);
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
                OnEscape?.Invoke(this);
                //report status to the case manager
                huntingCaseManager.ReportStatus(AnimalIdentity, AnimalState);
                break;
            //TODO eaten status has not yet been tested
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
                OnEaten?.Invoke(this);
                //report status to the case manager
                huntingCaseManager.ReportStatus(AnimalIdentity, AnimalState);
                break;
        }


    }

    public void ForceSet_AnimalStatus(AnimalStatus newStatus)
    {
        AnimalState = newStatus;
    }

    public AnimalStatus GetAnimalStatus()
    {
        return AnimalState;
    }

    public AnimalType GetAnimalType()
    {
        return AnimalIdentity.AnimalType;
    }

    public AnimalBehaviourType GetAnimalBehaviourType()
    {
        return AnimalIdentity.AnimalBehaviour;
    }

    public int GetMasterStateIndex()
    {
        return MasterState_Index;
    }

    public void SetMasterStateIndex(int newIndex)
    {
        MasterState_Index = newIndex;
    }

    public string GetLootData_String()
    {
        return AnimalData.LootData.ToString();
    }

    public int GetStateIndex()
    {
        return State_Index;
    }

    public void SetStateIndex(int newIndex)
    {
        State_Index = newIndex;
    }

    public AnimalAction_Base GetCurrentAnimalAction()
    {
        return CurrentAnimalAction;
    }

    public void BeingEaten_Begin(Animal_AI_Base theEater)
    {
        //set the being eaten status
        isBeingEaten = true;
        //invoke the being eaten event
        OnBeingEaten_Start?.Invoke(this, theEater);
    }

    public void BeingEaten_Interrupt(Animal_AI_Base thePreviousEater)
    {
        //set the being eaten status
        isBeingEaten = false;
        //invoke the being eaten interrupt event
        OnBeingEaten_Interrupt?.Invoke(this, thePreviousEater);
    }

    public bool Get_IsBeingEaten_Status()
    {
        return isBeingEaten;
    }
    public bool Get_IsEating_Status()
    {
        return isEating;
    }

    protected Animal_AI_Base GetClosestAnimal(List<Animal_AI_Base> animalList)
    {
        Animal_AI_Base closestAnimal = null;
        float smallestDistance = 999f;
        for(int i = 0; i < animalList.Count; i++)
        {
            if(closestAnimal == null)
            {
                closestAnimal = animalList[i];

                smallestDistance = Vector2.Distance(this.transform.position, closestAnimal.transform.position);
                continue;
            }

            float newDistance = Vector2.Distance(this.transform.position, closestAnimal.transform.position);

            //compare new distance and previous distance
            if(newDistance < smallestDistance)
            {
                //found new closest animal
                closestAnimal = animalList[i];
                //update smallest distance
                smallestDistance = newDistance;
            }
        }

        return closestAnimal;
    }

    protected void Sub_Or_Unsub_PreyEvents(Animal_AI_Base thePrey, bool isSubscribing)
    {
        if(isSubscribing == true)
        {
            //sub to the prey's events
            thePrey.OnDeath += Handle_MyPrey_Die;
            thePrey.OnEaten += Handle_MyPrey_Eaten;
            thePrey.OnEscape += Handle_MyPrey_Escape;
            thePrey.OnBeingEaten_Start += Handle_MyPrey_EatenBySomeoneElse;
            thePrey.OnBeingEaten_Interrupt += Handle_MyPrey_EatenGotInterrupted;
        }
        else
        {
            //unsub from the prey's events
            thePrey.OnDeath -= Handle_MyPrey_Die;
            thePrey.OnEaten -= Handle_MyPrey_Eaten;
            thePrey.OnEscape -= Handle_MyPrey_Escape;
            thePrey.OnBeingEaten_Start -= Handle_MyPrey_EatenBySomeoneElse;
            thePrey.OnBeingEaten_Interrupt -= Handle_MyPrey_EatenGotInterrupted;

        }
    }

    protected void Sub_Or_Unsub_PredatorEvents(Animal_AI_Base thePredator, bool isSubscribing)
    {
        if(isSubscribing == true)
        {
            //sub to the predator's events
            thePredator.OnDeath += Handle_MyPredator_Die;
            thePredator.OnEaten += Handle_MyPredator_Eaten;
            thePredator.OnEscape += Handle_MyPredator_Escape;
        }
        else
        {
            //unsub from the predator's events
            thePredator.OnDeath -= Handle_MyPredator_Die;
            thePredator.OnEaten -= Handle_MyPredator_Eaten;
            thePredator.OnEscape -= Handle_MyPredator_Escape;
        }
    }

}
