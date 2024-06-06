using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassController : MonoBehaviour
{
    public float RBMassPerMass = 100;
    public float MinMass = 15;
    public float MaxMass = 30;
    private CheeseMass _cheeseMass;
    public CheeseMass CheeseMass
    {
        get { return _cheeseMass; }
        set
        {
            CheeseMass newCheeseMass = value;
            if (newCheeseMass.Mass < MinMass)
            {
                newCheeseMass.GainMassWithSameStats(MinMass - newCheeseMass.Mass);
            }
            if (newCheeseMass.Mass > MaxMass)
            {
                newCheeseMass.LooseMass(newCheeseMass.Mass - MaxMass);
            }
            _cheeseMass = newCheeseMass;
            rb.mass = _cheeseMass.Mass * RBMassPerMass;
        }
    }

    public Rigidbody rb;
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
    public void GainMass(CheeseMass mass)
    {
        if (CheeseMass.Mass + mass.Mass > MaxMass)
        {
            float newMass = MaxMass - CheeseMass.Mass;
            mass.LooseMass(mass.Mass - newMass);
        }

        CheeseMass.MergeWithOtherCheeseMass(mass);
        rb.mass += (mass.Mass * RBMassPerMass);
        gameObject.transform.localScale += (MassScaleChange * mass.Mass);
    }
    public void GainStats(ECheeseMassStats stat, float amount)
    {
        CheeseMass.AddStat(stat, amount);
    }
    public void LooseMass(float amount)
    {
        Debug.Log($"MassController: LooseMass({amount})");
        if (shielded)
        {
            return;
        }

        if (CheeseMass.Mass - amount < MinMass)
        {
            amount = CheeseMass.Mass - MinMass;
        }

        Debug.Log($"MassController: LooseMass({amount}) fr");

        CheeseMass.LooseMass(amount);
        rb.mass -= (amount * RBMassPerMass);

        gameObject.transform.localScale -= (MassScaleChange * amount);

        StartCoroutine(DropCheeseCollectables(amount));
    }

    private IEnumerator DropCheeseCollectables(float amount)
    {
        for (int i = 0; i < amount; i++)
        {
            CheeseCollectable cc = Instantiate(CheeseCollectablePrefab, WheelCenter.transform.position + WheelCenter.transform.up + WheelCenter.transform.forward * -3, WheelCenter.transform.rotation);
            cc.CheeseMass = new CheeseMass(1, CheeseMass.Stats);
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
