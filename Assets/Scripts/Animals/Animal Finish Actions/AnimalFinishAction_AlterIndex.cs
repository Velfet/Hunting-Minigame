using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalFinishAction_AlterIndex : AnimalFinishAction_Base
{
    [Space(10)]
    public int AlterValue_StateIndex;
    public Enum_AlterType AlterType_StateIndex;
    public int AlterValue_ActionIndex;
    public Enum_AlterType AlterType_ActionIndex;
    

    public override void Activate_FinishAction(AnimalAction_ActivateData animalAction_ActivateData)
    {
        //get reference to the animal that this finish action belongs to
        Animal_AI_Base theAnimal = animalAction_ActivateData.TheAnimal;
        //get reference to the current action index
        int previous_MasterStateIndex = theAnimal.GetMasterStateIndex();
        int previous_StateActionIndex = theAnimal.GetStateIndex();
        //alter the index of the specified animal
        int final_MasterStateIndex = MyMathUtils.AlterValue(theAnimal.GetMasterStateIndex(), AlterType_StateIndex, AlterValue_StateIndex);
        int final_StateActionIndex = MyMathUtils.AlterValue(theAnimal.GetStateIndex(), AlterType_ActionIndex, AlterValue_ActionIndex);

        //check if the previous state is the same as the new state or not
        if(previous_MasterStateIndex == final_MasterStateIndex && previous_StateActionIndex == final_StateActionIndex)
        {
            //previous state and new state is the same, no need to do anything for now
            Debug.LogWarning("Same state and action index, nothing to do");
            return;
        }

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
        Debug.LogWarning("Next action start");
        theAnimal.ActivateCurrentAction(animalAction_ActivateData.TheTargetAnimal, ExecuteEvenIfDead);
    }
}
