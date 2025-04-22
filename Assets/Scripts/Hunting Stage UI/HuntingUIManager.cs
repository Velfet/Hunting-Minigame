using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntingUIManager : MonoBehaviour
{
    [SerializeField] private HuntingStageTimer StageTimer;
    [SerializeField] private HuntingLosePanel LosePanel;
    [SerializeField] private HuntingWinPanel WinPanel;


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
        WinPanel.Toggle_Active_State(activeState);
    }
    #endregion

}
