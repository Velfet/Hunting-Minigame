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

        //IF animal is not dead and the animal is not running away from predator
        if(theAnimal.GetAnimalStatus() == AnimalStatus.Alive && theAnimal.GetAnimalReactState() != AnimalReactState.RunFromPredator)
        {
            //alter the index of the specified animal
            int final_MasterStateIndex = MyMathUtils.AlterValue(theAnimal.GetMasterStateIndex(), AlterType_StateIndex, AlterValue_StateIndex);
            int final_StateActionIndex = MyMathUtils.AlterValue(theAnimal.GetStateIndex(), AlterType_ActionIndex, AlterValue_ActionIndex);

            //check the current animal action's priority. If it is higher than this finish action's priority, then don't alter the state
            //and don't trigger the next action
            //This means that: if the priority of both actions are the same value, then DO the NEW action
            AnimalAction_Base currentAnimalAction = theAnimal.GetCurrentAnimalAction();
            if(currentAnimalAction != null)
            {
                //compare the priority of the current animal's action and the priority of this finish action
                int currentActionPriority = currentAnimalAction.ActionPriority;
                if(currentActionPriority > FinishActionPriority)
                {
                    //Current action's priority is higher than this finish action's priority
                    //Do not alter the state of the animal and do not trigger the next action
                    Debug.LogWarning("the current action has a higher priority than this finish action. Not triggering the finish action.");
                    return;
                }
            }

            theAnimal.SetMasterStateIndex(final_MasterStateIndex);
            theAnimal.SetStateIndex(final_StateActionIndex);

            //also trigger the next action
            theAnimal.ActivateCurrentAction(animalAction_ActivateData.TheTargetAnimal);
        }
        
    }
}
