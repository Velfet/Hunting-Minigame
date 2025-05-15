using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HuntingGame
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        [SerializeField] private SoundGroupRegistry SoundGroupRegistry;
        //audio source for SFX
        public AudioSource AudioSource_SFX;
        //audio source for BGM
        public AudioSource AudioSource_BGM;
        //audio source for UI
        public AudioSource AudioSource_UI;
        
        

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void PlayAudio(string audioAssetID)
        {
            //try to find the audio clip
            SoundAsset_Base soundAsset = SoundGroupRegistry.GetSoundAsset(audioAssetID);
            if (soundAsset == null)
            {
                Debug.LogWarning("[AudioManager] could not find the sound asset with ID: " + audioAssetID);
            }
            else
            {
                SoundType soundType = soundAsset.type;
                AudioClip theClip = soundAsset.GetClip();

                switch (soundType)
                {
                    case SoundType.SFX:
                        //TODO use the audio source sfx to play the audio clip
                        break;
                    case SoundType.BGM:
                        //Tuse the audio source bgm to play the audio clip
                        if (AudioSource_BGM.isPlaying)
                        {
                            AudioSource_BGM.Stop();
                        }
                        AudioSource_BGM.clip = theClip;
                        AudioSource_BGM.Play();
                        break;
                    case SoundType.UI:
                        //use the audio source UI to play the audio clip
                        AudioSource_UI.PlayOneShot(theClip);
                        break;
                    default:
                        Debug.LogWarning("[AudioManager] sound asset type does not belong to any in the switch case. Audio type is: " + soundType.ToString());
                        break;
                }
            }
        }

    }
}

