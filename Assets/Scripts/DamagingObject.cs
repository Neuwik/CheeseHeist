using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingObject : MonoBehaviour
{
    public float Damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<MassController>(out MassController wheel))
        {
            wheel.ChangeMass(Damage * -1);
            Destroy(gameObject);
        }
    }
}
