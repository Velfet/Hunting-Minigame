using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading;

public class HuntingStageTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TimerText;
    [SerializeField] private int TimerValue;

    public void UpdateTimerValue(float newValue)
    {
        TimerValue = Mathf.CeilToInt(newValue);
        TimerText.text = TimerValue.ToString();
    }
    
}
