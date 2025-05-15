using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HuntingGame
{
    public abstract class SoundAsset_Base : ScriptableObject
    {
        public string id;
        public SoundType type;
        public abstract AudioClip GetClip();
    }
}

