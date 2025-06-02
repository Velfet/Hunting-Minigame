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
    [SerializeField] private List<AnimalSpawnData_Condition> AnimalSpawnData_Condition_List_NotYetSpawned;
    [SerializeField] private List<AnimalSpawnData_Condition> AnimalSpawnData_Condition_List_AlreadySpawned;
    [Space(10)]
    [SerializeField] private List<AnimalSpawnData_Condition> AnimalSpawnData_WeirdCondition_List_NotYetSpawned;
    [SerializeField] private List<AnimalSpawnData_Condition> AnimalSpawnData_WeirdCondition_List_AlreadySpawned;

    [Space(10)]
    [SerializeField] private HuntingCase_SO CurrentHuntingCase;
    [SerializeField] private Enum_HuntingGameState CurrentHuntingGameState;
    [Space(10)]
    [SerializeField] private HuntingUIManager HuntingUI;
    [SerializeField] private float MaxTimer;
    [SerializeField] private float CurrentTimer;    //this timer goes from MaxTimer to 0
    [Space(10)]
    [SerializeField] private int CurrentLevel_CriticalHitCount;
    [Space(10)]
    [SerializeField] private List<GameObject> SpawnedAnimals;
    //[SerializeField] private List<GameObject> CurrentLevel_SpawnedAnimals;  //used for reloading levels; Unused
    [Space(10)]
    [SerializeField] private List<AnimalIdentity> Animal_Dead;
    [SerializeField] private List<AnimalIdentity> Animal_Escape;
    [SerializeField] private List<AnimalIdentity> Animal_Eaten;
    [SerializeField] private List<AnimalIdentity> Animal_Killed;    //gets populated when animal dies; Does not get removed from list if the animal was eaten
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
        if (TestCase != null)
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

        if (SpawnedAnimals != null)
        {
            for (int i = 0; i < SpawnedAnimals.Count; i++)
            {
                Destroy(SpawnedAnimals[i].gameObject);
            }
            SpawnedAnimals.Clear();
        }
    }

    public void Load_NextLevel()
    {
        Load_SpecifiedLevel(CurrentRank, CurrentLevel + 1);
    }

    //TODO call this function when wanting to load a specified level
    public void Load_SpecifiedLevel(int newRank, int newLevel)
    {
        //clear previous level's spawned animal list
        // if(CurrentLevel_SpawnedAnimals != null)
        // {
        //     CurrentLevel_SpawnedAnimals.Clear();
        // }

        if (SpawnedAnimals != null)
        {
            for (int i = 0; i < SpawnedAnimals.Count; i++)
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
        if (SpawnedAnimals != null)
        {
            for (int i = 0; i < SpawnedAnimals.Count; i++)
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

        //Update level icon UI
        HuntingUI.Toggle_LevelIcon(CurrentLevel);
        //clear the list of spawned animals
        SpawnedAnimals.Clear();
        //empty list of dead, escaped, and eaten animals
        Animal_Dead.Clear();
        Animal_Escape.Clear();
        Animal_Eaten.Clear();
        Animal_Killed.Clear();
        //set number of critical hits to 0
        Set_CriticalHitAmount(0);
        //store list of "AnimalSpawnData" from the case
        if (CurrentHuntingCase == null)
        {
            Debug.LogWarning("case is null");
        }
        AnimalSpawnData_List = new List<AnimalSpawnData>(CurrentHuntingCase.All_AnimalSpawnData);
        //store list of "AnimalSpawnData_Condition" from the case
        AnimalSpawnData_Condition_List_NotYetSpawned = new List<AnimalSpawnData_Condition>(CurrentHuntingCase.All_AnimalSpawnData_Condition);
        //empty the list of "AnimalSpawnData_Condition" that refers to animals that has been spawned
        AnimalSpawnData_Condition_List_AlreadySpawned = new List<AnimalSpawnData_Condition>();
        //store the list of animal spawn data that need to be spawned by weird condition
        AnimalSpawnData_WeirdCondition_List_NotYetSpawned = new List<AnimalSpawnData_Condition>(CurrentHuntingCase.All_AnimalSpawnData_WeirdCondition);
        //empty the list of animal that is spawned by weird conditions
        AnimalSpawnData_WeirdCondition_List_AlreadySpawned = new List<AnimalSpawnData_Condition>();
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
        if (CurrentHuntingGameState == Enum_HuntingGameState.Active)
        {
            if (CurrentTimer_GoUp == 0f && TickTimer == 0f)
            {
                //tick down timer
                TickTimer += Time.deltaTime;
                //check if we need to spawn any animal right now
                CheckSpawnAnimal();
                return;
            }

            //tick down timer
            TickTimer += Time.deltaTime;
            if (TickTimer >= TickRate)
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

            if (SpawnedAnimals == null)
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

    //This spawns animals depending on conditions such as whether animal X is dead
    //or animal X escape + animal Y died
    private void CheckSpawnAnimal_Condition()
    {
        //when spawning animal, don't forget to include the animal spawn data
        //also need to keep track of which animal has been spawned using conditions
        //We'll use this data to determine if all animals spawned with conditions have been spawned or not

        List<AnimalSpawnData_Condition> spawnedAnimalData = new List<AnimalSpawnData_Condition>();

        HuntingCondition_Arguments theArgument = new HuntingCondition_Arguments();
        theArgument.Animal_Die = Animal_Dead;
        theArgument.Animal_Escape = Animal_Escape;
        theArgument.Animal_BeEaten = Animal_Eaten;
        theArgument.Animal_BeKilled = Animal_Killed;

        for (int i = 0; i < AnimalSpawnData_Condition_List_NotYetSpawned.Count; i++)
        {
            AnimalSpawnData_Condition currentSpawnData = AnimalSpawnData_Condition_List_NotYetSpawned[i];
            //check if the condition has been met to spawn this animal

            if (currentSpawnData.SpawnCondition_MultiChain.Get_MultiChain_ConditionStatus(theArgument) == true)
            {
                //spawn the animal
                GameObject currentSpawnedAnimal = null;
                currentSpawnedAnimal = Instantiate(currentSpawnData.Animal_Prefab, currentSpawnData.SpawnPosition, currentSpawnData.Animal_Prefab.transform.rotation);

                if (SpawnedAnimals == null)
                {
                    SpawnedAnimals = new List<GameObject>();
                }

                SpawnedAnimals.Add(currentSpawnedAnimal);
                //give reference of this case manager to the spawned animal
                currentSpawnedAnimal.GetComponent<Animal_AI_Base>().SetupAnimal_Complete(this, currentSpawnData);
                //add the animal to the "spawnedAnimalData" list;
                //we'll use that list to remove the animal(s) from the
                //"not yet spawned" list and add them to the "already spawned list"
                spawnedAnimalData.Add(currentSpawnData);
            }

        }


        //use the data in the "spawnedAnimalData" list
        //to remove the data from the "not yet spawned list"
        //and add it to the "already spawned list"
        for (int i = 0; i < spawnedAnimalData.Count; i++)
        {
            //remove from "not yet spawned list"
            AnimalSpawnData_Condition_List_NotYetSpawned.Remove(spawnedAnimalData[i]);
            //add to "already spawned list"
            AnimalSpawnData_Condition_List_AlreadySpawned.Add(spawnedAnimalData[i]);
        }
    }

    private void CheckSpawnAnimal_WeirdCondition(HuntingCondition_Arguments theArguments)
    {
        List<AnimalSpawnData_Condition> spawnedAnimalData = new List<AnimalSpawnData_Condition>();

        for (int i = 0; i < AnimalSpawnData_WeirdCondition_List_NotYetSpawned.Count; i++)
        {
            AnimalSpawnData_Condition currentSpawnData = AnimalSpawnData_WeirdCondition_List_NotYetSpawned[i];
            //check if the condition has been met to spawn this animal
            if (currentSpawnData.SpawnCondition_MultiChain.Get_MultiChain_ConditionStatus(theArguments) == true)
            {
                //spawn the animal
                GameObject currentSpawnedAnimal = null;
                currentSpawnedAnimal = Instantiate(currentSpawnData.Animal_Prefab, currentSpawnData.SpawnPosition, currentSpawnData.Animal_Prefab.transform.rotation);

                if (SpawnedAnimals == null)
                {
                    SpawnedAnimals = new List<GameObject>();
                }

                SpawnedAnimals.Add(currentSpawnedAnimal);
                //give reference of this case manager to the spawned animal
                currentSpawnedAnimal.GetComponent<Animal_AI_Base>().SetupAnimal_Complete(this, currentSpawnData);
                //add the animal to the "spawnedAnimalData" list;
                //we'll use that list to remove the animal(s) from the
                //"not yet spawned" list and add them to the "already spawned list"
                spawnedAnimalData.Add(currentSpawnData);
            }

        }


        //use the data in the "spawnedAnimalData" list
        //to remove the data from the "not yet spawned list"
        //and add it to the "already spawned list"
        for (int i = 0; i < spawnedAnimalData.Count; i++)
        {
            //remove from "not yet spawned list"
            AnimalSpawnData_WeirdCondition_List_NotYetSpawned.Remove(spawnedAnimalData[i]);
            //add to "already spawned list"
            AnimalSpawnData_WeirdCondition_List_AlreadySpawned.Add(spawnedAnimalData[i]);
        }
    }

    private void CheckOutOfTime()
    {
        if (CurrentTimer <= 0f)
        {
            //timer ran out, trigger the lose event
            Lose_OutOfTime();
        }
    }


    public void Report_IsEating(AnimalIdentity theEatingAnimal)
    {
        HuntingCondition_Arguments arguments = new HuntingCondition_Arguments();
        arguments.Animal_IsEating = new List<AnimalIdentity>();
        arguments.Animal_IsEating.Add(theEatingAnimal);
        //check if we need to spawn an animal
        CheckSpawnAnimal_WeirdCondition(arguments);
    }

    //function for animals to report their status
    public void ReportStatus(AnimalIdentity theAnimal, AnimalStatus newStatus)
    {
        //do not register any more status when the game state is inactive
        if (CurrentHuntingGameState == Enum_HuntingGameState.Inactive)
        {
            return;
        }

        //Note: an animal needs to die before it can be eaten
        switch (newStatus)
        {
            case AnimalStatus.Dead:
                //add the animal to the list of dead animals
                Animal_Dead.Add(theAnimal);
                //add the animal to the list of killed animals
                Animal_Killed.Add(theAnimal);
                //check if we need to spawn an animal
                CheckSpawnAnimal_Condition();
                //check win and lose condition
                Check_WinOrLose_Condition();
                break;
            case AnimalStatus.Escaped:
                //add the animal to the list of escaped animals
                Animal_Escape.Add(theAnimal);
                //check if we need to spawn an animal
                CheckSpawnAnimal_Condition();
                //check win and lose condition
                Check_WinOrLose_Condition();
                break;
            case AnimalStatus.Eaten:
                //remove the animal from the dead animals list
                Animal_Dead.Remove(theAnimal);
                //add the animal to the list of eaten animals
                Animal_Eaten.Add(theAnimal);
                //check if we need to spawn an animal
                CheckSpawnAnimal_Condition();
                //check win and lose condition
                Check_WinOrLose_Condition();
                break;
            case AnimalStatus.Alive:
            default:
                //nothing for now
                break;
        }

        //check if there are any animals left to spawn
        if (Check_AreThereMoreAnimalsToSpawn() == false)
        {
            Debug.LogWarning("[TestA] no more animals to spawn apparently");
            //there aren't anymore animals to spawn
            //check if any animals are still alive; if not, we can stop the timer
            //and check for the win/lose condtion timer varian since we already checked the non-timer variant above us
            if (Check_NoAnimalAlive() == true)
            {
                //not a single animal has the "Alive" status
                OnNoMoreAnimalAlive();
            }

        }


    }

    public void Check_WinOrLose_Condition()
    {
        if (CurrentHuntingGameState != Enum_HuntingGameState.Inactive)
        {
            HuntingCondition_Arguments theArgument = new HuntingCondition_Arguments();
            theArgument.Animal_Die = Animal_Dead;
            theArgument.Animal_Escape = Animal_Escape;
            theArgument.Animal_BeEaten = Animal_Eaten;
            theArgument.Animal_BeKilled = Animal_Killed;

            bool winCondition_Met = CurrentHuntingCase.WinCondition_Immediate_MultiChain.Get_MultiChain_ConditionStatus(theArgument);
            if (winCondition_Met == true)
            {
                WinEvent();
                return;
            }

            bool loseCondition_Met = CurrentHuntingCase.LoseCondition_Immediate_MultiChain.Get_MultiChain_ConditionStatus(theArgument);
            if (loseCondition_Met == true)
            {
                LoseEvent();
                return;
            }
        }
    }

    public void Check_WinOrLose_Condition_Timer()
    {
        if (CurrentHuntingGameState != Enum_HuntingGameState.Inactive)
        {
            HuntingCondition_Arguments theArgument = new HuntingCondition_Arguments();
            theArgument.Animal_Die = Animal_Dead;
            theArgument.Animal_Escape = Animal_Escape;
            theArgument.Animal_BeEaten = Animal_Eaten;
            theArgument.Animal_BeKilled = Animal_Killed;

            bool winCondition_Met = CurrentHuntingCase.WinCondition_Timer_MultiChain.Get_MultiChain_ConditionStatus(theArgument);
            if (winCondition_Met == true)
            {
                WinEvent();
                return;
            }

            bool loseCondition_Met = CurrentHuntingCase.LoseCondition_Timer_MultiChain.Get_MultiChain_ConditionStatus(theArgument);
            if (loseCondition_Met == true)
            {
                LoseEvent();
                return;
            }
        }

    }

    //call this function if the timer runs out
    public void Lose_OutOfTime()
    {
        //check if the win or lose condition is met (timer variant)
        Check_WinOrLose_Condition_Timer();

        //timer has run out, trigger lose condition
        if (CurrentHuntingGameState != Enum_HuntingGameState.Inactive)
        {
            LoseEvent();
        }
    }

    private void OnNoMoreAnimalAlive()
    {
        //every animal is dead
        //try to check for win/lose condition
        Check_WinOrLose_Condition();
        Check_WinOrLose_Condition_Timer();

        //if no event has been triggered, default to lose event
        if (CurrentHuntingGameState != Enum_HuntingGameState.Inactive)
        {
            Debug.LogWarning("[TestA] No more animal alive, trigger lose event");
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
        if (CurrentHuntingGameState == newState)
        {
            return;
        }

        CurrentHuntingGameState = newState;
    }

    public bool Check_AreThereMoreAnimalsToSpawn()
    {
        HuntingCondition_Arguments theArgument = new HuntingCondition_Arguments();
        theArgument.Animal_Die = Animal_Dead;
        theArgument.Animal_Escape = Animal_Escape;
        theArgument.Animal_BeEaten = Animal_Eaten;
        theArgument.Animal_BeKilled = Animal_Killed;
        //also add the list of animal identities that are alive
        List<AnimalIdentity> aliveAnimals = new List<AnimalIdentity>();
        for (int i = 0; i < SpawnedAnimals.Count; i++)
        {
            Animal_AI_Base theAnimal = SpawnedAnimals[i].GetComponent<Animal_AI_Base>();
            if (theAnimal != null && theAnimal.GetAnimalStatus() == AnimalStatus.Alive)
            {
                aliveAnimals.Add(theAnimal.GetAnimalIdentity());
            }
        }
        theArgument.Animal_IsAlive = aliveAnimals;

        List<AnimalSpawnData_Condition> toBeRemoved_List = new List<AnimalSpawnData_Condition>();
        List<AnimalSpawnData_Condition> notYetSpawned_Copy;

        //A. bool to check if animals spawned based on timer are all spawned
        bool timerSpawnAnimal_Done = Current_AnimalSpawnData_Index >= AnimalSpawnData_List.Count;
        //B. bool to check if animals spawned based on conditions (dead/escaped/eaten animals) are all spawned
        toBeRemoved_List = new List<AnimalSpawnData_Condition>();
        notYetSpawned_Copy = new List<AnimalSpawnData_Condition>(AnimalSpawnData_Condition_List_NotYetSpawned);
        for (int i = 0; i < notYetSpawned_Copy.Count; i++)
        {
            AnimalSpawnData_Condition currentSpawnData = notYetSpawned_Copy[i];
            //find out if the condition can still be fulfilled
            bool canBeSpawned = currentSpawnData.SpawnCondition_MultiChain.Can_MultiChain_StillBeFulfilled(theArgument);
            if (canBeSpawned == false)
            {
                toBeRemoved_List.Add(currentSpawnData);
            }
        }

        //remove from the "notYetSpawned_Copy" list according to the "toBeRemoved_List"
        for (int i = 0; i < toBeRemoved_List.Count; i++)
        {
            notYetSpawned_Copy.Remove(toBeRemoved_List[i]);
        }


        bool conditionSpawnAnimal_Done = notYetSpawned_Copy.Count == 0;
        //old
        //bool conditionSpawnAnimal_Done = AnimalSpawnData_Condition_List_NotYetSpawned.Count == 0;

        //C. bool to check if animals spawned based on weird condition (animal A started to eat) are all spawned
        toBeRemoved_List = new List<AnimalSpawnData_Condition>();
        notYetSpawned_Copy = new List<AnimalSpawnData_Condition>(AnimalSpawnData_WeirdCondition_List_NotYetSpawned);
        for (int i = 0; i < notYetSpawned_Copy.Count; i++)
        {
            AnimalSpawnData_Condition currentSpawnData = notYetSpawned_Copy[i];
            //find out if the condition can still be fulfilled
            bool canBeSpawned = currentSpawnData.SpawnCondition_MultiChain.Can_MultiChain_StillBeFulfilled(theArgument);
            if (canBeSpawned == false)
            {
                toBeRemoved_List.Add(currentSpawnData);
            }
        }

        //remove from the "notYetSpawned_Copy" list according to the "toBeRemoved_List"
        for (int i = 0; i < toBeRemoved_List.Count; i++)
        {
            notYetSpawned_Copy.Remove(toBeRemoved_List[i]);
        }

        bool weirdConditionSpawnAnimal_Done = notYetSpawned_Copy.Count == 0;
        //old
        //bool weirdConditionSpawnAnimal_Done = AnimalSpawnData_WeirdCondition_List_NotYetSpawned.Count == 0;

        return timerSpawnAnimal_Done == false || conditionSpawnAnimal_Done == false || weirdConditionSpawnAnimal_Done == false;
    }

    public bool Check_NoAnimalAlive()
    {
        for (int i = 0; i < SpawnedAnimals.Count; i++)
        {
            Animal_AI_Base theAnimal = SpawnedAnimals[i].GetComponent<Animal_AI_Base>();
            if (theAnimal.GetAnimalStatus() == AnimalStatus.Alive)
            {
                //there is at least one animal that is alive, return false
                return false;
            }
        }

        //if we got to this point, then that means not a single animal is alive, return true
        return true;
    }

    public bool IsCurrentLevelTheLastLevel()
    {
        int maxLevel = HuntingCase_Data.GetLevelAmountInRank(CurrentRank);
        return CurrentLevel == maxLevel;
    }

    public void StopAllAnimals()
    {
        for (int i = 0; i < SpawnedAnimals.Count; i++)
        {
            SpawnedAnimals[i].GetComponent<Animal_AI_Base>().StopAndDeleteAction();
        }
    }

    public string GetLootData_String()
    {
        string theData = "";
        //go through all spawned animals; Get the loot from the dead ones
        for (int i = 0; i < SpawnedAnimals.Count; i++)
        {
            Animal_AI_Base theAnimal = SpawnedAnimals[i].GetComponent<Animal_AI_Base>();
            if (theAnimal.GetAnimalStatus() == AnimalStatus.Dead)
            {
                if (string.IsNullOrEmpty(theData))
                {
                    //first loot data
                    theData += theAnimal.GetAnimalData().Name;
                }
                else
                {
                    //NOT first loot data
                    theData += ", " + theAnimal.GetAnimalData().Name;
                }
            }
        }

        return theData;
    }

    //TODO call this to get the list of loot in enum form
    public List<Enum_LootOptions> GetLootData_List()
    {
        List<Enum_LootOptions> lootData = new List<Enum_LootOptions>();

        //go through all spawned animals; Get the loot from the dead ones
        for (int i = 0; i < SpawnedAnimals.Count; i++)
        {
            Animal_AI_Base theAnimal = SpawnedAnimals[i].GetComponent<Animal_AI_Base>();
            if (theAnimal.GetAnimalStatus() == AnimalStatus.Dead)
            {
                lootData.Add(theAnimal.GetLootData());
            }
        }

        return lootData;
    }

    public int GetExperienceFromDeadAnimals()
    {
        //go through all spawned animals; Get the experience from the dead ones
        int totalExperience = 0;
        for (int i = 0; i < SpawnedAnimals.Count; i++)
        {
            Animal_AI_Base theAnimal = SpawnedAnimals[i].GetComponent<Animal_AI_Base>();
            if (theAnimal.GetAnimalStatus() == AnimalStatus.Dead)
            {
                totalExperience += theAnimal.GetAnimalExp();
            }
        }

        //detect if game outcome is flawless; if so, add flawless experience to the totla experience
        if (IsGameOutcome_Flawless() == true)
        {
            //formula: (3 * current rank) + (2 * current level)
            int flawlessExperience = (3 * CurrentRank) + (2 * CurrentLevel);
            totalExperience += flawlessExperience;
        }
        

        return totalExperience;
    }

    public bool IsGameOutcome_Flawless()
    {
        //get number of animals in the level
        int totalAnimalInTheLevel = 0;
        if (CurrentHuntingCase.All_AnimalSpawnData != null)
        {
            totalAnimalInTheLevel += CurrentHuntingCase.All_AnimalSpawnData.Count;
        }

        if (CurrentHuntingCase.All_AnimalSpawnData_Condition != null)
        {
            totalAnimalInTheLevel += CurrentHuntingCase.All_AnimalSpawnData_Condition.Count;
        }

        if (CurrentHuntingCase.All_AnimalSpawnData_WeirdCondition != null)
        {
            totalAnimalInTheLevel += CurrentHuntingCase.All_AnimalSpawnData_WeirdCondition.Count;
        }

        //get the number of animals that are dead in the level; Does not include animals that have been eaten
        int totalAnimal_Dead = 0;
        for (int i = 0; i < SpawnedAnimals.Count; i++)
        {
            Animal_AI_Base theAnimal = SpawnedAnimals[i].GetComponent<Animal_AI_Base>();
            if (theAnimal.GetAnimalStatus() == AnimalStatus.Dead)
            {
                totalAnimal_Dead++;
            }
        }


        bool result = totalAnimal_Dead == totalAnimalInTheLevel;
        return result;
    }

    public HuntingUIManager Get_HuntingUIManager()
    {
        return HuntingUI;
    }

    public int Get_CurrentLevel()
    {
        return CurrentLevel;
    }

    public void Update_CritHitAmount(int addAmount)
    {
        CurrentLevel_CriticalHitCount += addAmount;
    }

    public void Set_CriticalHitAmount(int newAmount)
    {
        CurrentLevel_CriticalHitCount = newAmount;
    }

    public int Get_CritHitAmount()
    {
        return CurrentLevel_CriticalHitCount;
    }


}
