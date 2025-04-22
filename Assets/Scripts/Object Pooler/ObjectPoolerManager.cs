using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolerManager : MonoBehaviour
{
    [SerializeField] private ArrowPooler ArrowPooler;



    public void ReturnAllWeaponHitboxes()
    {
        ArrowPooler.ReturnAllHuntingArrow();
    }
}
