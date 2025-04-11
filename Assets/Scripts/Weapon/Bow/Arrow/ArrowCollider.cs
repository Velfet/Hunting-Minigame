using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Note: Might want to either make a parent class "WeaponCollider" and make this class a child class of that class
//or just rename this class to "WeaponCollider", for now it's fine
public class ArrowCollider : MonoBehaviour, IWeaponHitSource
{   
    //not necessarily the game object of the arrow collider itself, but
    //the game object of the entire arrow, the parent game object if you will
    [SerializeField] private GameObject ParentGameObject;
    //set the stats of the arrow, such as how much dmg it deals
    [SerializeField] private HuntingAttackStats_SO AttackStats;
    //[SerializeField] private Collider TheCollider;

    public void Setup_AttackStats(HuntingAttackStats_SO newAttackStats)
    {
        AttackStats = newAttackStats;
    }

    public HuntingAttackStats_SO GetWeaponHitStats()
    {
        return AttackStats;
    }

    public void SetColliderState(bool newState)
    {
        //TheCollider.enabled = newState;
    }

    public void Set_GameObject_Active(bool newState)
    {
        ParentGameObject.SetActive(newState);
    }
}
