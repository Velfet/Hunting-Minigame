using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

public class AnimalHitManager : MonoBehaviour
{
    // [SerializeField] private Animal_Miss_Collider Miss_Collider;
    // [SerializeField] private Animal_Body_Collider body_Collider;
    // [SerializeField] private Animal_Head_Collider head_Collider;
    private Dictionary<IWeaponHitSource, List<IAnimalPart_Hit>> currentHits;
    //private List<(IAnimalPart_Hit bodyPart, HuntingAttackStats_SO attackData)> currentHits;

    private void Awake()
    {
        currentHits = new Dictionary<IWeaponHitSource, List<IAnimalPart_Hit>>();
        //currentHits = new List<(IAnimalPart_Hit bodyPart, HuntingAttackStats_SO attackData)>();
    }

    public void RegisterHit(Collider2D bodyPartCollider , Collider2D weaponCollider)
    {
        IWeaponHitSource source = weaponCollider.GetComponent<IWeaponHitSource>();
        IAnimalPart_Hit part = bodyPartCollider.GetComponent<IAnimalPart_Hit>();
        //Debug.LogWarning("Register hit phase 1");
        if (source == null || part == null)
        {
            Debug.LogWarning("NULL source: " + (source == null));
            Debug.LogWarning("NULL weapon: " + (part == null));
            return;
        }

        //HuntingAttackStats_SO attackData = source.GetWeaponHitStats();
        //Debug.LogWarning("Register hit phase 2");

        //check if a dictionary entry exist
        if(currentHits.ContainsKey(source) == false)
        {
            //make dictionary entry
            currentHits.Add(source, new List<IAnimalPart_Hit>());
        }
        //add to the body part list located in the dictionary entry
        currentHits[source].Add(part);
    }

    private void LateUpdate()
    {
        if(currentHits == null)
        {
            return;
        }
        
        List<IWeaponHitSource> toBeDeactivated = new List<IWeaponHitSource>();

        //cycle through each weapon collider
        foreach(var hitData in currentHits)
        {
            //get list of body parts
            List<IAnimalPart_Hit> bodyParts = hitData.Value;

            //to store the chosen hit data
            (IAnimalPart_Hit bodyPart, HuntingAttackStats_SO attackData)? chosenHit = null;

            //try to find the body part that got hit with the highest priority, that body part alone will be the one to react
            foreach(var singleBodyPart in bodyParts)
            {
                if(chosenHit == null || singleBodyPart.HitPriority > chosenHit.Value.bodyPart.HitPriority)
                {
                    chosenHit = (singleBodyPart, hitData.Key.GetWeaponHitStats());
                }
            }

            //tell the body part that got hit with the highest priority to react
            if(chosenHit != null)
            {
                chosenHit.Value.bodyPart.OnHit(chosenHit.Value.attackData);
                //add the weapon collider to the list, we'll deactivate the parent game object later
                toBeDeactivated.Add(hitData.Key);
            }
        }

        //deactivate the parent game objects of weapons colliders that have collided with at least 1 body part
        for(int i = 0; i < toBeDeactivated.Count; i++)
        {
            toBeDeactivated[i].Set_GameObject_Active(false);
        }

        //clear hit data this frame
        currentHits.Clear();
    }
}
