using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimalAction_Base
{    
    [SerializeReference]
    public AnimalFinishAction_Base AnimalFinishAction;
    public int ActionPriority = 0;  //the higher the priority, the less it will be able to be interrupted

    public virtual void Activate_AnimalAction(AnimalAction_ActivateData animalAction_ActivateData)
    {
        Debug.LogWarning("Base animal action, should not be used");
    }
}
