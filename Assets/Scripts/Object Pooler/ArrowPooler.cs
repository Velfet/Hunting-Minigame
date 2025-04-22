using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPooler : BasePooler<HuntingArrow>
{
    private List<HuntingArrow> activeArrows;

    public HuntingArrow GetHuntingArrow()
    {
        return GetObject();
    }

    public void ReturnHuntingArrow(HuntingArrow theArrow)
    {
        ReturnObject(theArrow);
    }

    public void ReturnAllHuntingArrow()
    {
        activeArrows = GetAllUsedObjects();

        while(activeArrows.Count > 0)
        {
            activeArrows[0].InterruptArrowMovement();
            ReturnHuntingArrow(activeArrows[0]);
        }
    }

    public List<HuntingArrow> GetActiveHuntingArrows()
    {
        return GetAllUsedObjects();
    }
}
