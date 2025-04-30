using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
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
    [SerializeField] private int CurrentRank;  //1st rank is rank 1
    [SerializeField] private int CurrentLevel; //1st level is level 1
    //TODO for test only, need to replace later for final implementation
    [SerializeField] private HuntingCase_SO TestCase;
    //end of test
    [Space(10)]
    [SerializeField] private List<AnimalSpawnData> AnimalSpawnData_List;
    [SerializeField] private int Current_AnimalSpawnData_Index;
    [Space(10)]
    [SerializeField] private HuntingCase_SO CurrentHuntingCase;
    [SerializeField] private Enum_HuntingGameState CurrentHuntingGameState;
    [Space(10)]
    [SerializeField] private HuntingUIManager HuntingUI;
    [SerializeField] private float MaxTimer;
    [SerializeField] private float CurrentTimer;    //this timer goes from MaxTimer to 0
    [Space(10)]
    [SerializeField] private List<GameObject> SpawnedAnimals;
    //[SerializeField] private List<GameObject> CurrentLevel_SpawnedAnimals;  //used for reloading levels; Unused
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
    [Space(10)]
    [SerializeField] private ObjectPoolerManager ObjectPoolerManager;


    private void Start()
    {
        //TODO test only, get hunting case data
        if(TestCase != null)
        {
            CurrentHuntingCase = TestCase;
            LoadCurrentLevel();
        }
    }

    //function might not be necessary
    public void ExitLevel()
    {
        // if(CurrentLevel_SpawnedAnimals != null)
        // {
        //     CurrentLevel_SpawnedAnimals.Clear();
        // }
        
        if(SpawnedAnimals != null)
        {
            for(int i = 0; i < SpawnedAnimals.Count; i++)
            {
                Destroy(SpawnedAnimals[i].gameObject);
            }
            SpawnedAnimals.Clear();
        }
    }

    public void Load_NextLevel()
    {
        Load_SpecifiedLevel(CurrentRank, CurrentLevel+1);
    }

    //TODO call this function when wanting to load a specified level
    public void Load_SpecifiedLevel(int newRank, int newLevel)
    {
        //clear previous level's spawned animal list
        // if(CurrentLevel_SpawnedAnimals != null)
        // {
        //     CurrentLevel_SpawnedAnimals.Clear();
        // }
        
        if(SpawnedAnimals != null)
        {
            for(int i = 0; i < SpawnedAnimals.Count; i++)
            {
                Destroy(SpawnedAnimals[i].gameObject);
            }
            SpawnedAnimals.Clear();
        }
        

        //load the specified level
        CurrentRank = newRank;
        CurrentLevel = newLevel;
        CurrentHuntingCase = HuntingCase_Data.GetHuntingCaseData(CurrentRank, CurrentLevel);
        LoadCurrentLevel();
    }

    public void ReloadCurrentLevel()
    {
        // if(SpawnedAnimals != null)
        // {
        //     CurrentLevel_SpawnedAnimals = new List<GameObject>(SpawnedAnimals);
        // }
        // else
        // {
        //     CurrentLevel_SpawnedAnimals = new List<GameObject>();
        // }

        //destroy any spawned animals before reloading the level
        if(SpawnedAnimals != null)
        {
            for(int i = 0; i < SpawnedAnimals.Count; i++)
            {
                Destroy(SpawnedAnimals[i].gameObject);
            }
            SpawnedAnimals.Clear();
        }
        
        LoadCurrentLevel();
    }

    public void LoadCurrentLevel()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //clear the list of spawned animals
        SpawnedAnimals.Clear();
        //empty list of dead, escaped, and eaten animals
        Animal_Dead.Clear();
        Animal_Escape.Clear();
        Animal_Eaten.Clear();
        //store list of "AnimalSpawnData" from the case
        if(CurrentHuntingCase == null)
        {
            Debug.LogWarning("case is null");
        }
        AnimalSpawnData_List = new List<AnimalSpawnData>(CurrentHuntingCase.All_AnimalSpawnData);
        //store max timer and current timer from the case
        MaxTimer = CurrentHuntingCase.TimerDuration;
        CurrentTimer = MaxTimer;
        TickTimer = 0f;
        CurrentTimer_GoUp = 0f;
        HuntingUI.Toggle_Active_Timer(true);
        HuntingUI.Update_TimerValue(CurrentTimer);
        //initialize the index of the current "AnimalSpawnData"
        Current_AnimalSpawnData_Index = 0;
        //change hunting game state to "Active"
        Update_HuntingGameState(Enum_HuntingGameState.Active);
        //enable hunting bow
        HuntingBow.Setup_Bow();
        HuntingBow.Update_BowState(Enum_BowState.Active);
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
                HuntingUI.Update_TimerValue(CurrentTimer);
                //check if we need to spawn any animal right now
                CheckSpawnAnimal();
                //check if the timer has run out
                CheckOutOfTime();
            }
        }
    }

    //This spawns animals depending on the timer
    private void CheckSpawnAnimal()
    {
        while (Current_AnimalSpawnData_Index < AnimalSpawnData_List.Count && CurrentTimer_GoUp >= AnimalSpawnData_List[Current_AnimalSpawnData_Index].SpawnTime)
        {
            AnimalSpawnData currentAnimalSpawnData = AnimalSpawnData_List[Current_AnimalSpawnData_Index];
            //Spawn animal
            GameObject currentSpawnedAnimal = null;
            currentSpawnedAnimal = Instantiate(currentAnimalSpawnData.Animal_Prefab, currentAnimalSpawnData.SpawnPosition, currentAnimalSpawnData.Animal_Prefab.transform.rotation);
            // if(CurrentLevel_SpawnedAnimals != null && CurrentLevel_SpawnedAnimals.Count > 0)
            // {
            //     //use the animals that was spawned previously. Should only be used if reloading the same level
            //     currentSpawnedAnimal = CurrentLevel_SpawnedAnimals[Current_AnimalSpawnData_Index];
            //     currentSpawnedAnimal.transform.SetPositionAndRotation(currentAnimalSpawnData.SpawnPosition, currentAnimalSpawnData.Animal_Prefab.transform.rotation);
            // }
            // else
            // {
            //     //create a new animal. Should be used if NOT reloading the level
            //     currentSpawnedAnimal = Instantiate(currentAnimalSpawnData.Animal_Prefab, currentAnimalSpawnData.SpawnPosition, currentAnimalSpawnData.Animal_Prefab.transform.rotation);
            // }
            
            if(SpawnedAnimals == null)
            {
                SpawnedAnimals = new List<GameObject>();
            }

            SpawnedAnimals.Add(currentSpawnedAnimal);
            //give reference of this case manager to the spawned animal
            currentSpawnedAnimal.GetComponent<Animal_AI_Base>().SetupAnimal_Complete(this, currentAnimalSpawnData);
            //increment index
            Current_AnimalSpawnData_Index++;
        }
    }

    //TODO This spawns animals depending on conditions such as whether animal X is dead
    //or animal X escape + animal Y died
    private void CheckSpawnAnimal_Condition()
    {
        //when spawning animal, don't forget to include the animal spawn data
        //also need to keep track of which animal has been spawned using conditions
        //We'll use this data to determine if all animals spawned with conditions have been spawned or not
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
        //do not register any more status when the game state is inactive
        if(CurrentHuntingGameState == Enum_HuntingGameState.Inactive)
        {
            return;
        }

        //Note: an animal needs to die before it can be eaten
        switch(newStatus)
        {
            case AnimalStatus.Dead:
                //add the animal to the list of dead animals
                Animal_Dead.Add(theAnimal);
                //TODO check if we need to spawn an animal

                //check win and lose condition
                Check_WinOrLose_Condition();
                break;
            case AnimalStatus.Escaped:
                //add the animal to the list of escaped animals
                Animal_Escape.Add(theAnimal);
                //TODO check if we need to spawn an animal

                //check win and lose condition
                Check_WinOrLose_Condition();
                break;
            case AnimalStatus.Eaten:
                //remove the animal from the dead animals list
                Animal_Dead.Remove(theAnimal);
                //add the animal to the list of eaten animals
                Animal_Eaten.Add(theAnimal);
                //TODO check if we need to spawn an animal

                //check win and lose condition
                Check_WinOrLose_Condition();
                break;
            case AnimalStatus.Alive:
            default:
                //nothing for now
                break;
        }

        //TODO might want to check if there are any animals left to spawn
        //if there isn't and no animals are alive, we can stop the timer
        //and check for the win/lose condtion timer varian since we already checked the non-timer variant above us

    }

    public void Check_WinOrLose_Condition()
    {
        if(CurrentHuntingGameState != Enum_HuntingGameState.Inactive)
        {
            bool winCondition_Met = CurrentHuntingCase.WinCondition_Immediate_MultiChain.Get_MultiChain_ConditionStatus(Animal_Dead, Animal_Escape, Animal_Eaten);
            if(winCondition_Met == true)
            {
                WinEvent();
                return;
            }

            bool loseCondition_Met = CurrentHuntingCase.LoseCondition_Immediate_MultiChain.Get_MultiChain_ConditionStatus(Animal_Dead, Animal_Escape, Animal_Eaten);
            if(loseCondition_Met == true)
            {
                LoseEvent();
                return;
            }
        }
    }

    public void Check_WinOrLose_Condition_Timer()
    {
        bool winCondition_Met = CurrentHuntingCase.WinCondition_Timer_MultiChain.Get_MultiChain_ConditionStatus(Animal_Dead, Animal_Escape, Animal_Eaten);
        if(winCondition_Met == true)
        {
            WinEvent();
            return;
        }

        bool loseCondition_Met = CurrentHuntingCase.LoseCondition_Timer_MultiChain.Get_MultiChain_ConditionStatus(Animal_Dead, Animal_Escape, Animal_Eaten);
        if(loseCondition_Met == true)
        {
            LoseEvent();
            return;
        }
    }

    //call this function if the timer runs out
    public void Lose_OutOfTime()
    {
        //check if the win or lose condition is met (timer variant)
        Check_WinOrLose_Condition_Timer();

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
        HuntingUI.Toggle_Active_Timer(false);
        Update_HuntingGameState(Enum_HuntingGameState.Inactive);
        //disable hunting bow
        HuntingBow.Update_BowState(Enum_BowState.NonActive);
        //stop all animal actions
        StopAllAnimals();
        //disable arrows
        ObjectPoolerManager.ReturnAllWeaponHitboxes();
        //enable mouse
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //open win panel
        HuntingUI.Toggle_Active_WinPanel(true);
        //Debug.LogWarning("Win event");
    }

    private void LoseEvent()
    {
        //stop the timer
        //change hunting game state to "Active"
        HuntingUI.Toggle_Active_Timer(false);
        Update_HuntingGameState(Enum_HuntingGameState.Inactive);
        //disable hunting bow
        HuntingBow.Update_BowState(Enum_BowState.NonActive);
        //stop all animal actions
        StopAllAnimals();
        //disable arrows
        ObjectPoolerManager.ReturnAllWeaponHitboxes();
        //enable mouse
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //Open lose panel
        HuntingUI.Toggle_Active_LosePanel(true);
        //Debug.LogWarning("Lose event");
    }

    public void Update_HuntingGameState(Enum_HuntingGameState newState)
    {
        if(CurrentHuntingGameState == newState)
        {
            return;
        }

        CurrentHuntingGameState = newState;
    }

    public bool Check_AreThereMoreAnimalsToSpawn()
    {
        //bool to check if animals spawned based on timer are all spawned
        bool timerSpawnAnimal_Done = Current_AnimalSpawnData_Index >= AnimalSpawnData_List.Count;
        //TODO bool to check if animals spawned based on conditions (dead/escaped/eaten animals) are all spawned

        //TODO temporary return value, delete later
        return timerSpawnAnimal_Done == false;
    }

    public bool IsCurrentLevelTheLastLevel()
    {
        int maxLevel = HuntingCase_Data.GetLevelAmountInRank(CurrentRank);
        return CurrentLevel == maxLevel;
    }

    public void StopAllAnimals()
    {
        for(int i = 0; i < SpawnedAnimals.Count; i++)
        {
            SpawnedAnimals[i].GetComponent<Animal_AI_Base>().StopAndDeleteAction();
        }
    }

    public string GetLootData_String()
    {
        string theData = "";
        //go through all spawned animals; Get the loot from the dead ones
        for(int i = 0; i < SpawnedAnimals.Count; i++)
        {
            Animal_AI_Base theAnimal = SpawnedAnimals[i].GetComponent<Animal_AI_Base>();
            if(theAnimal.GetAnimalStatus() == AnimalStatus.Dead)
            {
                if(string.IsNullOrEmpty(theData))
                {
                    //first loot data
                    theData += theAnimal.GetLootData_String();
                }
                else
                {
                    //NOT first loot data
                    theData += ", " + theAnimal.GetLootData_String();
                }
            }
        }

        return theData;
    }
}
