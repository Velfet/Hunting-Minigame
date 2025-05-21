using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntingUIManager : MonoBehaviour
{
    [SerializeField] private HuntingStageTimer StageTimer;
    [SerializeField] private HuntingLosePanel LosePanel;
    [SerializeField] private HuntingWinPanel WinPanel;
    [SerializeField] private LevelIconManager LevelIconManager;
    [SerializeField] private float ActivateDelayDuration;
    [Space(10)]
    [SerializeField] private FloatingReactionText_Manager FloatingReactionText_Manager;


    private IEnumerator DelayActivateWinPanel_Coroutine;


    #region stage timer function
    public void Toggle_Active_Timer(bool activeState)
    {
        StageTimer.Toggle_Active_Timer(activeState);
    }
    public void Update_TimerValue(float currentTime)
    {
        StageTimer.Update_TimerValue(currentTime);
    }
    #endregion

    #region  lose panel function
    public void Toggle_Active_LosePanel(bool activeState)
    {
        LosePanel.Toggle_Active_State(activeState);
    }
    #endregion

    #region  win panel function
    public void Toggle_Active_WinPanel(bool activeState)
    {
        if(activeState == false)
        {
            WinPanel.Toggle_Active_State(false);
        }
        else
        {
            //function to start coroutine
            Start_DelayActivatePanel();
        }
    }

    private void Interrupt_DelayActivatePanel()
    {
        if(DelayActivateWinPanel_Coroutine != null)
        {
            StopCoroutine(DelayActivateWinPanel_Coroutine);
            DelayActivateWinPanel_Coroutine = null;
        }
    }

    private void Start_DelayActivatePanel()
    {
        Interrupt_DelayActivatePanel();

        //set and start the coroutine
        DelayActivateWinPanel_Coroutine = DelayActivatePanel(ActivateDelayDuration);
        StartCoroutine(DelayActivateWinPanel_Coroutine);
    }

    private IEnumerator DelayActivatePanel(float delayDuration)
    {
        float currentTime = 0f;

        while(currentTime < delayDuration)
        {
            yield return null;
            currentTime += Time.deltaTime;
        }

        //delay is done, activate the panel now
        WinPanel.Toggle_Active_State(true);
    }
    #endregion

    #region level icon
    public void Toggle_LevelIcon(int amount)
    {
        LevelIconManager.ActivateImages(amount);
    }
    #endregion

    #region floating text
    public void Activate_FloatingText(string theText, Vector3 worldPos)
    {
        FloatingReactionText_Manager.ActivateFloatingText(theText, worldPos);
    }
    #endregion

}
