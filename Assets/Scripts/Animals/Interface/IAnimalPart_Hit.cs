using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAnimalPart_Hit
{
    int HitPriority { get; }
    Collider2D BodyPartCollider { get; }
    void OnHit(HuntingAttackStats_SO attackData);

    //might want to change the parameter to a class that has the info of the attack, such as a
    //"AttackStats" class. We could also extract that class from the Collider2D I guess, so this isn't strictly necessary
}
