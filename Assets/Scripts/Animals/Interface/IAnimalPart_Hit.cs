using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAnimalPart_Hit
{
    bool MakeWeaponDisappear { get; }
    int HitPriority { get; }
    Collider BodyPartCollider { get; }
    void OnHit(HuntingAttackStats_SO attackData);
}
