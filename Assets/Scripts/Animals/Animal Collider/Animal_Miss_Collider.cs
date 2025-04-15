using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal_Miss_Collider : Animal_BodyPart_Collider_Base
{
    public override void OnHit(HuntingAttackStats_SO attackData)
    {
        //tell the animal that the aura was hit, which might startle the animal, altering its behaviour
        Debug.LogWarning("Miss aura reacts to being hit");
        //no need to transfer over the attack data for now
        TheAnimal.Trigger_MissAuraHit_Action();
    }
}
