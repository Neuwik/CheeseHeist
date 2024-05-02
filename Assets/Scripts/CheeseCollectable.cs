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
            //Debug.Log($"{cheeseWheel.name} came in contact with honey");
            
            new WaitForSeconds(1);
            cheeseWheel.ChangeMass(CheeseMass);
            Destroy(gameObject);
        }
    }
}
