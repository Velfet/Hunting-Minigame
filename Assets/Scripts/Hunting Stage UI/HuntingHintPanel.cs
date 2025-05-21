using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using HuntingGame;

public class HuntingHintPanel : MonoBehaviour
{
    [SerializeField] private RectTransform HintPanel_RT;
    [SerializeField] private Vector2 Visible_Pos;
    [SerializeField] private Vector2 Non_Visible_Pos;
    [SerializeField] private float Toggle_Anim_Duration;
    [SerializeField] private bool IsVisibleState = false;

    public void Toggle_HintPanel()
    {
        //play button click SFX
        AudioManager.Instance.PlayAudio(AudioConst.ButtonClick_SFX);
        
        IsVisibleState = !IsVisibleState;

        //decide on the duration of the tween based on current distance to target position
        Vector2 targetPos = IsVisibleState ? Visible_Pos : Non_Visible_Pos;
        float maxDistance = Vector2.Distance(Visible_Pos, Non_Visible_Pos);
        float distance = Vector2.Distance(HintPanel_RT.anchoredPosition, targetPos);
        float finalDuration = (maxDistance > 0f) 
        ? (distance / maxDistance) * Toggle_Anim_Duration 
        : 0f;

        //stop previous tween
        HintPanel_RT.DOKill(); 
        //do the tween to show or hide the hint panel
        HintPanel_RT.DOAnchorPos(targetPos, finalDuration).SetEase(Ease.InOutCubic);

    }
}
