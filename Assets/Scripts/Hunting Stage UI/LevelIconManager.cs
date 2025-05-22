using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelIconManager : MonoBehaviour
{
    [SerializeField] private List<HuntingLevelIcon> HuntingLevelIcons;


    public void ActivateImages(int currentLevelIndex)
    {
        for (int i = 0; i < HuntingLevelIcons.Count; i++)
        {
            if (i < currentLevelIndex)
            {
                //Activate "done" visual
                HuntingLevelIcons[i].Update_LevelIcon(0);
            }
            else if (i == currentLevelIndex)
            {
                //Activate "Current" visual
                HuntingLevelIcons[i].Update_LevelIcon(1);
            }
            else
            {
                //Activate "Not yet" visual
                HuntingLevelIcons[i].Update_LevelIcon(2);
            }
        }
    }
    
    public void ActivateImages_NewDone(int currentLevelIndex)
    {
        for (int i = 0; i < HuntingLevelIcons.Count; i++)
        {
            if (i < currentLevelIndex)
            {
                //Activate "done" visual
                HuntingLevelIcons[i].Update_LevelIcon(0);
            }
            else
            {
                //Activate "Not yet" visual
                HuntingLevelIcons[i].Update_LevelIcon(2);
            }
        }
    }
}
