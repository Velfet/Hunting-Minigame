using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntingCase_Data : MonoBehaviour
{
    [Serializable]
    public class HuntingCase_Rank_Data
    {
        public List<HuntingCase_Level_Data> allLevel_HuntingCase_Data;
    }

    [Serializable]
    public class HuntingCase_Level_Data
    {
        public List<HuntingCase_SO> level_HuntingCases;
    }

    [SerializeField] private List<HuntingCase_Rank_Data> All_HuntingCase_Data;

    //current rank starts at 1, current level also starts at 1
    public HuntingCase_SO GetHuntingCaseData(int currentRank, int currentLevel)
    {
        int currentRank_Index = currentRank - 1;
        //verify that current rank is valid
        if(All_HuntingCase_Data.Count >= currentRank_Index || currentRank_Index < 0)
        {
            //current rank is not valid
            Debug.LogWarning("[HuntingCaseData] Rank is not valid: " + currentRank);
            return null;
        }

        HuntingCase_Rank_Data currentRankData = All_HuntingCase_Data[currentRank_Index];
        int currentLevel_Index = currentLevel - 1;
        //verify that current level is valid
        if(currentRankData.allLevel_HuntingCase_Data.Count >= currentLevel_Index || currentLevel_Index < 0)
        {
            //current rank is not valid
            Debug.LogWarning("[HuntingCaseData] Level is not valid: " + currentLevel);
            return null;
        }

        HuntingCase_Level_Data currentLevelData = currentRankData.allLevel_HuntingCase_Data[currentLevel_Index];
        //select a random case for the current rank and level and return it
        int randomIndex = UnityEngine.Random.Range(0, currentLevelData.level_HuntingCases.Count);
        HuntingCase_SO selectedHuntingCaseData = currentLevelData.level_HuntingCases[randomIndex];

        return selectedHuntingCaseData;
    }
}
