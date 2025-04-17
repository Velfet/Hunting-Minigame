using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Animal_HP_UI : MonoBehaviour
{
    [SerializeField] private Slider HP_Slider;
    [SerializeField] private GameObject HP_Slider_GO;
    [Space(10)]
    [SerializeField] private RectTransform MyTransform;
    [SerializeField] private RectTransform HP_Secondary_Slider;
    [Space(20)]
    [SerializeField] private float DamagedHealthSliderSpeed;
    [Space(20)]
    [SerializeField] private float ShakeDuration;
    [SerializeField] private float ShakeMagnitude;


    private IEnumerator SlideDamagedHealthRoutine;
    private IEnumerator ShakeRoutine;
    private WaitForSeconds ShakeRoutineDelay = new WaitForSeconds(0.025f);



    public void UpdateInstant_HP_Slider_Visual(float fillPercentage)
    {
        HP_Slider.value = fillPercentage;
        Update_SecondaryHP_Visual(fillPercentage);

        if(fillPercentage == 0)
        {
            HP_Slider_GO.SetActive(false);
        }
        else
        {
            HP_Slider_GO.SetActive(true);
        }
    }

    public void Update_HP_Slider_Visual(float fillPercentage)
    {
        HP_Slider.value = fillPercentage;

        if(fillPercentage == 0)
        {
            HP_Slider_GO.SetActive(false);
        }
        else
        {
            HP_Slider_GO.SetActive(true);
            //start coroutine to edit the secondary fill
            BeginSlideDamagedHealth(fillPercentage);

            BeginShake();
        }

    }

    private void BeginSlideDamagedHealth(float fillAmountTarget)
    {
        if(SlideDamagedHealthRoutine != null)
        {
            StopCoroutine(SlideDamagedHealthRoutine);
            SlideDamagedHealthRoutine = null;
        }

        SlideDamagedHealthRoutine = SlideDamagedHealth(fillAmountTarget);
        StartCoroutine(SlideDamagedHealthRoutine);
    }

    private void BeginShake()
    {
        if(ShakeRoutine != null)
        {
            StopCoroutine(ShakeRoutine);
            ShakeRoutine = null;
        }

        ShakeRoutine = Shake();
        StartCoroutine(ShakeRoutine);
    }

    private IEnumerator SlideDamagedHealth(float fillAmountTarget)
    {
        float fillAmount;

        //approach the target until we're close enough
        while(Mathf.Abs(HP_Secondary_Slider.anchorMax.x - fillAmountTarget) > 0.02f)
        {
            fillAmount = Mathf.Lerp(HP_Secondary_Slider.anchorMax.x, fillAmountTarget, Time.deltaTime * DamagedHealthSliderSpeed);
            
            Update_SecondaryHP_Visual(fillAmount);
            
            yield return null;
        }

        Update_SecondaryHP_Visual(fillAmountTarget);
        yield break;

    }

    private IEnumerator Shake()
    {
        Vector3 originalPos = MyTransform.localPosition;

        float currentTime = 0f;

        float magnitudeMultiplier2 ;

        float shakeDurationDivCurrentTime;

        while(currentTime < ShakeDuration)
        {   
            shakeDurationDivCurrentTime = Mathf.Clamp((ShakeDuration/2)/currentTime, 1, 2);

            magnitudeMultiplier2 = shakeDurationDivCurrentTime;

            float xRand = Random.Range(-1f, 1f) * ShakeMagnitude * magnitudeMultiplier2;
            float yRand = Random.Range(-1f, 1f) * ShakeMagnitude * magnitudeMultiplier2;

            MyTransform.localPosition = new Vector3(originalPos.x + xRand, originalPos.y + yRand, originalPos.z);

            currentTime += 0.025f;

            yield return ShakeRoutineDelay;
        }

        MyTransform.localPosition = originalPos;

        yield break;
    }


    public void Update_SecondaryHP_Visual(float fillPercentage)
    {
        HP_Secondary_Slider.anchorMin = Vector2.zero;
        HP_Secondary_Slider.anchorMax = new Vector2(fillPercentage, 1);
        HP_Secondary_Slider.offsetMin = Vector2.zero;
        HP_Secondary_Slider.offsetMax = Vector2.zero;
    }
}
