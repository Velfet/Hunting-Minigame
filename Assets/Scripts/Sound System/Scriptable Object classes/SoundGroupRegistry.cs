using System.Collections;
using System.Collections.Generic;
using HuntingGame;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundGroupRegistry", menuName = "ScriptableObjects/SoundAsset/Registry")]
public class SoundGroupRegistry : ScriptableObject
{
    public List<SoundAsset_Base> soundAssets;

    private Dictionary<string, SoundAsset_Base> soundAssetDic;


    public SoundAsset_Base GetSoundAsset(string assetID)
    {
        if (soundAssetDic == null)
        {
            Build_SoundAssetDictionnary();
        }

        return soundAssetDic.TryGetValue(assetID, out var theAsset) ? theAsset : null;
    }

    private void Build_SoundAssetDictionnary()
    {
        soundAssetDic = new Dictionary<string, SoundAsset_Base>();

        foreach (var soundAsset in soundAssets)
        {
            soundAssetDic.Add(soundAsset.id, soundAsset);
        }
    }



    #region unused for now
    // public AudioClip GetAudioClip(string assetID)
    // {
    //     SoundAsset_Base soundAsset = GetSoundAsset(assetID);

    //     if (soundAsset == null)
    //     {
    //         Debug.LogWarning("[SoundGroupRegistry] could not find the sound asset with ID: " + assetID);
    //         return null;
    //     }
    //     else
    //     {
    //         return soundAsset.GetClip();
    //     }
    // }
    #endregion

}
