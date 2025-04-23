using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Animal_BodyPart_Collider_Base : MonoBehaviour, IAnimalPart_Hit
{
    [SerializeField] protected AnimalHitManager HitManager;
    [SerializeField] protected bool makeWeaponDisappear;
    [SerializeField] protected int hitPriority;
    [SerializeField] protected Collider bodyPartCollider;
    [SerializeField] protected Vector3 hitPosition;

    public bool MakeWeaponDisappear => makeWeaponDisappear;
    public int HitPriority => hitPriority;
    public Collider BodyPartCollider => bodyPartCollider;

    

    //reference to the animal
    public Animal_AI_Base TheAnimal;

    protected void OnTriggerEnter(Collider other)
    {
        //register the arrow collider to the hit manager
        HitManager.RegisterHit(bodyPartCollider, other);

        //store the hit position, aka the position on the animal where the weapon hitbox hits the animal
        IWeaponHitSource source = other.GetComponent<IWeaponHitSource>();
        if (source != null)
        {
            hitPosition = bodyPartCollider.ClosestPoint(other.transform.position);
            hitPosition.z = TheAnimal.transform.position.z;
        }
    }

    public virtual void OnHit(HuntingAttackStats_SO attackData)
    {
        //tell the animal that the body part was hit
        Debug.LogWarning("[animal body part] base class, should not be called");
    }
}
