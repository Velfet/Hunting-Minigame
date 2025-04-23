using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalFinishAction_Damage : AnimalFinishAction_Base
{
    //different damage multiplier for body and head hit
    public float DamageMultiplier;
    
    public override void Activate_FinishAction(AnimalAction_ActivateData animalAction_ActivateData)
    {
        //get reference to the animal that this finish action belongs to
        Animal_AI_Base theAnimal = animalAction_ActivateData.TheAnimal;
        //deal damage to the animal
        float damageAmount = animalAction_ActivateData.AttackData.AttackDamage * DamageMultiplier;
        Debug.LogWarning("Damage amount: " + damageAmount);
        theAnimal.DamageAnimal((int) damageAmount);
    }
}
