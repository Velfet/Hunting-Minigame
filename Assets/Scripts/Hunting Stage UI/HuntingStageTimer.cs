using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading;

public class HuntingStageTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TimerText;
    [SerializeField] private int TimerValue;

    public void Update_TimerValue(float newValue)
    {
        TimerValue = Mathf.CeilToInt(newValue);
        TimerText.text = TimerValue.ToString();
    }

    public void Toggle_Active_Timer(bool activeState)
    {
        gameObject.SetActive(activeState);
    }
    
}
