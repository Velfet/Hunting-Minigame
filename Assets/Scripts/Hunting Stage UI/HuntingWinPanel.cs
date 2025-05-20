using System.Collections;
using System.Collections.Generic;
using HuntingGame;
using TMPro;
using UnityEditor;
using UnityEngine;
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
    [SerializeField] private Color ContinueButtonText_ActiveColor;
    [SerializeField] private Color ContinueButtonText_NonactiveColor;
    [Space(10)]
    [SerializeField] private Button ReplayButton;

    private bool hasStarted = false;


    void Start()
    {
        hasStarted = true;
        OnEnable_Or_Start();
    }

    void OnEnable()
    {
        if(hasStarted == true)
        {
            //subscribe button function here
            OnEnable_Or_Start();
        }
    }

    void OnEnable_Or_Start()
    {
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
        
        TownButton.onClick.AddListener(GoTo_Town);

        if (ReplayButton != null)
        {
            ReplayButton.onClick.AddListener(RetryLevel);
        }
        

        //Update loot info text
        LootInfo.text = HuntingCaseManager.GetLootData_String();

        //TODO use this list of enums to get the real loot later
        List<Enum_LootOptions> lootDrop = HuntingCaseManager.GetLootData_List();
    }

    void OnDisable()
    {
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

}
