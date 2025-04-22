using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntingGameClient : MonoBehaviour
{
    [SerializeField] private HuntingCaseManager HuntingCaseManager;
    [SerializeField] private int TestRank;
    [SerializeField] private int TestLevel;

    // Start is called before the first frame update
    void Start()
    {
        Start_TestCase();
    }

    public void Start_TestCase()
    {
        HuntingCaseManager.Load_SpecifiedLevel(TestRank, TestLevel);
    }

    
}
