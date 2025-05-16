using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimalData_", menuName = "ScriptableObjects/Animal/AnimalData_SO")]
public class AnimalData_SO : ScriptableObject
{
    public string Name;
    public int Health;
    public float RunSpeed;
    public float RunSpeed_Faster;
    public float WalkSpeed;
    public float FallSpeed;
    public int Experience;
    public Enum_LootOptions LootData;
    public AnimalSoundName_SO AnimalSoundName;
}
