using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ASN_", menuName = "ScriptableObjects/Animal/AnimalSoundName")]
public class AnimalSoundName_SO : ScriptableObject
{
    public string Hit_SoundID;
    public string CritHit_SoundID;
    public string Death_SoundID;
    public string Roar_SoundID;
    public string Eat_SoundID;
}
