using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HuntingLevelIcon : MonoBehaviour
{
    [SerializeField] private Image LevelIcon_Image;
    [Space(10)]
    [SerializeField] private Sprite Sprite_Done;
    [SerializeField] private Sprite Sprite_Current;
    [SerializeField] private Sprite Sprite_NotYet;

    //current mode: 0, 1, 2 (done, current, not yet)
    public void Update_LevelIcon(int currentMode)
    {
        switch (currentMode)
        {
            case 0:
                LevelIcon_Image.sprite = Sprite_Done;
                break;
            case 1:
                LevelIcon_Image.sprite = Sprite_Current;
                break;
            case 2:
                LevelIcon_Image.sprite = Sprite_NotYet;
                break;
            default:
                break;
        }
    }
}
