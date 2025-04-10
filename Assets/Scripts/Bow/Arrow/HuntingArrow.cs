using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class HuntingArrow : MonoBehaviour
{
    [SerializeField] private Transform Arrow_Transform;

    private IEnumerator ArrowMoveRoutine;




    public void InterruptArrowMovement()
    {
        if(ArrowMoveRoutine != null)
        {
            StopCoroutine(ArrowMoveRoutine);
            ArrowMoveRoutine = null;
        }
    }

    public void StartArrowMovement(Vector3 startPos, Vector3 endPos, float travelDuration)
    {
        InterruptArrowMovement();

        ArrowMoveRoutine = MoveArrow(startPos, endPos, travelDuration, BowConst.ArrowParabolaHeight);
        StartCoroutine(ArrowMoveRoutine);
    }

    private IEnumerator MoveArrow(Vector3 startPos, Vector3 endPos, float travelDuration, float parabolaHeight)
    {
        float arrowPeakTime = BowConst.Arrow_PeakTime;

        float currentTimeNormalized = 0f;
        Arrow_Transform.position = startPos;

        Vector3 lastArrowPos = startPos;

        Vector3 travelDirection = (endPos - startPos).normalized;
        Vector3 arcNormalDirection = new Vector3(-travelDirection.y, travelDirection.x, 0f);

        while(currentTimeNormalized < 1f)
        {
            currentTimeNormalized += Time.deltaTime / travelDuration;

            //Get parabola position
            Vector3 currentArrowPos = Vector3.Lerp(startPos, endPos, currentTimeNormalized);
            //float arrowHeight = parabolaHeight * 4f * (currentTimeNormalized - currentTimeNormalized * currentTimeNormalized);
            //float arrowHeight = GetArc(parabolaHeight, currentTimeNormalized);
            float arrowHeight = GetArc_CustomPeakTime(parabolaHeight, currentTimeNormalized, arrowPeakTime);

            float testHeight = GetArc_CustomPeakTime(1f, currentTimeNormalized, arrowPeakTime);
            //Debug.LogWarning("[testParabola] normalized time: " + currentTimeNormalized);
            //Debug.LogWarning("[testParabola] height (0 to 1): " + testHeight);
            currentArrowPos += arcNormalDirection * arrowHeight;

            //set position
            Arrow_Transform.position = currentArrowPos;
            //set scale
            Arrow_Transform.localScale = Vector3.one * (1f - 0.35f*(currentTimeNormalized));

            //get and set rotation of arrow
            Vector2 moveDirection = currentArrowPos - lastArrowPos;
            //Debug.LogWarning("[test] movedirection: " + moveDirection);
            if(currentTimeNormalized< 1f)
            {
                float angle = Mathf.Atan2(moveDirection.y, moveDirection.x);
                angle *= Mathf.Rad2Deg;
                //Debug.LogWarning("[test2] angle: " + angle);
                Arrow_Transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }
            

            //update last position
            lastArrowPos = currentArrowPos;

            yield return null;
        }

        //arrow has reached its destination
        Debug.LogWarning("Arrow has reached destination, might want to activate its collider now");
    }

    //parabola where the arc is at the middle
    public float GetArc(float parabolaHeight, float currentTimeNormalized)
    {
        float arc = parabolaHeight * 4f * (currentTimeNormalized - currentTimeNormalized * currentTimeNormalized);

        return arc;
    }

    //parabola where you can specify at which percentage of the journey the peak will be located at
    public float GetArc_CustomPeakTime(float parabolaHeight, float currentTimeNormalized, float peakTime)
    {
        // Create a normalized t value relative to peakTime
        float x;
        if (currentTimeNormalized < peakTime)
        {
            x = currentTimeNormalized / peakTime;
        }
        else
        {
            x = (1f - currentTimeNormalized) / (1f - peakTime);
        }

        return parabolaHeight * (1f - (1f - x) * (1f - x));
    }
}
