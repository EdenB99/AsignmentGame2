using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.StandaloneInputModule;
using UnityEngine.InputSystem;
using Cinemachine;
using System;

public class CameraChange : MonoBehaviour
{
    CameraInput Camerainput;
    public CinemachineVirtualCamera[] vcams;

    private void Awake()
    {
        Camerainput = new();
    }
    private void Start()
    {
        if (vcams == null)
        {
            vcams = FindObjectsByType<CinemachineVirtualCamera>(FindObjectsSortMode.None);
        }
    }

    private void OnEnable()
    {
        Camerainput.Camera.Enable();
        Camerainput.Camera.Input1.performed += InputOne;
        Camerainput.Camera.Input2.performed += InputTwo;
        Camerainput.Camera.Input3.performed += InputThree;
    }


    private void OnDisable()
    {
        Camerainput.Camera.Input3.performed -= InputThree;
        Camerainput.Camera.Input2.performed -= InputTwo;
        Camerainput.Camera.Input1.performed -= InputOne;
        Camerainput.Camera.Disable();
    }
    private void InputOne(InputAction.CallbackContext context)
    {
        vcams[0].Priority = 100;
        vcams[1].Priority = 10;
        vcams[2].Priority = 10;
    }

    private void InputTwo(InputAction.CallbackContext context)
    {
        vcams[0].Priority = 10;
        vcams[1].Priority = 100;
        vcams[2].Priority = 10;
    }

    private void InputThree(InputAction.CallbackContext context)
    {
        vcams[0].Priority = 10;
        vcams[1].Priority = 10;
        vcams[2].Priority = 100;
    }

}
