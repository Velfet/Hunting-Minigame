using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverManager_UI : MonoBehaviour
{
    [SerializeField] private HoverTarget_UI CurrentHoverTarget;
    [Space(10)]
    [SerializeField] private HoverVisual_Text_UI hoverVisual_Text_UI;


    public void Receive_HoverTarget(HoverTarget_UI hoveredObject)
    {
        //check if we are already hovering over the hoveredObject
        if(CurrentHoverTarget == hoveredObject)
        {
            return;
        }

        if(hoveredObject == null)
        {
            //tell the previous hover object that it is no longer being hovered on
            CurrentHoverTarget.OnStopBeingHovered();
        }

        //update current hover target
        CurrentHoverTarget = hoveredObject;
        //update visual
        if(CurrentHoverTarget == null)
        {
            //Debug.LogWarning("hover null");
            hoverVisual_Text_UI.Disable_HoverVisual();
        }
        else
        {
            //Debug.LogWarning("hover not null");
            hoverVisual_Text_UI.SetHoverText(CurrentHoverTarget.HoverText);
            //tell the current hover object that is is being hovered on
            CurrentHoverTarget.OnBeingHovered();
        }

    }


}
