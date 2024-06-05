using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheeseCollectable : MonoBehaviour
{
    public CheeseMass CheeseMass;

    private void Start()
    {
        if (CheeseMass == null)
        {
            CheeseMass = new CheeseMass();
        }
        else if (CheeseMass.Mass == 0)
        {
            CheeseMass.GainMassWithSameStats(1);
        }
        else if (CheeseMass.Mass < 0)
        {
            CheeseMass.GainMassWithSameStats(CheeseMass.Mass * -1);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<MassController>(out MassController cheeseWheel))
        {         
            new WaitForSeconds(1);
            cheeseWheel.GainMass(CheeseMass);
            Destroy(gameObject);
        }
    }
}
