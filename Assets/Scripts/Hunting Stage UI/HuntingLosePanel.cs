using System.Collections;
using System.Collections.Generic;
using HuntingGame;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HuntingLosePanel : MonoBehaviour
{
    [SerializeField] private HuntingCaseManager HuntingCaseManager;
    [Space(10)]
    [SerializeField] private Button RetryButton;
    [SerializeField] private Button TownButton;
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
        RetryButton.onClick.AddListener(RetryLevel);
        TownButton.onClick.AddListener(GoTo_Town);
    }

    void OnDisable()
    {
        //stop the toggle click blocker coroutine
        Interrupt_ClickBlockToggle();

        //unsubscribe button function here
        RetryButton.onClick.RemoveAllListeners();
        TownButton.onClick.RemoveAllListeners();
    }

    //contact the hunting case manager to retry the current level
    private void RetryLevel()
    {
        //play button click SFX
        AudioManager.Instance.PlayAudio(AudioConst.ButtonClick_SFX);

        Toggle_Active_State(false);
        HuntingCaseManager.ReloadCurrentLevel();
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
