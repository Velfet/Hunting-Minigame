using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePooler<T> : MonoBehaviour where T : MonoBehaviour
{
    public T thePrefab;
    public int initialSize; //also used when the object pooler makes new objects when the object pooler does not have enough objects to meet demand

    //list of objects that are NOT being used
    private List<T> freeList;
    //list of objects that are being used
    private List<T> usedList;

    public void Awake()
    {
        freeList = new List<T>(initialSize);
        usedList = new List<T>(initialSize);

        //instantiate the prefab objects and disable their gameobjects.
        CreateObject(initialSize);
    }

    //return an object from this object pooler
    public T GetObject()
    {
        var numFree = freeList.Count;
        if(numFree == 0)
        {
            //object pooler ran out of objects. Need to instantiate some more
            CreateObject(initialSize);
            numFree = initialSize;
        }

        // get the object from the end of the free list
        var pooledObject = freeList[numFree - 1];
        freeList.RemoveAt(numFree - 1);
        usedList.Add(pooledObject);
        return pooledObject;
    }

    //receive returned object back to the object pooler
    public void ReturnObject(T pooledObject)
    {
        //remove the pooled object from the used list
        usedList.Remove(pooledObject);
        //add the pooled object to the free list
        freeList.Add(pooledObject);

        //set the parent of the pooled object to this object pooler
        var theObject_Transform = pooledObject.transform;
        theObject_Transform.SetParent(this.transform);
        //disable the pooled object
        theObject_Transform.localPosition = Vector3.zero;
        theObject_Transform.localRotation = Quaternion.identity;
        theObject_Transform.gameObject.SetActive(false);
    }

    private void CreateObject(int theAmount)
    {
        for(int i = 0; i < theAmount; i++)
        {
            //instantiate the object, deactivate it, and put it in the free list
            var newObject = Instantiate(thePrefab, this.transform);
            newObject.gameObject.SetActive(false);
            freeList.Add(newObject);
        }
    }

    public List<T> GetAllUsedObjects()
    {
        return usedList;
    }
}
