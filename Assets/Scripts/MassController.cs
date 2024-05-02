using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassController : MonoBehaviour
{
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ChangeMass(float amount)
    {
        if ((rb.mass += amount) < 0) 
        {
            Debug.Log("no Cheese left");
        }
        gameObject.transform.localScale += new Vector3(0.2f,0.1f,0.2f);
    }
}
