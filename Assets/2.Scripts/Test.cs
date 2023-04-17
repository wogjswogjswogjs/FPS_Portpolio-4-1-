using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Test : MonoBehaviour
{
    Collider bc;
    public Test2 test2;
    void Start()
    {
        bc = this.GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    
    private void OnDrawGizmos()
    {
       
    }
}
