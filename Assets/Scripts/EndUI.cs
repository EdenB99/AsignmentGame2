using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using TreeEditor;
using UnityEngine;

public class EndUI : MonoBehaviour
{
    TextMeshProUGUI[] infoText;
    //[current time : ]
    public float currenttime = 0;
    //[average Speed : ]
    
    //[Over-the-line : ]
    bool isEnd = false;

    private void Awake()
    {
        infoText = GetComponentsInChildren<TextMeshProUGUI>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnEnable()
    {
        isEnd = true;
    }
}
