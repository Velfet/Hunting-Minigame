using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BowCooldownUI : MonoBehaviour
{
    [SerializeField] private Slider CooldownSlider;


    public void Update_Slider_Visual(float fillPercentage)
    {
        CooldownSlider.value = fillPercentage;
    }

    public void Toggle_Active_GO(bool activeState)
    {
        gameObject.SetActive(activeState);
    }
}
