using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Animal_AI_Base : MonoBehaviour
{
    [SerializeField] private GameObject Animal_GO;
    [SerializeField] private AnimalData_SO AnimalData;
    [SerializeField] private AnimalStatus AnimalState;
    [Space(10)]
    [SerializeField] private AnimalMasterState AnimalMasterState;
    [SerializeField] private int MasterState_Index;
    [SerializeField] private int State_Index;
    [SerializeReference] private AnimalAction_Base CurrentAnimalAction;



    public void Start()
    {
        //activate initial action
        ActivateCurrentAction();
    }

    private IEnumerator MoveCoroutine;

    public void StartWalkToPoint(Vector3 walkDestination)
    {
        //stop the previous move coroutine if it exists
        InterruptMove();

        //start a coroutine to make this animal walk to the walk destination
        MoveCoroutine = MoveToPoint(walkDestination, AnimalData.WalkSpeed);
        //Debug.LogWarning("Start move coroutine");
        StartCoroutine(MoveCoroutine);
    }

    public void StarRunToPoint(Vector3 runDestination)
    {
        //stop the previous move coroutine if it exists
        InterruptMove();

        //start a coroutine to make this animal run to the walk destination
        MoveCoroutine = MoveToPoint(runDestination, AnimalData.RunSpeed);
        //Debug.LogWarning("Start move coroutine");
        StartCoroutine(MoveCoroutine);
    }

    public void StartIdle(float idleDuration)
    {
        //stop the previous move coroutine if it exists
        InterruptMove();

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
        while(currentTime <= idleDuration)
        {
            yield return null;
            currentTime += Time.deltaTime;
        }

        //Animal has finished idling, now do something after its done
        ActivateCurrentAction_FinishAction();
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
