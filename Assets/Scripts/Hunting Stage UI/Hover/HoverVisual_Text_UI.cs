using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HoverVisual_Text_UI : MonoBehaviour
{
    [SerializeField] private CursorManager CursorManager;
    [Space(10)]
    [SerializeField] private Transform tooltipGO;
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private RectTransform textRectTransform;
    [Space(10)]
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform Canvas_UI_RT;

    Camera mainCamera;
    private Vector2 offsetPos;
    private float textBoxSizeX, textBoxSizeY;
    Vector2 mousePosWorldPoint, mousePosScreen;

    Vector2 screenPos_Offset;


    public void Disable_HoverVisual()
    {
        tooltipGO.gameObject.SetActive(false);
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        UpdatePosition();
    }

    public void SetHoverText(string theText)
    {
        if(tooltipGO.gameObject.activeInHierarchy == false)
        {
            tooltipGO.gameObject.SetActive(true);
        }

        textMesh.text = theText;
        UpdatePosition();
    }

    public void SetPositionOffset(Vector2 newOffset)
    {
        screenPos_Offset = newOffset;
    }



    private void UpdatePosition()
    {
        if (tooltipGO.gameObject.activeInHierarchy == false)
        {
            return;
        }
        //update position to follow active cursor's position; may need to make some sort of "CursorManager" class
        //get cursor screen position
        mousePosScreen = CursorManager.Get_CurrentCursor_ScreenPos();
        mousePosScreen += screenPos_Offset;
        //get cursor world position
        mousePosWorldPoint = CursorManager.Get_CurrentCursor_WorldPos();
        //mousePosWorldPoint = mainCamera.ScreenToWorldPoint(mousePosScreen);

        //get point in the canvas, canvas should be the same as screen size
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            Canvas_UI_RT,
            mousePosScreen,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPoint
        );

        //get half size of the hover visual box
        Vector2 halfSize = textRectTransform.sizeDelta * 0.5f;

        //get the variables to clamp the x and y position
        float clampX = Canvas_UI_RT.sizeDelta.x * 0.5f - halfSize.x;
        float clampY = Canvas_UI_RT.sizeDelta.y * 0.5f - halfSize.y;

        //clamp the hover visual box
        localPoint.x = Mathf.Clamp(localPoint.x, -clampX, clampX);
        localPoint.y = Mathf.Clamp(localPoint.y, -clampY, clampY);
        textRectTransform.anchoredPosition = localPoint;

    }

    


}
