using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorHover_WorldSpace_UI : MonoBehaviour
{
    [SerializeField] private HoverManager_UI HoverManager;
    [Space(10)]
    public Transform customCursorWorld;  // Your world-space cursor
    public Canvas canvas;                // Your Screen Space - Overlay canvas
    public Camera mainCamera;           // Usually Camera.main
    public bool IsActive;

    private GraphicRaycaster raycaster;
    private EventSystem eventSystem;
    Vector3 screenPos;

    void Start()
    {
        raycaster = canvas.GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;
    }

    // Update is called once per frame
    void Update()
    {
        if(IsActive == true)
        {
            //get screen space from world space position of the hunting cursor
            //screenPos = RectTransformUtility.WorldToScreenPoint(mainCamera, customCursorWorld.position);
            screenPos = mainCamera.WorldToScreenPoint(customCursorWorld.position);
            Vector2 screenpos_v2 = screenPos;
            //make pointer event data for the newly made screen position
            PointerEventData pointerEventData = new PointerEventData(eventSystem)
            {
                position = screenpos_v2
            };

            //cast a ray using the pointer event data
            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerEventData, results);

            //go through the results and if we come across a hover target, tell the hover manager that we have found it
            bool foundHoverTarget = false;
            foreach(var result in results)
            {
                HoverTarget_UI hoverTarget = result.gameObject.GetComponent<HoverTarget_UI>();
                if(hoverTarget != null)
                {
                    foundHoverTarget = true;
                    HoverManager.Receive_HoverTarget(hoverTarget);

                    break;
                }
            }

            if(foundHoverTarget == false)
            {
                HoverManager.Receive_HoverTarget(null);
            }
        }
        

    }

    public void Set_ActiveState(bool newState)
    {
        IsActive = newState;
        
        //if new state is not active and the cursor was hovering on something before,
        //then we need to tell the hover manager that this hover is no longer active
        //so we should stop condidering the "something" from before to be hovered on
        if(IsActive == false)
        {
            HoverManager.Receive_HoverTarget(null);
        }

    }

    public Vector3 Get_HuntingCursor_ScreenPos()
    {
        return mainCamera.WorldToScreenPoint(customCursorWorld.position);
        //return RectTransformUtility.WorldToScreenPoint(mainCamera, customCursorWorld.position);
    }

    public Vector3 Get_HuntingCursor_WorldPos()
    {
        return customCursorWorld.position;
    }
}
