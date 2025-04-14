using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntingCursor_Base : MonoBehaviour
{
    [SerializeField] private GameObject Cursor_GO;
    [Space(10)]
    [SerializeField] private float Pos_X_Min;
    [SerializeField] private float Pos_X_Max;
    [SerializeField] private float Pos_Y_Min;
    [SerializeField] private float Pos_Y_Max;

    private void Update()
    {
        Vector3 mousePos_Screen = Input.mousePosition;
        mousePos_Screen.z = Cursor_GO.transform.position.z - Camera.main.transform.position.z;

        Vector3 mousePos_World = Camera.main.ScreenToWorldPoint(mousePos_Screen);
        
        Vector3 bowPos = new Vector3();
        bowPos.x = mousePos_World.x;
        bowPos.y = mousePos_World.y;

        bowPos.z = Cursor_GO.transform.position.z;

        //clamp the cursor's x and y position
        bowPos.x = Mathf.Clamp(bowPos.x, Pos_X_Min, Pos_X_Max);
        bowPos.y = Mathf.Clamp(bowPos.y, Pos_Y_Min, Pos_Y_Max);

        Cursor_GO.transform.position = bowPos;
    }

    public Vector3 Get_HuntingCursor_Pos()
    {
        return Cursor_GO.transform.position;
    }
}
