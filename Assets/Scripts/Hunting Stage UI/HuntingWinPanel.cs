using System.Collections;
using System.Collections.Generic;
using HuntingGame;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class HuntingWinPanel : MonoBehaviour
{
    [SerializeField] private HuntingCaseManager HuntingCaseManager;
    [Space(10)]
    [SerializeField] private TextMeshProUGUI LootInfo;
    [SerializeField] private Button ContinueButton;
    [SerializeField] private Button TownButton;
    [SerializeField] private TextMeshProUGUI ContinueButton_Text;
    [Space(10)]
    [SerializeField] private TextMeshProUGUI CritHit_Text;
    [Space(10)]
    [SerializeField] private Color ContinueButtonText_ActiveColor;
    [SerializeField] private Color ContinueButtonText_NonactiveColor;
    [Space(10)]
    [SerializeField] private Button ReplayButton;
    [Space(10)]
    [SerializeField] private GameObject FlawlessVisual_GO;
    [Space(20)]
    [SerializeField] private GameObject ClickBlocker_GO;
    [SerializeField] private float ClickBlocker_Duration;

    private bool hasStarted = false;
    private IEnumerator ToggleClickBlocker_Routine;

    void Start()
    {
        hasStarted = true;
        OnEnable_Or_Start();
    }

    void OnEnable()
    {
        if (hasStarted == true)
        {
            //subscribe button function here
            OnEnable_Or_Start();
        }
    }

    void OnEnable_Or_Start()
    {
        //start coroutine to enable the click blocker for a certain amount of time
        Start_ClickBlockToggle();

        //subscribe button function here
        if (HuntingCaseManager.IsCurrentLevelTheLastLevel() == true)
        {
            //disable continue button
            ContinueButton.interactable = false;
            //ContinueButton.gameObject.SetActive(false);
            ContinueButton_Text.color = ContinueButtonText_NonactiveColor;
        }
        else
        {
            //enable continue button
            ContinueButton.interactable = true;
            //ContinueButton.gameObject.SetActive(true);
            ContinueButton.onClick.AddListener(ContinueToNextLevel);
            ContinueButton_Text.color = ContinueButtonText_ActiveColor;
        }

        //Update level icon UI
        HuntingCaseManager.Get_HuntingUIManager().Toggle_LevelIcon_Win(HuntingCaseManager.Get_CurrentLevel() + 1);

        //Update flawless visual display depending if the outcome of the level was flawless or not
        FlawlessVisual_GO.SetActive(HuntingCaseManager.IsGameOutcome_Flawless());

        TownButton.onClick.AddListener(GoTo_Town);

        if (ReplayButton != null)
        {
            ReplayButton.onClick.AddListener(RetryLevel);
        }


        //Update loot info text
        LootInfo.text = HuntingCaseManager.GetLootData_String();

        //Update crit text
        CritHit_Text.text = "x" + HuntingCaseManager.Get_CritHitAmount().ToString();

        //TODO use this list of enums to get the real loot later
        List<Enum_LootOptions> lootDrop = HuntingCaseManager.GetLootData_List();

        //TODO use this int data to get rank experience from dead animals (note, you don't get experience from animals that have been eaten)
        int experienceAmount = HuntingCaseManager.GetExperienceFromDeadAnimals();
    }

    void OnDisable()
    {
        //stop the toggle click blocker coroutine
        Interrupt_ClickBlockToggle();

        //unsubscribe button function here
        ContinueButton.onClick.RemoveAllListeners();
        TownButton.onClick.RemoveAllListeners();
        if (ReplayButton != null)
        {
            ReplayButton.onClick.RemoveAllListeners();
        }

    }

    //contact the hunting case manager to retry the current level
    private void ContinueToNextLevel()
    {
        //play button click SFX
        AudioManager.Instance.PlayAudio(AudioConst.ButtonClick_SFX);

        Toggle_Active_State(false);
        HuntingCaseManager.Load_NextLevel();
    }

    //TODO placeholder function, need to update the functionality
    private void GoTo_Town()
    {
        //play button click SFX
        AudioManager.Instance.PlayAudio(AudioConst.ButtonClick_SFX);

        Toggle_Active_State(false);
#if UNITY_EDITOR
        // Stop playing the scene in the editor
        EditorApplication.isPlaying = false;
#else
            // Quit the application in a build
            Application.Quit();
#endif
    }

    //contact the hunting case manager to retry the current level
    private void RetryLevel()
    {
        //play button click SFX
        AudioManager.Instance.PlayAudio(AudioConst.ButtonClick_SFX);

        Toggle_Active_State(false);
        HuntingCaseManager.ReloadCurrentLevel();
    }

    public void Toggle_Active_State(bool activeState)
    {
        gameObject.SetActive(activeState);

    }

    private void Interrupt_ClickBlockToggle()
    {
        //deactivate the click blocker now
        ClickBlocker_GO.SetActive(false);

        if (ToggleClickBlocker_Routine != null)
        {
            StopCoroutine(ToggleClickBlocker_Routine);
            ToggleClickBlocker_Routine = null;
        }
    }

    private void Start_ClickBlockToggle()
    {
        Interrupt_ClickBlockToggle();

        ToggleClickBlocker_Routine = ToggleClickBlocker();
        StartCoroutine(ToggleClickBlocker_Routine);
    }

    private IEnumerator ToggleClickBlocker()
    {
        //activate the click blocker
        ClickBlocker_GO.SetActive(true);

        float currentTime = 0f;

        while (currentTime < ClickBlocker_Duration)
        {
            yield return null;
            currentTime += Time.deltaTime;
        }

        //delay is done, deactivate the click blocker now
        ClickBlocker_GO.SetActive(false);
    }

}
