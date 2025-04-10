using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntingBow : MonoBehaviour
{
    
    [SerializeField] private BowStat_SO BowStat_All;
    [SerializeField] private BowStat_Single CurrentBowStat;
    //TODO only used for testing
    [SerializeField] private int TestJobRank;
    [Space(10)]
    //controls if the bow is able to move and shoot arrows or not; Does not manage cooldown state, that's someone else's job
    [SerializeField] private Enum_BowState BowState;
    [Space(10)]
    [SerializeField] private GameObject Bow_GO;
    [SerializeField] private float Bow_X_Offset;
    [SerializeField] private float Bow_Y_Offset;
    [Space(10)]
    //TODO only a placeholder, need to replace with a hunting arrow object pooler manager or some sorts
    [SerializeField] private HuntingArrow TestArrow;

    private void Awake()
    {
        //TODO setup bow stat according to current rank
        CurrentBowStat = BowStat_All.BowStats[TestJobRank];
    }

    //on update, if bow state is active, then follow the mouse position with positional offset
    private void Update()
    {
        //if bow is active, follow the mouse position with an offset
        if(BowState == Enum_BowState.Active)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10f;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            Vector3 bowPos = mousePos;
            bowPos.x -= Bow_X_Offset;
            bowPos.y -= Bow_Y_Offset;
            Bow_GO.transform.position = bowPos;

            //check for left mouse button click
            if(Input.GetMouseButtonDown(0))
            {
                //TODO only for testing; later on, we need to check if the bow is cooling down or not before authorizing an arrow launch
                TestArrow.StartArrowMovement(bowPos, mousePos, CurrentBowStat.HitDelay);
            }
        }
        



    }
    
}
