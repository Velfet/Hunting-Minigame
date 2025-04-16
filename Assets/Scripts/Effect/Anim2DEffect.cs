using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim2DEffect : MonoBehaviour
{
    [SerializeField] private Animator Animator;

    [SerializeField] private string AnimClipName;
    
    public void PlayEffectAnim()
    {
        Animator.Play(AnimClipName, 0, 0f);
    }
}
