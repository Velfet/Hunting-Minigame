using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class HuntingArrow : MonoBehaviour
{
    [SerializeField] private Transform Arrow_Transform;
    //might want to replace with a Arrow_Collider script instead of a BoxCollider2D, or maybe it's not necessary for now, at least
    [SerializeField] private ArrowCollider Arrow_Collider;
    //set the stats of the arrow, such as how much dmg it deals

    private IEnumerator ArrowMoveRoutine;

    public void InterruptArrowMovement()
    {
        if(ArrowMoveRoutine != null)
        {
            StopCoroutine(ArrowMoveRoutine);
            ArrowMoveRoutine = null;
        }
    }

    public void ActivateArrow()
    {
        gameObject.SetActive(true);
    }

    public void Setup_AttackStats(HuntingAttackStats_SO newAttackStats)
    {
        Arrow_Collider.Setup_AttackStats(newAttackStats);
    }

    public void StartArrowMovement(Vector3 startPos, Vector3 endPos, float travelDuration, Vector3 direction_After)
    {
        InterruptArrowMovement();

        ArrowMoveRoutine = MoveArrow(startPos, endPos, travelDuration, BowConst.ArrowParabolaHeight, direction_After);
        StartCoroutine(ArrowMoveRoutine);
    }

    private IEnumerator MoveArrow(Vector3 startPos, Vector3 endPos, float travelDuration, float parabolaHeight, Vector3 direction_After)
    {
        //deactivate arrow's collider
        //Arrow_Collider.SetColliderState(false);

        //float arrowPeakTime = BowConst.Arrow_PeakTime;

        float currentTimeNormalized = 0f;
        Arrow_Transform.position = startPos;

        Vector3 lastArrowPos = startPos;

        Vector3 travelDirection = (endPos - startPos).normalized;
        Vector3 arcNormalDirection = new Vector3(-travelDirection.y, travelDirection.x, 0f);

        float totalDistance = Vector3.Distance(startPos, endPos);
        float desiredSpeed = totalDistance / travelDuration; // units per second

        while(currentTimeNormalized < 2f)
        {
            currentTimeNormalized += Time.deltaTime / travelDuration;

            //Get parabola position
            Vector3 currentArrowPos;
            if(currentTimeNormalized < 1f)
            {
                currentArrowPos = Vector3.Lerp(startPos, endPos, currentTimeNormalized);
            }
            else
            {
                //TODO add speed to the direction_After
                currentArrowPos = Arrow_Transform.position + (desiredSpeed * direction_After * Time.deltaTime);
                Debug.LogWarning("Speed: " + desiredSpeed);
            }
            //float arrowHeight = parabolaHeight * 4f * (currentTimeNormalized - currentTimeNormalized * currentTimeNormalized);
            //float arrowHeight = GetArc(parabolaHeight, currentTimeNormalized);
            //float arrowHeight = GetArc_CustomPeakTime(parabolaHeight, currentTimeNormalized, arrowPeakTime);

            //Debug.LogWarning("[testParabola] normalized time: " + currentTimeNormalized);
            //Debug.LogWarning("[testParabola] height (0 to 1): " + testHeight);
            //currentArrowPos += arcNormalDirection * arrowHeight;

            //set position
            Arrow_Transform.position = currentArrowPos;
            //set scale
            //Arrow_Transform.localScale = Vector3.one * (1f - 0.35f*(currentTimeNormalized));

            //get and set rotation of arrow
            Arrow_Transform.forward = travelDirection;
            // if(currentTimeNormalized < 1f)
            // {
            //     Arrow_Transform.forward = travelDirection;
            // }
            // else
            // {
            //     Arrow_Transform.forward = direction_After;
            // }
            
            // Vector2 moveDirection = currentArrowPos - lastArrowPos;
            // //Debug.LogWarning("[test] movedirection: " + moveDirection);
            // if(currentTimeNormalized< 1f)
            // {
            //     float angle = Mathf.Atan2(moveDirection.y, moveDirection.x);
            //     angle *= Mathf.Rad2Deg;
            //     //Debug.LogWarning("[test2] angle: " + angle);
            //     Arrow_Transform.rotation = Quaternion.Euler(0f, 0f, angle);
            // }
            

            //update last position
            lastArrowPos = currentArrowPos;

            yield return null;
        }



        //arrow has reached its destination
        //deactivate arrow's collider
        Arrow_Collider.Set_GameObject_Active(false);

        //Arrow_Collider.SetColliderState(true);
        //Debug.LogWarning("Arrow has reached destination, might want to activate its collider now");
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
