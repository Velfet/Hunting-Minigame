using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using HuntingGame;
using UnityEngine.UI;
using TMPro;

public class HuntingHintPanel : MonoBehaviour
{
    [SerializeField] private RectTransform HintPanel_RT;
    [SerializeField] private Vector2 Visible_Pos;
    [SerializeField] private Vector2 Non_Visible_Pos;
    [SerializeField] private float Toggle_Anim_Duration;
    [SerializeField] private bool IsVisibleState = false;
    [Space(10)]
    [SerializeField] private List<HuntingRule_SO> AllHuntingRuleData;
    private int MaxSlideIndex;
    [SerializeField] private int CurrentSlideIndex;
    [Space(10)]
    [SerializeField] private Button PreviousButton;
    [SerializeField] private Button NextButton;
    [Space(10)]
    [SerializeField] private Button RuleToggleButton;
    [SerializeField] private Sprite RuleToggle_ActiveSprite;
    [SerializeField] private Sprite RuleToggle_InactiveSprite;
    [Space(10)]
    [SerializeField] private TextMeshProUGUI TitleText;
    [SerializeField] private TextMeshProUGUI DescriptionText;

    public void Toggle_HintPanel()
    {
        //play button click SFX
        AudioManager.Instance.PlayAudio(AudioConst.RuleOpenAndClose_SFX);

        IsVisibleState = !IsVisibleState;

        //update Rule toggle button's visual depending if the Rule UI is visible or not
        SpriteState ruleToggle_SpriteState = RuleToggleButton.spriteState;
        if (IsVisibleState == true)
        {
            //rule UI is visible, use the active sprite
            RuleToggleButton.image.sprite = RuleToggle_ActiveSprite;
            ruleToggle_SpriteState.selectedSprite = RuleToggle_ActiveSprite;
        }
        else
        {
            //rule UI is not visible, use the inactive sprite
            RuleToggleButton.image.sprite = RuleToggle_InactiveSprite;
            ruleToggle_SpriteState.selectedSprite = RuleToggle_InactiveSprite;
        }
        RuleToggleButton.spriteState = ruleToggle_SpriteState;

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

    private void OnEnable()
    {
        //sub buttons
        PreviousButton.onClick.AddListener(GoTo_PreviousSlide);
        NextButton.onClick.AddListener(GoTo_NextSlide);

        //initialize with 1st slide
        if (AllHuntingRuleData != null && AllHuntingRuleData.Count > 0)
        {
            MaxSlideIndex = AllHuntingRuleData.Count - 1;
            UpdateSlideIndex(0, true);
        }

    }

    private void OnDisable()
    {
        //unsub buttons
        PreviousButton.onClick.RemoveAllListeners();
        NextButton.onClick.RemoveAllListeners();
    }

    private void GoTo_PreviousSlide()
    {
        //play button click SFX
        AudioManager.Instance.PlayAudio(AudioConst.ButtonClick_SFX);

        int newIndex = CurrentSlideIndex - 1;
        if (newIndex >= 0)
        {
            UpdateSlideIndex(newIndex);
        }
    }

    private void GoTo_NextSlide()
    {
        //play button click SFX
        AudioManager.Instance.PlayAudio(AudioConst.ButtonClick_SFX);

        int newIndex = CurrentSlideIndex + 1;
        if (newIndex <= MaxSlideIndex)
        {
            UpdateSlideIndex(newIndex);
        }
    }

    private void UpdateSlideIndex(int newIndex, bool isSetup = false)
    {
        int previousIndex = CurrentSlideIndex;

        if (newIndex == previousIndex && isSetup == false)
        {
            //no change to the index, do nothing
            return;
        }

        if (newIndex == 0)
        {
            //hide previous button
            PreviousButton.gameObject.SetActive(false);
        }

        if (newIndex == MaxSlideIndex)
        {
            //hide next button
            NextButton.gameObject.SetActive(false);
        }

        if (previousIndex < newIndex)
        {
            //show previous button
            PreviousButton.gameObject.SetActive(true);
        }
        else if (previousIndex > newIndex || newIndex != MaxSlideIndex)
        {
            //show next button
            NextButton.gameObject.SetActive(true);
        }

        //update the current index
        CurrentSlideIndex = newIndex;

        //update visual
        UpdateRulePanelVisual();
    }

    private void UpdateRulePanelVisual()
    {
        //update title and description text
        TitleText.text = AllHuntingRuleData[CurrentSlideIndex].RuleTitle_String;
        DescriptionText.text = AllHuntingRuleData[CurrentSlideIndex].RuleDetail_String;
    }


}
