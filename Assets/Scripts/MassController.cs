using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassController : MonoBehaviour
{
    public float MassSteps = 100;
    public float MinMass = 15;
    public float MaxMass = 30;
    public Rigidbody rb;
    public float CurrentMass { get => rb.mass / MassSteps; }
    [SerializeField]
    private Vector3 MassScaleChange = new Vector3(0.2f, 0.1f, 0.2f);

    public CheeseCollectable CheeseCollectablePrefab;
    public GameObject WheelCenter { get => gameObject; }

    private bool shielded = false;
    private int shieldCount = 0;

    private bool _isInitialized = false;
    private float _rbStartMass = 0;
    private Vector3 _startScale = Vector3.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (!_isInitialized)
        {
            _rbStartMass = rb.mass;
            _startScale = gameObject.transform.localScale;
        }
    }

    void Start()
    {
        rb.mass = _rbStartMass;
        gameObject.transform.localScale = _startScale;
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
        if (CurrentMass + amount > MaxMass)
        {
            amount = MaxMass - CurrentMass;
        }
        rb.mass += (amount * MassSteps);
        gameObject.transform.localScale += (MassScaleChange * amount);
    }
    private void LooseMass(float amount)
    {
        if (shielded)
        {
            return;
        }

        if (CurrentMass - amount < MinMass)
        {
            amount = CurrentMass - MinMass;
        }
        rb.mass -= (amount * MassSteps);
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

    public void Shield(float duration)
    {
        StartCoroutine(ApplyShield(duration));
    }

    public IEnumerator ApplyShield(float duration)
    {
        shieldCount++;
        shielded = true;
        yield return new WaitForSeconds(duration);
        shieldCount--;
        if (shieldCount <= 0)
        {
            shielded = false;
        }
    }
}
