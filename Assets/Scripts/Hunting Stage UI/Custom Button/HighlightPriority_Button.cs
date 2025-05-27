using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//This button class prioritizes Highlight visual over Selected visual, but still places Pressed visual over Highlight visual
public class HighlightPriority_Button : Button
{
    private bool isPointerOver = false;
    private bool isPointerDown = false;

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        //if pointer is inside, prioritize Highlighted visual over Selected visual but Pressed visual still gets priority over Highlighted visual
        if (IsHighlightedCustom())
        {
            base.DoStateTransition(SelectionState.Highlighted, instant);
        }
        else
        {
            base.DoStateTransition(state, instant);
        }

    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
        // Force re-evaluation
        EvaluateState();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
        // Force re-evaluation
        EvaluateState();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        isPointerDown = true;
        // Force re-evaluation
        EvaluateState();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        isPointerDown = false;
        // Force re-evaluation
        EvaluateState();
    }

    private void EvaluateState()
    {
        // Re-evaluate the current state to force the correct visuals
        DoStateTransition(currentSelectionState, false);
    }

    protected bool IsHighlightedCustom()
    {
        return IsActive() && IsInteractable() && isPointerOver && !isPointerDown;
    }

    protected override void InstantClearState()
    {
        base.InstantClearState();

        isPointerOver = false;
        isPointerDown = false;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        isPointerDown = false;
    }


}
