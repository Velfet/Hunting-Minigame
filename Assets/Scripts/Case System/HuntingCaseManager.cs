using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntingCaseManager : MonoBehaviour
{
    //process in case manager that is using this Scriptable Object's data:
    //Initialize the index of the current animal spawn data to be the first one (start index = 0)
    //when timer ticks down, check if the current animal in the current animal spawn data should be spawned
    //If the animal is spawned, increment the index; If the index is not valid, then stop checking current animal spawn data
    //Note: with this approach, we need to make sure that the "SpawnTime" in the list of animal spawn data need to be ascending,
    //with the first entry having the lowest "SpawnTime" and the last entry having the highest "SpawnTime"
    //Note 2: when the animal is being spawned by the case manager, give a reference to the case manager to the spawned animals so that
    //the animal can report its state change (it dies, escaped, or gets eaten) to the case manager. This data can be used
    //to determine if the win or lose condition has been met or not

    [SerializeField] private HuntingCase_Data HuntingCase_Data;
    [Space(10)]
    //TODO for test only, need to replace later for final implementation
    [SerializeField] private int TestRank;  //1st rank is rank 1
    [SerializeField] private int TestLevel; //1st level is level 1
    [SerializeField] private HuntingCase_SO TestCase;
    //end of test
    [Space(10)]
    [SerializeField] private List<AnimalSpawnData> AnimalSpawnData_List;
    [SerializeField] private int Current_AnimalSpawnData_Index;
    [Space(10)]
    [SerializeField] private HuntingCase_SO CurrentHuntingCase;
    [SerializeField] private Enum_HuntingGameState CurrentHuntingGameState;
    [Space(10)]
    [SerializeField] private HuntingStageTimer StageTimer;
    [SerializeField] private float MaxTimer;
    [SerializeField] private float CurrentTimer;    //this timer goes from MaxTimer to 0
    [Space(10)]
    [SerializeField] private List<GameObject> SpawnedAnimals;
    [Space(10)]
    [SerializeField] private List<AnimalIdentity> Animal_Dead;
    [SerializeField] private List<AnimalIdentity> Animal_Escape;
    [SerializeField] private List<AnimalIdentity> Animal_Eaten;
    [Space(10)]
    [SerializeField] private float TickRate;
    [SerializeField] private float TickTimer;
    [SerializeField] private float CurrentTimer_GoUp;   //this timer goes from 0 to MaxTimer
    [Space(10)]
    [SerializeField] private HuntingBow HuntingBow;


    private void Start()
    {
        //TODO test only, get hunting case data
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if(TestCase != null)
        {
            CurrentHuntingCase = TestCase;
        }
        else
        {
            CurrentHuntingCase = HuntingCase_Data.GetHuntingCaseData(TestRank, TestLevel);
        }

        //store list of "AnimalSpawnData" from the case
        AnimalSpawnData_List = new List<AnimalSpawnData>(CurrentHuntingCase.All_AnimalSpawnData);
        //store max timer and current timer from the case
        MaxTimer = CurrentHuntingCase.TimerDuration;
        CurrentTimer = MaxTimer;
        TickTimer = 0f;
        CurrentTimer_GoUp = 0f;
        StageTimer.UpdateTimerValue(CurrentTimer);
        //initialize the index of the current "AnimalSpawnData"
        Current_AnimalSpawnData_Index = 0;
        //change hunting game state to "Active"
        Update_HuntingGameState(Enum_HuntingGameState.Active);
        //enable hunting bow
        HuntingBow.Update_BowState(Enum_BowState.Active);
        //end of test


    }

    public void Update()
    {
        if(CurrentHuntingGameState == Enum_HuntingGameState.Active)
        {
            if(CurrentTimer_GoUp == 0f && TickTimer == 0f)
            {
                //tick down timer
                TickTimer += Time.deltaTime;
                //check if we need to spawn any animal right now
                CheckSpawnAnimal();
                return;
            }

            //tick down timer
            TickTimer += Time.deltaTime;
            if(TickTimer >= TickRate)
            {
                TickTimer -= TickRate;
                CurrentTimer_GoUp += TickRate;
                CurrentTimer -= TickRate;
                //Update timer UI
                StageTimer.UpdateTimerValue(CurrentTimer);
                //check if we need to spawn any animal right now
                CheckSpawnAnimal();
                //check if the timer has run out
                CheckOutOfTime();
            }
        }
    }

    private void CheckSpawnAnimal()
    {
        while (Current_AnimalSpawnData_Index < AnimalSpawnData_List.Count && CurrentTimer_GoUp >= AnimalSpawnData_List[Current_AnimalSpawnData_Index].SpawnTime)
        {
            AnimalSpawnData currentAnimalSpawnData = AnimalSpawnData_List[Current_AnimalSpawnData_Index];
            //Spawn animal
            GameObject currentSpawnedAnimal = Instantiate(currentAnimalSpawnData.Animal_Prefab, currentAnimalSpawnData.SpawnPosition, currentAnimalSpawnData.Animal_Prefab.transform.rotation);
            SpawnedAnimals.Add(currentSpawnedAnimal);
            //give reference of this case manager to the spawned animal
            currentSpawnedAnimal.GetComponent<Animal_AI_Base>().SetupAnimal_Complete(this);
            //increment index
            Current_AnimalSpawnData_Index++;
        }
    }

    private void CheckOutOfTime()
    {
        if(CurrentTimer <= 0f)
        {
            //timer ran out, trigger the lose event
            Lose_OutOfTime();
        }
    }
    

    //function for animals to report their status
    public void ReportStatus(AnimalIdentity theAnimal, AnimalStatus newStatus)
    {
        //Note: an animal needs to die before it can be eaten
        switch(newStatus)
        {
            case AnimalStatus.Dead:
                //add the animal to the list of dead animals
                Animal_Dead.Add(theAnimal);
                //check win and lose condition
                Check_WinOrLose_Condition();
                break;
            case AnimalStatus.Escaped:
                //add the animal to the list of escaped animals
                Animal_Escape.Add(theAnimal);
                //check win and lose condition
                Check_WinOrLose_Condition();
                break;
            case AnimalStatus.Eaten:
                //remove the animal from the dead animals list
                Animal_Dead.Remove(theAnimal);
                //add the animal to the list of eaten animals
                Animal_Eaten.Add(theAnimal);
                //check win and lose condition
                Check_WinOrLose_Condition();
                break;
            case AnimalStatus.Alive:
            default:
                //nothing for now
                break;
        }
    }

    public void Check_WinOrLose_Condition()
    {
        bool winCondition_Met = CurrentHuntingCase.WinCondition.Get_ConditionStatus(Animal_Dead, Animal_Escape, Animal_Eaten);
        if(winCondition_Met == true)
        {
            WinEvent();
            return;
        }

        bool loseCondition_Met = CurrentHuntingCase.LoseCondition.Get_ConditionStatus(Animal_Dead, Animal_Escape, Animal_Eaten);
        if(loseCondition_Met == true)
        {
            LoseEvent();
            return;
        }

    }

    //call this function if the timer runs out
    public void Lose_OutOfTime()
    {
        //check if the win or lose condition is met
        Check_WinOrLose_Condition();

        //timer has run out, trigger lose condition
        if(CurrentHuntingGameState != Enum_HuntingGameState.Inactive)
        {
            LoseEvent();
        }
    }


    private void WinEvent()
    {
        //stop the timer
        //change hunting game state to "Active"
        Update_HuntingGameState(Enum_HuntingGameState.Inactive);
        //disable hunting bow
        HuntingBow.Update_BowState(Enum_BowState.NonActive);
        //enable mouse
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //TODO win condition is met, trigger win event
        Debug.LogWarning("Win event");
    }

    private void LoseEvent()
    {
        //stop the timer
        //change hunting game state to "Active"
        Update_HuntingGameState(Enum_HuntingGameState.Inactive);
        //disable hunting bow
        HuntingBow.Update_BowState(Enum_BowState.NonActive);
        //enable mouse
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //TODO lose condition is met, trigger lose event
        Debug.LogWarning("Lose event");
    }

    public void Update_HuntingGameState(Enum_HuntingGameState newState)
    {
        if(CurrentHuntingGameState == newState)
        {
            return;
        }

        CurrentHuntingGameState = newState;
    }
}
