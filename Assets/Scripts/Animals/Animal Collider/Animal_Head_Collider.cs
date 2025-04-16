using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal_Head_Collider : Animal_BodyPart_Collider_Base
{
    public override void OnHit(HuntingAttackStats_SO attackData)
    {
        //tell the animal that the head was hit, which will then do damage to the animal and etc
        Debug.LogWarning("Head reacts to being hit");
        TheAnimal.Trigger_HeadHit_Action(attackData, hitPosition);
    }
}
