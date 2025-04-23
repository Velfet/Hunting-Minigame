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
    //TODO need to remove this; we'll use the list of prey or predator instead
    protected Animal_AI_Base currentTargetAnimal;

    //list of prey and predator animals; gets emptied when setting up the animal
    protected List<Animal_AI_Base> myPreys;
    protected List<Animal_AI_Base> myPredators;
    //reset when setting up animal
    protected Animal_AI_Base currentPrey;
    protected Animal_AI_Base currentPredator;

    //TODO invoke action
    //TODO have other animals subscribe to action
    //TODO have other animals unsubscribe to action
    public event Action<Animal_AI_Base> OnDeath;
    public event Action<Animal_AI_Base> OnEaten;
    public event Action<Animal_AI_Base> OnEscape;

    //TODO function: add or remove animal to prey or predator list, this function will call function to potentialy alter behaviour of animal
    //TODO function: make functions to subscribe to prey or predator's:
    //1. death event
    //2. eaten event
    //3. escape event
    //TODO function: potentially alter behaviour of animal depending on current prey and predator list
    //TODO finish actions for the following:
    //1. react prey seen
    //2. react predator seen
    //3. react no more prey
    //4. react no more predator

    //enum AnimalReactState: Default, ArrowHit, ChasePrey, RunFromPredator; reset when setup animal
    //TODO change react state to arrow hit if hit by an arrow and previous state was default
    //(in other words, animal does not react to arrow if they are reacting to prey/predator)
    [Space(10)]
    [SerializeField] protected AnimalReactState ReactState;
    //store master state index and state index before reacting to prey and predator
    //note: reset when setup animal
    [Space(10)]
    [SerializeField] protected int Previous_MasterState_Index;
    [SerializeField] protected int Previous_State_Index;



    public void SetupAnimal()
    {
        MaxHP = AnimalData.Health;
        CurrentHP = MaxHP;
        //set animal state
        UpdateAnimalStatus(AnimalStatus.Alive);
        //setup the HP bar
        Animal_HP_UI.UpdateInstant_HP_Slider_Visual(1f);
    }

    public void SetupAnimal_Complete(HuntingCaseManager newManager)
    {
        //store reference to the hunting case manager
        huntingCaseManager = newManager;

        //no target animal
        currentTargetAnimal = null;

        //empty list of prey and predator
        myPreys = new List<Animal_AI_Base>();
        myPredators = new List<Animal_AI_Base>();
        currentPrey = null;
        currentPredator = null;

        //stop previous action
        StopAndDeleteAction();

        //setup state index
        MasterState_Index = 0;
        State_Index = 0;

        //reset previous state index
        Previous_MasterState_Index = 0;
        Previous_State_Index = 0;

        //reset react state back to default
        ReactState = AnimalReactState.Default;

        //set animal hp
        SetupAnimal();

        //enable animal visibility
        AnimationManager.ToggleAnimalVisualVisibility(true);

        //enable the animal
        gameObject.SetActive(true);

        //activate initial action
        ActivateCurrentAction();
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
        if(currentTargetAnimal == null || currentTargetAnimal.GetAnimalStatus() != AnimalStatus.Dead)
        {
            //target animal does not exist or it is not dead, cannot eat the target animal
            Debug.LogWarning("[StartEat] target animal is null or it is not dead. Cannot eat the target animal");
            return;
        }

        //stop the previous move coroutine if it exists
        InterruptMove();

        //start eat animation
        AnimationManager.Start_Animation(AnimalAnimationKeys.Eat);
        //start a coroutine to make this animal idle for the specified duration
        MoveCoroutine = EatForSomeTime(eatDuration, currentTargetAnimal);
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

            //update destinationPos
            destinationPos = targetTransform.position;
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
        float currentTime = 0f;
        while(currentTime < eatDuration)
        {
            yield return null;
            currentTime += Time.deltaTime;
        }

        //Animal has finished eating, mark the eaten animal as "Eaten"
        theEatenAnimal.UpdateAnimalStatus(AnimalStatus.Eaten);
        //trigger finish action
        ActivateCurrentAction_FinishAction();
    }

    #endregion

    public void StopAndDeleteAction()
    {
        //stop current action
        InterruptMove();

        //set current action to null
        CurrentAnimalAction = null;
    }

    public void ActivateCurrentAction(Animal_AI_Base targetAnimal = null)
    {
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
                TheTargetAnimal = currentTargetAnimal
            };
            Debug.LogWarning("action done");
            CurrentAnimalAction.AnimalFinishAction.Activate_FinishAction(animalAction_ActivateData);
        }
    }

    public void InterruptMove()
    {
        if(MoveCoroutine != null)
        {
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

        MissAuraHit_Action.Activate_FinishAction(animalAction_ActivateData);
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
        BloodHit_Particle.transform.position = hitPosition;
        BloodHit_Particle.Play();
        
        //trigger the head hit action
        AnimalAction_ActivateData animalAction_ActivateData = new AnimalAction_ActivateData{
            TheAnimal = this,
            AttackData = attackData
        };

        HeadHit_Action.Activate_FinishAction(animalAction_ActivateData);
    }

    public void Trigger_ReactCollider_Action(AnimalFinishAction_Base theReactAction, Animal_AI_Base targetAnimal)
    {
        //no reaction if the animal is not alive
        if(AnimalState != AnimalStatus.Alive)
        {
            return;
        }

        //trigger the react collider hit action
        AnimalAction_ActivateData animalAction_ActivateData = new AnimalAction_ActivateData{
            TheAnimal = this,
            TheTargetAnimal = targetAnimal
        };

        theReactAction.Activate_FinishAction(animalAction_ActivateData);
    }

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

    //TODO call this function, enter sight and prey is alive or dead
    public void AddPrey(Animal_AI_Base thePrey)
    {
        //check if the prey has not already been added to the prey list
        if(myPreys.Contains(thePrey) == false)
        {
            //add the prey to the list of preys
            myPreys.Add(thePrey);
            //potentially alter behaviour, call a function here
            React_PreyPredator_AddRemove();
        }
    }

    //TODO call this function, out of sight, when prey escapes or gets eaten
    public void RemovePrey(Animal_AI_Base thePrey)
    {
        //check if the prey is in the prey list
        if(myPreys.Remove(thePrey) == true)
        {
            //prey was in the prey list, it has now been removed
            //potentially alter behaviour, call a function here
            React_PreyPredator_AddRemove();
        }
    }

    //TODO call this function, enter sight and predator is alive
    public void AddPredator(Animal_AI_Base thePredator)
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

    //TODO call this function, out of sight, when prey dies, escapes, or gets eaten
    public void RemovePredator(Animal_AI_Base thePredator)
    {
        //check if the predator is in the predator list
        if(myPredators.Remove(thePredator) == true)
        {
            //predator was in the predator list, it has not been removed
            //potentiall alter behaviour, call a function here
            React_PreyPredator_AddRemove();
        }
    }

    //TODO function to potentially alter behaviour because a prey/predator have been added/removed
    public virtual void React_PreyPredator_AddRemove()
    {
        AnimalReactState previousReactState = ReactState;

        //TODO potentially alter the animal react state of this animal
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

    //TODO function to react when predator dies, escaped, or eaten; we'll make override of them in the child classes
    //TODO function to react when prey dies, escaped, or eaten;  we'll make override of them in the child classes

    public void UpdateAnimalStatus(AnimalStatus newStatus)
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
                AnimationManager.Start_Animation(AnimalAnimationKeys.Die);
                //show and play blood animation
                AnimalDieBlood_Effect.gameObject.SetActive(true);
                AnimalDieBlood_Effect.PlayEffectAnim();
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
                //report status to the case manager
                huntingCaseManager.ReportStatus(AnimalIdentity, AnimalState);
                break;
            //TODO eaten status has not yet been tested
            case AnimalStatus.Eaten:
                //stop animation
                AnimationManager.Stop_Animation();
                //hide animal sprite
                AnimationManager.ToggleAnimalVisualVisibility(false);
                //hide blood
                AnimalDieBlood_Effect.gameObject.SetActive(true);
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

    public void SetCurrentTargetAnimal(Animal_AI_Base newTargetAnimal)
    {
        currentTargetAnimal = newTargetAnimal;
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
}
