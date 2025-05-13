using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Collider_SyncWithCanvas : MonoBehaviour
{
    [SerializeField] private RectTransform Canvas_UI_RT;
    [SerializeField] private RectTransform Image_UI_RT;
    [SerializeField] private Camera Image_UI_Camera;
    [SerializeField] private BoxCollider TheCollider;


    private void LateUpdate()
    {
        Vector3 worldPos;

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Image_UI_Camera, Image_UI_RT.position);
        RectTransformUtility.ScreenPointToWorldPointInRectangle(Canvas_UI_RT, screenPoint, Image_UI_Camera, out worldPos);

        //set the position
        transform.position = worldPos;

        //set the size of the collider
        Vector2 colliderSize = Image_UI_RT.rect.size;
        float scaleFactor = Image_UI_RT.lossyScale.x;

        TheCollider.size = new Vector3(colliderSize.x * scaleFactor, colliderSize.y * scaleFactor, TheCollider.size.z);
    }
}
