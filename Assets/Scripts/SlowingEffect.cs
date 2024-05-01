using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowingEffect : MonoBehaviour
{

    void Start()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<CheeseWheelMovement>(out CheeseWheelMovement wheep))
        {
            Debug.Log($"{wheep.name} came in contact with honey");
            StartCoroutine(wheep.ApplySlowness(3));
        }
    }
}
