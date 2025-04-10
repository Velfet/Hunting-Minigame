using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Animal_BodyPart_Collider_Base : MonoBehaviour, IAnimalPart_Hit
{
    [SerializeField] protected AnimalHitManager HitManager;
    [SerializeField] protected int hitPriority;
    [SerializeField] protected Collider2D bodyPartCollider;

    public int HitPriority => hitPriority;
    public Collider2D BodyPartCollider => bodyPartCollider;
    //TODO might want to add reference to the animal

    protected void OnTriggerEnter2D(Collider2D other)
    {
        //register the arrow collider to the hit manager
        HitManager.RegisterHit(bodyPartCollider, other);
    }

    public virtual void OnHit(HuntingAttackStats_SO attackData)
    {
        //tell the animal that the body part was hit
        Debug.LogWarning("[animal body part] base class, should not be called");
    }
}
