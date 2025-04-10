using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponHitSource
{
    HuntingAttackStats_SO GetWeaponHitStats();
    void Set_GameObject_Active(bool newState);
}
