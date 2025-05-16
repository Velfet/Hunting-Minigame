using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HuntingGame
{
    public abstract class SoundAsset_Base : ScriptableObject
    {
        public string id;
        public SoundType type;
        [Range(0f, 1f)]
        public float soundVolume = 1f;
        public abstract AudioClip GetClip();
    }
}

