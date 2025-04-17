using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment_Collider_Base : MonoBehaviour, IEnvironmentPart_Hit
{
    [SerializeField] protected bool makeWeaponDisappear;
    [SerializeField] protected int hitPriority;
    [SerializeField] protected Collider environmentCollider;
    [SerializeField] protected IWeaponHitSource theWeaponHitBox;

    public bool MakeWeaponDisappear => makeWeaponDisappear;
    public int HitPriority => hitPriority;
    public Collider EnvironmentCollider => environmentCollider;

    protected virtual void OnTriggerEnter(Collider other)
    {
        //base class, should not be called
        Debug.LogWarning("[Environment Part] OnTriggerEnter: base class, should not be called");
    }

    public virtual void OnHit(HuntingAttackStats_SO attackData)
    {
        //tell the environment that the body part was hit
        Debug.LogWarning("[Environment Part] OnHit: base class, should not be called");
    }
}
