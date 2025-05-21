using System.Collections;
using System.Collections.Generic;
using HuntingGame;
using UnityEngine;

public class HuntingGameClient : MonoBehaviour
{
    [SerializeField] private HuntingCaseManager HuntingCaseManager;
    [SerializeField] private HuntingBow Bow;
    [SerializeField] private HoverTarget_UI_MultipleImages Rank_UI;
    [Space(10)]
    [SerializeField] private List<string> RankNames;
    [Space(10)]
    [SerializeField] private string HuntingBGM_SoundID;
    [Space(10)]
    [SerializeField] private int TestRank;
    [SerializeField] private int TestLevel;

    // Start is called before the first frame update
    void Start()
    {
        //Start_TestCase();
    }

    public void Start_TestCase()
    {
        Rank_UI.ActivateImages(TestRank);
        Rank_UI.Update_HoverText(RankNames[TestRank - 1]);    //for index, substract 1 from rank
        Bow.Set_BowRank(TestRank - 1);    //for index, substract 1 from rank
        HuntingCaseManager.Load_SpecifiedLevel(TestRank, TestLevel);
        
        //play Hunting Game BGM
        AudioManager.Instance.PlayAudio(HuntingBGM_SoundID);
    }


    public void SetRank(int newRank)
    {
        TestRank = newRank;
    }

    public void Start_HuntingMinigame(int theRank)
    {
        SetRank(theRank);
        Start_TestCase();
    }

    
}
