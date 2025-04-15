using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Animal_AI_Base : MonoBehaviour
{
    [SerializeField] private GameObject Animal_GO;
    [SerializeField] private AnimalData_SO AnimalData;
    [SerializeField] private AnimalStatus AnimalState;
    [Space(10)]
    [SerializeField] private int CurrentHP;
    [SerializeField] private int MaxHP;
    [Space(10)]
    [SerializeField] private AnimalMasterState AnimalMasterState;
    [SerializeField] private int MasterState_Index;
    [SerializeField] private int State_Index;
    [SerializeReference] private AnimalAction_Base CurrentAnimalAction;
    [Space(10)]
    [SerializeReference] private AnimalFinishAction_Base MissAuraHit_Action;
    [SerializeReference] private AnimalFinishAction_Base BodyHit_Action;
    [SerializeReference] private AnimalFinishAction_Base HeadHit_Action;
    [Space(10)]
    [SerializeField] private AnimalAnimationManager AnimationManager;


    public void SetupAnimal()
    {
        MaxHP = AnimalData.Health;
        CurrentHP = MaxHP;
    }

    public void Start()
    {
        //TODO only for testing
        //enable animal visibility
        AnimationManager.ToggleAnimalVisualVisibility(true);

        //set animal hp
        SetupAnimal();

        //activate initial action
        ActivateCurrentAction();
        //end of testing


    }

    private IEnumerator MoveCoroutine;

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
        //start a coroutine to make this animal run to the walk destination
        MoveCoroutine = MoveToPoint(runDestination, AnimalData.RunSpeed);
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

        //Animal has finished idling, now do something after its done
        ActivateCurrentAction_FinishAction();
    }

    public void StopAndDeleteAction()
    {
        //stop current action
        InterruptMove();

        //set current action to null
        CurrentAnimalAction = null;
    }

    public void ActivateCurrentAction()
    {
        AnimalAction_ActivateData animalAction_ActivateData = new AnimalAction_ActivateData{
            TheAnimal = this
        };

        CurrentAnimalAction = AnimalMasterState.GetAnimalAction(MasterState_Index, State_Index);
        CurrentAnimalAction.Activate_AnimalAction(animalAction_ActivateData);
    }

    public void ActivateCurrentAction_FinishAction()
    {
        if(CurrentAnimalAction != null && CurrentAnimalAction.AnimalFinishAction != null)
        {
            AnimalAction_ActivateData animalAction_ActivateData = new AnimalAction_ActivateData{
                TheAnimal = this
            };
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
    public void Trigger_BodyHit_Action(HuntingAttackStats_SO attackData)
    {
        //no reaction if the animal is not alive
        if(AnimalState != AnimalStatus.Alive)
        {
            return;
        }

        //trigger the miss aura hit action
        AnimalAction_ActivateData animalAction_ActivateData = new AnimalAction_ActivateData{
            TheAnimal = this,
            AttackData = attackData
        };

        BodyHit_Action.Activate_FinishAction(animalAction_ActivateData);
    }

    //call this function when the head is hit
    public void Trigger_HeadHit_Action(HuntingAttackStats_SO attackData)
    {
        //no reaction if the animal is not alive
        if(AnimalState != AnimalStatus.Alive)
        {
            return;
        }
        
        //trigger the miss aura hit action
        AnimalAction_ActivateData animalAction_ActivateData = new AnimalAction_ActivateData{
            TheAnimal = this,
            AttackData = attackData
        };

        HeadHit_Action.Activate_FinishAction(animalAction_ActivateData);
    }


    //call this function when the animal takes damage
    public void DamageAnimal(int damageAmount)
    {
        //TODO do some hit visual effect
        //reduce current hp of the animal
        CurrentHP = Math.Max(CurrentHP - damageAmount, 0);
        //Update animal state only if animal was not already dead
        if(CurrentHP == 0 && AnimalState != AnimalStatus.Dead)
        {
            //update animal state
            UpdateAnimalStatus(AnimalStatus.Dead);
        }

    }

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
                break;
            case AnimalStatus.Escaped:
                //stop the current animal action
                StopAndDeleteAction();
                //stop animation
                AnimationManager.Stop_Animation();
                //hide animal sprite
                AnimationManager.ToggleAnimalVisualVisibility(false);
                break;
            //TODO eaten status has not yet been tested
            case AnimalStatus.Eaten:
                //stop animation
                AnimationManager.Stop_Animation();
                //hide animal sprite
                AnimationManager.ToggleAnimalVisualVisibility(false);
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

    public int GetMasterStateIndex()
    {
        return MasterState_Index;
    }

    public void SetMasterStateIndex(int newIndex)
    {
        MasterState_Index = newIndex;
    }

    public int GetStateIndex()
    {
        return State_Index;
    }

    public void SetStateIndex(int newIndex)
    {
        State_Index = newIndex;
    }

}
