using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class HuntingBow : MonoBehaviour
{
    [SerializeField] private BowStat_SO BowStat_All;
    [SerializeField] private BowStat_Single CurrentBowStat;
    [Space(10)]
    //TODO only used for testing
    [SerializeField] private int TestJobRank;
    [Space(10)]
    [SerializeField] private BowCooldownUI BowCooldownUI;
    //reference to hunting cursor
    [SerializeField] private HuntingCursor_Base HuntingCursor;
    [Space(10)]
    [SerializeField] private bool IsOffCooldown;    //if true, that means the bow is not in cooldown state and can shoot
    //controls if the bow is able to move and shoot arrows or not; Does not manage cooldown state, that's someone else's job
    [SerializeField] private Enum_BowState BowState;
    [Space(10)]
    [SerializeField] private GameObject Bow_GO;
    [SerializeField] private float Bow_X_Offset;
    [SerializeField] private float Bow_Y_Offset;
    [Space(10)]
    [SerializeField] private float Pos_X_Min;
    [SerializeField] private float Pos_X_Max;
    [SerializeField] private float Pos_Y_Min;
    [SerializeField] private float Pos_Y_Max;
    

    [Space(10)]
    [SerializeField] private ArrowPooler ArrowPooler;
    //only a placeholder, need to replace with a hunting arrow object pooler manager or some sorts
    //[SerializeField] private HuntingArrow TestArrow;

    //bow rotation fields
    private Vector3 bowDirection;
    private Quaternion bowLookDirection;

    //bow shoot coolddown timer fields
    private float maxCooldownTimer;
    private float currentCooldownTimer;

    public void Setup_Bow()
    {
        //TODO setup bow stat according to current rank, only for testing
        CurrentBowStat = BowStat_All.BowStats[TestJobRank];
        //setup the cooldown timer
        maxCooldownTimer = CurrentBowStat.HitCooldown;
        currentCooldownTimer = maxCooldownTimer;
        IsOffCooldown = true;
        //hide cooldown UI
        BowCooldownUI.Toggle_Active_GO(false);
        //end of testing
    }

    //on update, if bow state is active, then follow the mouse position with positional offset
    private void Update()
    {
        //if bow is active, follow the mouse position with an offset
        if(BowState == Enum_BowState.Active)
        {
            // Vector3 mousePos_Screen = Input.mousePosition;
            // mousePos_Screen.z = Bow_GO.transform.position.z - Camera.main.transform.position.z;

            //Vector3 mousePos_World = Camera.main.ScreenToWorldPoint(mousePos_Screen);
            Vector3 mousePos_World = HuntingCursor.Get_HuntingCursor_Pos();

            //get mouse pos in range of 0 to 1 for x and y position
            // Vector2 mousePos_Normalized = new Vector2();
            // mousePos_Normalized.x = mousePos_Screen.x / Screen.width;
            // mousePos_Normalized.y = mousePos_Screen.y / Screen.height;
            
            Vector3 bowPos = new Vector3();
            bowPos.x = mousePos_World.x;
            bowPos.y = mousePos_World.y;
            // bowPos.x = Mathf.Lerp(Pos_X_Min, Pos_X_Max, mousePos_Normalized.x);
            // bowPos.y = Mathf.Lerp(Pos_Y_Min, Pos_Y_Max, mousePos_Normalized.y);


            bowPos.z = Bow_GO.transform.position.z;
            bowPos.x -= Bow_X_Offset;
            bowPos.y -= Bow_Y_Offset;

            //clamp the bow's x and y position
            bowPos.x = Mathf.Clamp(bowPos.x, Pos_X_Min, Pos_X_Max);
            bowPos.y = Mathf.Clamp(bowPos.y, Pos_Y_Min, Pos_Y_Max);

            Bow_GO.transform.position = bowPos;

            //bow rotation
            //mousePos_Screen.z = 10f - Camera.main.transform.position.z;
            //Vector3 arrowTargetPos = Camera.main.ScreenToWorldPoint(mousePos_Screen);

            Vector3 arrowTargetPos = Camera.main.WorldToScreenPoint(HuntingCursor.Get_HuntingCursor_Pos());
            arrowTargetPos.z = 10f - Camera.main.transform.position.z;
            arrowTargetPos = Camera.main.ScreenToWorldPoint(arrowTargetPos);
            arrowTargetPos.z = 10f;

            bowDirection = (arrowTargetPos - Bow_GO.transform.position).normalized;
            bowLookDirection = Quaternion.LookRotation(bowDirection);

            Bow_GO.transform.rotation = bowLookDirection * Quaternion.Euler(0, -90, 0);

            //tick down bow shoot cooldown if needed
            if(IsOffCooldown == false)
            {
                currentCooldownTimer -= Time.deltaTime;
                //update cooldown UI visual according to the timer
                BowCooldownUI.Update_Slider_Visual((maxCooldownTimer-currentCooldownTimer)/maxCooldownTimer);
                if(currentCooldownTimer <= 0f)
                {
                    //cooldown finished
                    IsOffCooldown = true;
                    currentCooldownTimer = maxCooldownTimer;
                    //hide cooldown UI
                    BowCooldownUI.Toggle_Active_GO(false);
                }
            }

            //check for left mouse button click
            if(Input.GetMouseButtonDown(0) && IsOffCooldown == true)
            {
                //activate bow shoot cooldown
                IsOffCooldown = false;
                //show cooldown UI
                BowCooldownUI.Toggle_Active_GO(true);

                //TODO only for testing; later on, we need to check if the bow is cooling down or not before authorizing an arrow launch
                // mousePos_Screen.z = 10f - Camera.main.transform.position.z;

                // Vector3 arrowTargetPos = Camera.main.ScreenToWorldPoint(mousePos_Screen);
                // arrowTargetPos.z = 10f;

                //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // Vector3 rayDirection = (HuntingCursor.Get_HuntingCursor_Pos() - Camera.main.transform.position).normalized;
                // Ray ray = new Ray(Camera.main.transform.position, rayDirection);
                // Vector3 travelDir_After = ray.direction.normalized;

                Vector3 travelDir_After = (HuntingCursor.Get_HuntingCursor_Pos() - Camera.main.transform.position).normalized;

                HuntingArrow theArrow = ArrowPooler.GetHuntingArrow();
                theArrow.Setup_PoolerReference(ArrowPooler);
                theArrow.Setup_AttackStats(CurrentBowStat.ArrowStats);
                theArrow.ActivateArrow();
                //TODO start pos might not be "bowPos"
                theArrow.StartArrowMovement(bowPos, arrowTargetPos, CurrentBowStat.HitDelay, travelDir_After);
            }
        }
        



    }
    
    public void Update_BowState(Enum_BowState newState)
    {
        if(BowState == newState)
        {
            return;
        }

        BowState = newState;
        //edit state of hunting cursor as well
        HuntingCursor.Update_BowState(newState);

        switch(BowState)
        {
            case Enum_BowState.NonActive:
                IsOffCooldown = true;
                //hide cooldown UI
                currentCooldownTimer = maxCooldownTimer;
                BowCooldownUI.Toggle_Active_GO(false);
                break;
            default:
                break;
        }
        
    }


    public void Set_BowRank(int newRank)
    {
        TestJobRank = newRank;
    }

}
