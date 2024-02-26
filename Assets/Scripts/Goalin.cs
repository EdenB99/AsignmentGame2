using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalIn : MonoBehaviour
{
    CarBase car;
    BoxCollider goalinTrigger;
    public Canvas EndUI;
    void Start()
    {
        goalinTrigger = GetComponent<BoxCollider>();
        car = FindAnyObjectByType<CarBase>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        
        EndUI.gameObject.SetActive(true);
        car.EndinputData();
    }
}
