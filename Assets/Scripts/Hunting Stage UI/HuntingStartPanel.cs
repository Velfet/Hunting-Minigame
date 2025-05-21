using System.Collections;
using System.Collections.Generic;
using HuntingGame;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HuntingStartPanel : MonoBehaviour
{
    [SerializeField] private HuntingGameClient HuntingGameClient;
    [Space(10)]
    [SerializeField] private Button StartButton;
    [SerializeField] private Button TownButton;

    private bool hasStarted = false;


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
        //subscribe button function here
        StartButton.onClick.AddListener(StartLevel);
        TownButton.onClick.AddListener(GoTo_Town);
    }

    void OnDisable()
    {
        //unsubscribe button function here
        StartButton.onClick.RemoveAllListeners();
        TownButton.onClick.RemoveAllListeners();
    }
    
    //contact the hunting case manager to retry the current level
    private void StartLevel()
    {
        //play button click SFX
        AudioManager.Instance.PlayAudio(AudioConst.ButtonClick_SFX);

        Toggle_Active_State(false);

        //TODO set the rank using 
        //HuntingGameClient.SetRank(rankIntGoesHere);
        //before starting the game
        //or call this function instead
        //HuntingGameClient.Start_HuntingMinigame(rankIntGoesHere);

        HuntingGameClient.Start_TestCase();
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
    
    public void Toggle_Active_State(bool activeState)
    {
        gameObject.SetActive(activeState);

    }
}
