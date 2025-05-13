using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingReactionText : MonoBehaviour
{
    [SerializeField] private RectTransform MyRect;
    [SerializeField] private TextMeshProUGUI NumberText;

    [SerializeField] private Animator Animator;
    [SerializeField] private string AnimationClipName;

    private FloatingReactionText_Manager floatingTextManager;

    public void BeginFloatingTextAnim(string theText, Vector3 targetWorldPos, RectTransform canvasRect, FloatingReactionText_Manager newManager)
    {
        gameObject.SetActive(true);

        floatingTextManager = newManager;

        //set position
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, targetWorldPos);
        Vector2 canvasPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, Camera.main, out canvasPos);
        MyRect.anchoredPosition = canvasPos;

        //set text
        NumberText.text = theText;
        Animator.Play(AnimationClipName);
    }

    //call this through an Animation Event
    public void OnAnimFinished()
    {
        //return self to pooler, pooler should be the one to deactivate this object
        floatingTextManager.ReturnFloatingText(this);
    }
}
