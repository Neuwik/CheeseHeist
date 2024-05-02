using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheeseCollectable : MonoBehaviour
{
    public float CheeseMass = 1;


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<MassController>(out MassController cheeseWheel))
        {         
            new WaitForSeconds(1);
            cheeseWheel.ChangeMass(CheeseMass);
            Destroy(gameObject);
        }
    }
}
