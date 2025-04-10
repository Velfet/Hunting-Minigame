using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalFinishAction_AlterIndex : AnimalFinishAction_Base
{
    public int AlterValue_StateIndex;
    public Enum_AlterType AlterType_StateIndex;
    public int AlterValue_ActionIndex;
    public Enum_AlterType AlterType_ActionIndex;

    public override void Activate_FinishAction(AnimalAction_ActivateData animalAction_ActivateData)
    {
        //get reference to the animal that this finish action belongs to
        Animal_AI_Base theAnimal = animalAction_ActivateData.TheAnimal;
        //alter the index of the specified animal

        int final_MasterStateIndex = MyMathUtils.AlterValue(theAnimal.GetMasterStateIndex(), AlterType_StateIndex, AlterValue_StateIndex);
        int final_StateActionIndex = MyMathUtils.AlterValue(theAnimal.GetStateIndex(), AlterType_ActionIndex, AlterValue_ActionIndex);
        theAnimal.SetMasterStateIndex(final_MasterStateIndex);
        theAnimal.SetStateIndex(final_StateActionIndex);

        //also trigger the next action
        theAnimal.ActivateCurrentAction();
    }
}
