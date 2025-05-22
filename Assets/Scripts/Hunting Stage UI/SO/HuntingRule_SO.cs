using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HuntingGame
{
    [CreateAssetMenu(fileName = "HuntingRule_SO_", menuName = "ScriptableObjects/UI/HuntingGame/HuntingRule")]
    public class HuntingRule_SO : ScriptableObject
    {
        public string RuleTitle_String;
        [TextArea(5, 10)]
        public string RuleDetail_String;
    }

}

