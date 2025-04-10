using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SimpleBase
{
    public int baseValue;
}

[Serializable]
public class SimpleChildA : SimpleBase
{
    public float childAValue;
}

[Serializable]
public class SimpleChildB : SimpleBase
{
    public string childBValue;
}

public class TestSerialization : MonoBehaviour
{
    [SerializeField]
    public List<SimpleBase> simpleList;
}
