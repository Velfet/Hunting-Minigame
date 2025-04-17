using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnvironmentPart_Hit
{
    bool MakeWeaponDisappear { get; }
    int HitPriority { get; }    //unused for now
    Collider EnvironmentCollider { get; }
    void OnHit(HuntingAttackStats_SO attackData);
}
