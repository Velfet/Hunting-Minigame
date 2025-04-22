using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        Update_BillboardUI();
    }

    public void Update_BillboardUI()
    {
        //transform.forward = mainCamera.transform.forward;

        // Vector3 lookDirection = transform.position - mainCamera.transform.position;
        // lookDirection.y = 0; // remove vertical tilt
        //transform.rotation = Quaternion.identity;

        //transform.LookAt(mainCamera.transform.position, Vector3.up);

        // Direction from UI to camera
        // Vector3 lookDirection = transform.position - mainCamera.transform.position;
        // //lookDirection.y = 0; // eliminate vertical component

        // if (lookDirection != Vector3.zero)
        //     transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);

        transform.LookAt(transform.position + mainCamera.transform.forward, mainCamera.transform.up);
    }
}
