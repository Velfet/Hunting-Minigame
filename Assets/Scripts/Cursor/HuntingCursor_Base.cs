using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntingCursor_Base : MonoBehaviour
{
    [SerializeField] private GameObject Cursor_GO;
    [Space(10)]
    [SerializeField] private Enum_BowState BowState;
    [Space(10)]
    [SerializeField] private CursorHover_WorldSpace_UI CursorHover;
    [Space(10)]
    [SerializeField] private float MoveSpeed;
    [Space(10)]
    [SerializeField] private float Pos_X_Min;
    [SerializeField] private float Pos_X_Max;
    [SerializeField] private float Pos_Y_Min;
    [SerializeField] private float Pos_Y_Max;


    public event Action<bool> On_UpdateCustomCursor_State;

    private void Update()
    {
        //Ver 1
        // Vector3 mousePos_Screen = Input.mousePosition;
        // mousePos_Screen.z = Cursor_GO.transform.position.z - Camera.main.transform.position.z;

        // Vector3 mousePos_World = Camera.main.ScreenToWorldPoint(mousePos_Screen);
        
        // Vector3 bowPos = new Vector3();
        // bowPos.x = mousePos_World.x;
        // bowPos.y = mousePos_World.y;

        // bowPos.z = Cursor_GO.transform.position.z;

        // //clamp the cursor's x and y position
        // bowPos.x = Mathf.Clamp(bowPos.x, Pos_X_Min, Pos_X_Max);
        // bowPos.y = Mathf.Clamp(bowPos.y, Pos_Y_Min, Pos_Y_Max);

        //Cursor_GO.transform.position = bowPos;
        //End of ver 1


        if(BowState == Enum_BowState.Active)
        {
            float mouseX = Input.GetAxis("Mouse X"); // horizontal movement
            float mouseY = Input.GetAxis("Mouse Y"); // vertical movement

            // move object along X and Y axes based on mouse movement
            Vector3 movement = new Vector3(mouseX, mouseY, 0f);
            Vector3 newPos = Cursor_GO.transform.position + (movement * MoveSpeed);
            //clamp the position
            newPos.x = Mathf.Clamp(newPos.x, Pos_X_Min, Pos_X_Max);
            newPos.y = Mathf.Clamp(newPos.y, Pos_Y_Min, Pos_Y_Max);
            //apply the position to the cursor
            Cursor_GO.transform.position = newPos;
        }
        
    }

    public Vector3 Get_HuntingCursor_Pos()
    {
        return Cursor_GO.transform.position;
    }

    public void Update_BowState(Enum_BowState newState)
    {
        if(BowState == newState)
        {
            return;
        }

        BowState = newState;
        
        //show the hunting cursor if the state is active
        //and hide the hunting cursor if the state is non-active
        switch(BowState)
        {
            case Enum_BowState.Active:
                CursorHover.Set_ActiveState(true);
                On_UpdateCustomCursor_State?.Invoke(true);
                gameObject.SetActive(true);
                break;
            case Enum_BowState.NonActive:
                CursorHover.Set_ActiveState(false);
                On_UpdateCustomCursor_State?.Invoke(false);
                gameObject.SetActive(false);
                break;
            default:
                break;
        }
        
    }
}
