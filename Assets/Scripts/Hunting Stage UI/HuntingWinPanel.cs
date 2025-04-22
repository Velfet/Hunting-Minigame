using System.Collections;
using System.Collections.Generic;
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
        if(HuntingCaseManager.IsCurrentLevelTheLastLevel() == true)
        {
            //disable continue button
            ContinueButton.gameObject.SetActive(false);
        }
        else
        {
            //enable continue button
            ContinueButton.gameObject.SetActive(true);
            ContinueButton.onClick.AddListener(ContinueToNextLevel);
        }
        
        TownButton.onClick.AddListener(GoTo_Town);

        //Update loot info text
        LootInfo.text = HuntingCaseManager.GetLootData_String();
    }

    void OnDisable()
    {
        //unsubscribe button function here
        ContinueButton.onClick.RemoveAllListeners();
        TownButton.onClick.RemoveAllListeners();
    }

    //contact the hunting case manager to retry the current level
    private void ContinueToNextLevel()
    {
        Toggle_Active_State(false);
        HuntingCaseManager.Load_NextLevel();
    }

    //TODO placeholder function, need to update the functionality
    private void GoTo_Town()
    {
        Toggle_Active_State(false);
        #if UNITY_EDITOR
            // Stop playing the scene in the editor
            EditorApplication.isPlaying = false;
        #else
            // Quit the application in a build
            Application.Quit();
        #endif
    }

    public void Toggle_Active_State(bool activeState)
    {
        gameObject.SetActive(activeState);
    }
}
