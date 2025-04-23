using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AnimalIdentity : IEquatable<AnimalIdentity>
{
    public string Name;
    public AnimalType AnimalType;
    public AnimalBehaviourType AnimalBehaviour;

    public bool Equals(AnimalIdentity other)
    {
        return Name == other.Name && AnimalType == other.AnimalType && AnimalBehaviour == other.AnimalBehaviour;
    }

    public override bool Equals(object obj)
    {
        return obj is AnimalIdentity other && Equals(other);
    }

    public override int GetHashCode()
    {
        return (Name, AnimalType, AnimalBehaviour).GetHashCode();
    }
}
