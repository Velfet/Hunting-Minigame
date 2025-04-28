using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal_AI_Flyer : Animal_AI_Base
{
    

    public override void UpdateAnimalStatus(AnimalStatus newStatus, bool instantDeath = false)
    {
        if(AnimalState == newStatus)
        {
            return;
        }

        AnimalState = newStatus;
        
        switch(newStatus)
        {
            case AnimalStatus.Alive:
                //nothing for now; Maybe set current hp to max hp?
                break;
            case AnimalStatus.Dead:
                //stop the current animal action
                StopAndDeleteAction();
                //play dead animation
                //TODO activate "OnDeath_Action" which should make:
                React_Die_Start();
                //0. disable the "Animal_Detected_Collider"
                //1. the animal fall to the ground
                //2. re-enable the "Animal_Detected_Collider"
                //3. spawn blood effect
                //4. invoke dead action (Raise_OnDeathEvent)
                //[942] show and play blood animation
                
                //Old start, should remove later
                // if(instantDeath == false)
                // {
                //     AnimationManager.Start_Animation(AnimalAnimationKeys.Die);
                //     AnimalDieBlood_Effect.gameObject.SetActive(true);
                //     AnimalDieBlood_Effect.PlayEffectAnim();
                // }
                // else
                // {
                //     AnimationManager.Start_Animation_JumpToEnd(AnimalAnimationKeys.Die);
                //     AnimalDieBlood_Effect.gameObject.SetActive(true);
                //     AnimalDieBlood_Effect.PlayEffectAnim_JumpToEnd();
                // }
                // //invoke dead action
                // Raise_OnDeathEvent();
                //Old end

                //report status to the case manager
                huntingCaseManager.ReportStatus(AnimalIdentity, AnimalState);
                break;
            case AnimalStatus.Escaped:
                //stop the current animal action
                StopAndDeleteAction();
                //stop animation
                AnimationManager.Stop_Animation();
                //hide animal sprite
                AnimationManager.ToggleAnimalVisualVisibility(false);
                //invoke escape action
                Raise_OnEscapeEvent();
                //report status to the case manager
                huntingCaseManager.ReportStatus(AnimalIdentity, AnimalState);
                break;
            case AnimalStatus.Eaten:
                //update is being eaten status
                isBeingEaten = false;
                //stop animation
                AnimationManager.Stop_Animation();
                //hide animal sprite
                AnimationManager.ToggleAnimalVisualVisibility(false);
                //hide blood
                AnimalDieBlood_Effect.gameObject.SetActive(true);
                //invoke was eaten action
                Raise_OnEatenEvent();
                //report status to the case manager
                huntingCaseManager.ReportStatus(AnimalIdentity, AnimalState);
                break;
        }


    }

    protected void React_Die_Start()
    {
        //0. disable the "Animal_Detected_Collider"
        Animal_Detected_Collider.gameObject.SetActive(false);
        //1. the animal fall to the ground
        AnimalAction_ActivateData animalAction_ActivateData = new AnimalAction_ActivateData{
            TheAnimal = this,
        };
        OnDeath_Action.Activate_FinishAction(animalAction_ActivateData);
        //2. re-enable the "Animal_Detected_Collider"
        //3. spawn blood effect
        //4. invoke dead action (Raise_OnDeathEvent)
    }


}
