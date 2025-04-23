using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimalFinishAction_Base
{
    public int FinishActionPriority = 0;
    public virtual void Activate_FinishAction(AnimalAction_ActivateData animalAction_ActivateData)
    {
        Debug.LogWarning("Base animal finish action, should not be used");
    }
}
