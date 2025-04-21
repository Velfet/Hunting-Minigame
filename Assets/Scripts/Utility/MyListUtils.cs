using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyListUtils
{
    //returns true if all the elements in list A exist in list B
    public static bool Is_ListA_Inside_ListB<T>(List<T> listA, List<T> listB)
    {
        if(listA == null || listA.Count == 0)
        {
            return true;
        }

        //int is the number of entries; This allows us to keep track of duplicate data in the list
        Dictionary<T, int> lookUp = new Dictionary<T, int>();

        //populate the dictionary with the entries in "listA"
        for(int i = 0; i < listA.Count; i++)
        {
            int elementCount = 0;

            //check if the current element is already in the dictionary or not
            if(lookUp.TryGetValue(listA[i], out elementCount))
            {
                //element already in dictionary, increment count
                lookUp[listA[i]] = elementCount + 1;
            }
            else
            {
                //element not yet in dictionary, add it with an initial count of 1
                lookUp.Add(listA[i], 1);
            }
        }

        //update and potentially de-populate the dictionary with the entries in "ListB"
        for(int i = 0; i < listB.Count; i++)
        {
            int elementCount = 0;

            //check if the current element is in the dictionary or not
            if(lookUp.TryGetValue(listB[i], out elementCount))
            {
                //element is in dictionary, reduce its count if it has not yet already reached 0
                elementCount--;
                if(elementCount > 0)
                {
                    //element count is not yet zero, don't remove it from the dictionary
                    lookUp[listB[i]] = elementCount;
                }
                else
                {
                    //element count is 0, remove the entry from the dictionary
                    lookUp.Remove(listB[i]);
                    //check if the dictionary is already empty
                    if(lookUp.Count == 0)
                    {
                        return true;
                    }
                }
            }
            else
            {
                //element is not in dictionary, move on to the next element in the list
                continue;
            }
        }

        //return true if the dictionary is empty; otherwise, return false
        return lookUp.Count == 0;
    }
}
