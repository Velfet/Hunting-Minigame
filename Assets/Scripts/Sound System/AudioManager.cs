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

        //TODO call this function to play audio
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

                if (theClip == null)
                {
                    Debug.LogWarning($"[AudioManager] audio asset ID {audioAssetID} does not have an AudioCLip");
                    return;
                }

                switch (soundType)
                {
                    case SoundType.SFX:
                        //use the audio source sfx to play the audio clip
                        AudioSource_SFX.PlayOneShot(theClip, soundAsset.soundVolume);
                        break;
                    case SoundType.BGM:
                        //Tuse the audio source bgm to play the audio clip
                        if (AudioSource_BGM.isPlaying)
                        {
                            AudioSource_BGM.Stop();
                        }
                        AudioSource_BGM.loop = true;
                        AudioSource_BGM.clip = theClip;
                        AudioSource_BGM.volume = soundAsset.soundVolume;
                        AudioSource_BGM.Play();
                        break;
                    case SoundType.UI:
                        //use the audio source UI to play the audio clip
                        AudioSource_UI.PlayOneShot(theClip, soundAsset.soundVolume);
                        break;
                    default:
                        Debug.LogWarning("[AudioManager] sound asset type does not belong to any in the switch case. Audio type is: " + soundType.ToString());
                        break;
                }
            }
        }

        public AudioClip GetAudioClip(string audioAssetID, out float theVolume)
        {
            //try to find the audio clip
            SoundAsset_Base soundAsset = SoundGroupRegistry.GetSoundAsset(audioAssetID);
            if (soundAsset == null)
            {
                Debug.LogWarning("[AudioManager] could not find the sound asset with ID: " + audioAssetID);
                theVolume = 0f;
                return null;
            }
            else
            {
                SoundType soundType = soundAsset.type;
                AudioClip theClip = soundAsset.GetClip();
                theVolume = soundAsset.soundVolume;
                return theClip;
            }
        }

    }
}

