using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimalAction_Base
{    
    [SerializeReference]
    public AnimalFinishAction_Base AnimalFinishAction;

    public virtual void Activate_AnimalAction(AnimalAction_ActivateData animalAction_ActivateData)
    {
        Debug.LogWarning("Base animal action, should not be used");
    }
}
