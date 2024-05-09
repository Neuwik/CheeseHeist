using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassController : MonoBehaviour
{
    public float MinMass = 10;
    public float MaxMass = 30;
    private Rigidbody rb;
    private Vector3 MassScaleChange = new Vector3(0.2f, 0.1f, 0.2f);

    public CheeseCollectable CheeseCollectablePrefab;
    public GameObject WheelCenter;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ChangeMass(float amount)
    {
        if (amount > 0)
        {
            GainMass(amount);
        }
        else
        {
            LooseMass(amount * -1);
        }
    }
    private void GainMass(float amount)
    {
        if (rb.mass + amount > MaxMass)
        {
            amount = MaxMass - rb.mass;
        }
        rb.mass += amount;
        gameObject.transform.localScale += (MassScaleChange * amount);
    }
    private void LooseMass(float amount)
    {
        if (rb.mass - amount < MinMass)
        {
            amount = rb.mass - MinMass;
        }
        rb.mass += amount;
        gameObject.transform.localScale -= (MassScaleChange * amount);
        StartCoroutine(DropCheeseCollectables(amount));
    }

    private IEnumerator DropCheeseCollectables(float amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Instantiate(CheeseCollectablePrefab, WheelCenter.transform.position + WheelCenter.transform.up + WheelCenter.transform.forward * -3, WheelCenter.transform.rotation);
            yield return new WaitForSeconds(0.3f);
        }
    }
}
