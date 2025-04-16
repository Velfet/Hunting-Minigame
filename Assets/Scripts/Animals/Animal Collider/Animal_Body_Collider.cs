using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal_Body_Collider : Animal_BodyPart_Collider_Base
{
    public override void OnHit(HuntingAttackStats_SO attackData)
    {
        //tell the animal that the body was hit, which will then do damage to the animal and etc
        Debug.LogWarning("Body reacts to being hit");
        TheAnimal.Trigger_BodyHit_Action(attackData, hitPosition);
    }
}
