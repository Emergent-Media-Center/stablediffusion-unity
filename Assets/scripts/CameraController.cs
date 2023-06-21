using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera vCamera;

    void Start()
    {
        vCamera = GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            vCamera.enabled = true;
        }
        else
        {
            vCamera.enabled = false;
        }
    }
}
