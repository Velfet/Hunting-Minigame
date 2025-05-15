using System.Collections;
using System.Collections.Generic;
using HuntingGame;
using UnityEngine;

[CreateAssetMenu(fileName = "SA_Group_", menuName = "ScriptableObjects/SoundAsset/Group")]
public class SoundAsset_Group : SoundAsset_Base
{
    public List<AudioClip> clips;

    public override AudioClip GetClip()
    {
        if (clips == null || clips.Count == 0)
        {
            Debug.LogWarning("[SoundAsset_Group] audio clip is null");
        }

        return clips[UnityEngine.Random.Range(0, clips.Count)];
    }
}
