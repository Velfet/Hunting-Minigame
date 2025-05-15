using System.Collections;
using System.Collections.Generic;
using HuntingGame;
using UnityEngine;

[CreateAssetMenu(fileName = "SA_Single_", menuName = "ScriptableObjects/SoundAsset/Single")]
public class SoundAsset_Single : SoundAsset_Base
{
    public AudioClip clip;

    public override AudioClip GetClip()
    {
        if (clip == null)
        {
            Debug.LogWarning("[SoundAsset_Single] audio clip is null");
        }

        return clip;
    }
}
