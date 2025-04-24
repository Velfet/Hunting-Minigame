using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalAction_ActivateData
{
    public Animal_AI_Base TheAnimal;
    public HuntingAttackStats_SO AttackData;
    public Animal_AI_Base TheTargetAnimal;  //used when an action needs a reference to the target animal, for example: its position
    public Animal_AI_Base ThePredatorAnimal;
}
