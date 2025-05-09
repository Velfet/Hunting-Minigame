using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingReactionText_Manager : MonoBehaviour
{
    [SerializeField] private RectTransform CanvasRect;
    [SerializeField] private List<FloatingReactionText> ActiveFloatingText = new List<FloatingReactionText>();
    [SerializeField] private List<FloatingReactionText> DormantFloatingText = new List<FloatingReactionText>();

    public void ReturnFloatingText(FloatingReactionText returnedFloatingText)
    {
        //deactivate floating text
        returnedFloatingText.gameObject.SetActive(false);
        //remove from active list
        ActiveFloatingText.Remove(returnedFloatingText);
        //add to dormant list
        DormantFloatingText.Add(returnedFloatingText);
    }

    //call this function to show a floating text
    public void ActivateFloatingText(string theText, Vector3 worldPos)
    {
        //check if there are any available floating text
        if(DormantFloatingText.Count > 0)
        {
            //get floating text from dormant list
            FloatingReactionText theFloatingText = DormantFloatingText[0];
            //remove from dormant list
            DormantFloatingText.Remove(theFloatingText);
            //add to active list
            ActiveFloatingText.Add(theFloatingText);
            //activate the floating text
            theFloatingText.BeginFloatingTextAnim(theText, worldPos, CanvasRect, this);
        }
        else
        {
            Debug.LogWarning("Not enough floating text");
        }
    }
}
