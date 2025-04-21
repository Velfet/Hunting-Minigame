using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyMathUtils
{
    public static int AlterValue(int num1, Enum_AlterType alterType, int num2)
    {
        int returnValue = num1;
        switch(alterType)
        {
            case Enum_AlterType.Add:
                returnValue += num2;
                break;
            case Enum_AlterType.Replace:
                returnValue = num2;
                break;
            case Enum_AlterType.Nothing:
            default:
                break;
        }

        return returnValue;
    }

    
}
