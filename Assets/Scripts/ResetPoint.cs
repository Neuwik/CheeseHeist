using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    { 
        if (other.TryGetComponent<CheeseWheelMovement>(out CheeseWheelMovement wheel))
        {
            Debug.Log($"{wheel.name} has reached a checkpoint {transform.position}");
            wheel.SetResetPoint(gameObject);
        }
    }
}
