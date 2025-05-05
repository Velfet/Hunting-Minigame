using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private HuntingCursor_Base HuntingCursor;
    [SerializeField] private CursorHover_WorldSpace_UI HuntingCursor_CursorHover;
    [SerializeField] private bool IsHuntingCursorActive;

    private bool hasStarted;
    private Camera mainCamera;


    private void Onable()
    {
        if(hasStarted == true)
        {
            Start_Or_OnEnable();
        }        
    }

    private void Start()
    {
        mainCamera = Camera.main;
        hasStarted = true;
        Start_Or_OnEnable();
    }

    private void Start_Or_OnEnable()
    {
        //subscribe to events
        HuntingCursor.On_UpdateCustomCursor_State += Set_ActiveState_HuntingCursor;
    }

    public void OnDisable()
    {
        //unsubscribe to events
        HuntingCursor.On_UpdateCustomCursor_State -= Set_ActiveState_HuntingCursor;
    }

    public void Set_ActiveState_HuntingCursor(bool newState)
    {
        IsHuntingCursorActive = newState;
    }

    public Vector3 Get_CurrentCursor_ScreenPos()
    {
        if(IsHuntingCursorActive == true)
        {
            return HuntingCursor_CursorHover.Get_HuntingCursor_ScreenPos();
        }
        else
        {
            //return mouse position
            return Input.mousePosition;
        }
    }

    public Vector3 Get_CurrentCursor_WorldPos()
    {
        if(IsHuntingCursorActive == true)
        {
            return HuntingCursor_CursorHover.Get_HuntingCursor_WorldPos();
        }
        else
        {
            //return mouse position
            return mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
    }


}
