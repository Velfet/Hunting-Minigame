using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalAnimationManager : MonoBehaviour
{
    [SerializeField] private Transform Visual_Transform;
    [SerializeField] private SpriteRenderer AnimalSprite;
    [SerializeField] private Animator Animator;
    [SerializeField] private AnimalAnimationKeys CurrentAnimation;
    [SerializeField] private bool IsFacingRight_CurrentStatus;
    [SerializeField] private bool DefaultFaceIsRight;
    [Space(20)]
    [SerializeField] private List<AnimalAnimationKeys> AnimalAnimationClipKeys;
    [SerializeField] private List<string> AnimalAnimationClipNames;
    //the "AnimalAnimationClipKeys" and "AnimalAnimationClipNames" needs to have the same value
    //protected bool hasStarted = false;
    private Dictionary<AnimalAnimationKeys, string> AnimalAnimationClips_Dictionary;


    private void Awake()
    {
        Construct_AnimalAnimationClips_Dictionary();
    }

    public void Start_Animation(AnimalAnimationKeys newAnimation)
    {
        if(newAnimation == CurrentAnimation)
        {
            //don't replay the animation that is already ongoing
            return;
        }

        if(Animator.enabled == false)
        {
            Animator.enabled = true;
        }

        Animator.Play(AnimalAnimationClips_Dictionary[newAnimation], 0, 0f);

        CurrentAnimation = newAnimation;
    }

    public void Start_Animation_JumpToEnd(AnimalAnimationKeys newAnimation)
    {
        if(newAnimation == CurrentAnimation)
        {
            //don't replay the animation that is already ongoing
            return;
        }

        if(Animator.enabled == false)
        {
            Animator.enabled = true;
        }

        if(AnimalAnimationClips_Dictionary == null)
        {
            Construct_AnimalAnimationClips_Dictionary();
        }

        Animator.Play(AnimalAnimationClips_Dictionary[newAnimation], 0, 1f);
        Animator.Update(0f);

        CurrentAnimation = newAnimation;
    }

    public void Stop_Animation()
    {
        Animator.enabled = false;
    }

    public void SetPlayerFaceDirection(bool isFacingRight)
    {
        if(isFacingRight == IsFacingRight_CurrentStatus)
        {
            return;
        }

        IsFacingRight_CurrentStatus = isFacingRight;

        if(isFacingRight == true)
        {
            //Face right
            if(DefaultFaceIsRight == true)
            {
                Visual_Transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                Visual_Transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
            
        }
        else
        {
            //Face left
            if(DefaultFaceIsRight == true)
            {
                Visual_Transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                Visual_Transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    private void Construct_AnimalAnimationClips_Dictionary()
    {
        AnimalAnimationClips_Dictionary = new Dictionary<AnimalAnimationKeys, string>();

        for(int i = 0; i < AnimalAnimationClipKeys.Count; i++)
        {
            AnimalAnimationClips_Dictionary[AnimalAnimationClipKeys[i]] = AnimalAnimationClipNames[i];
        }
    }

    public void ToggleAnimalVisualVisibility(bool isVisible)
    {
        if(isVisible == true)
        {
            AnimalSprite.enabled = true;
        }
        else
        {
            AnimalSprite.enabled = false;
        }
    }
}
