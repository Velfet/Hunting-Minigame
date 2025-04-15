using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalFinishAction_DamageAndAlterIndex : AnimalFinishAction_Base
{
    //different damage multiplier for body and head hit
    public float DamageMultiplier;
    [Space(10)]
    public int AlterValue_StateIndex;
    public Enum_AlterType AlterType_StateIndex;
    public int AlterValue_ActionIndex;
    public Enum_AlterType AlterType_ActionIndex;

    public override void Activate_FinishAction(AnimalAction_ActivateData animalAction_ActivateData)
    {
        //get reference to the animal that this finish action belongs to
        Animal_AI_Base theAnimal = animalAction_ActivateData.TheAnimal;
        //deal damage to the animal
        float damageAmount = animalAction_ActivateData.AttackData.AttackDamage * DamageMultiplier;
        Debug.LogWarning("Damage amount: " + damageAmount);
        theAnimal.DamageAnimal((int) damageAmount);

        //IF animal is not dead
        if(theAnimal.GetAnimalStatus() == AnimalStatus.Alive)
        {
            //alter the index of the specified animal
            int final_MasterStateIndex = MyMathUtils.AlterValue(theAnimal.GetMasterStateIndex(), AlterType_StateIndex, AlterValue_StateIndex);
            int final_StateActionIndex = MyMathUtils.AlterValue(theAnimal.GetStateIndex(), AlterType_ActionIndex, AlterValue_ActionIndex);
            theAnimal.SetMasterStateIndex(final_MasterStateIndex);
            theAnimal.SetStateIndex(final_StateActionIndex);

            //also trigger the next action
            theAnimal.ActivateCurrentAction();
        }
        
    }
}
