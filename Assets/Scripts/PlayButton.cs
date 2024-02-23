using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    public Image image;
    CameraChange CameraChange;
    CarBase CarBase;
    public Canvas ingameText;

    void Start()
    {
        CameraChange = FindAnyObjectByType<CameraChange>();
        CarBase = FindAnyObjectByType<CarBase>();
        
    }
    public void OnClick()
    {
        Debug.Log("Clicked Image");
        gameObject.SetActive(false);
        CameraChange.vcams[0].Priority = 100;
        CameraChange.vcams[1].Priority = 10;
        CameraChange.vcams[2].Priority = 10;
        CarBase.StartinputData();
        ingameText.gameObject.SetActive(true);
    }
}
