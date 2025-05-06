using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverTarget_UI_MultipleImages : HoverTarget_UI
{

    [SerializeField] private List<GameObject> Image_GO_List;
    [SerializeField] private List<GameObject> Highlight_GO_List;


    public override void OnBeingHovered()
    {
        //set visual to highlight mode
        for(int i = 0; i < Highlight_GO_List.Count; i++)
        {
            Highlight_GO_List[i].SetActive(true);
        }
    }

    public override void OnStopBeingHovered()
    {
        //set visual to non-higlight mode
        for(int i = 0; i < Highlight_GO_List.Count; i++)
        {
            Highlight_GO_List[i].SetActive(false);
        }
    }

    public void ActivateImages(int imageAmount)
    {
        for(int i = 0; i < Image_GO_List.Count; i++)
        {
            if(i < imageAmount)
            {
                Image_GO_List[i].SetActive(true);
            }
            else
            {
                Image_GO_List[i].SetActive(false);
            }
        }
    }



}
