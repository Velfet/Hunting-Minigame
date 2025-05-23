using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverTarget_UI : MonoBehaviour
{
    [SerializeField] protected HoverManager_UI HoverManager;
    public string HoverText;
    [Space(10)]
    [SerializeField] private Image Image;
    [SerializeField] private Sprite HighlightSprite;
    [SerializeField] private Sprite NormalSprite;
    [Space(10)]
    [SerializeField] private Vector2 Offset_ScreenPos;



    public virtual void OnBeingHovered()
    {
        //set visual to highlight mode
        Image.sprite = HighlightSprite;
    }

    public virtual void OnStopBeingHovered()
    {
        //set visual to non-higlight mode
        Image.sprite = NormalSprite;
    }

    public void Update_HoverText(string newText)
    {
        HoverText = newText;
    }

    public Vector2 Get_Offset_ScreenPos()
    {
        return Offset_ScreenPos;
    }
}
