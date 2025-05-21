using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelIconManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> Highlight_GO_List;

    
    public void ActivateImages(int imageAmount)
    {
        for (int i = 0; i < Highlight_GO_List.Count; i++)
        {
            if (i < imageAmount)
            {
                Highlight_GO_List[i].SetActive(true);
            }
            else
            {
                Highlight_GO_List[i].SetActive(false);
            }
        }
    }
}
