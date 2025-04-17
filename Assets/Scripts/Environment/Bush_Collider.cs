using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush_Collider : Environment_Collider_Base
{
    protected override void OnTriggerEnter(Collider other)
    {
        //check if the collider is a weapon hit box; if so, store a reference to that collider
        IWeaponHitSource source = other.GetComponent<IWeaponHitSource>();
        if (source != null)
        {
            theWeaponHitBox = source;
            //trigger OnHit function
            OnHit(theWeaponHitBox.GetWeaponHitStats());
        }
        
    }

    public override void OnHit(HuntingAttackStats_SO attackData)
    {
        //deactivate the weapon hit box
        if(theWeaponHitBox != null)
        {
            if(makeWeaponDisappear == true)
            {
                Debug.LogWarning("Disable weapon hitbox");
                theWeaponHitBox.Set_GameObject_Active(false);
            }
        }
    }
}
