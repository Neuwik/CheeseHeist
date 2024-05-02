using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowingEffect : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CheeseWheelMovement>(out CheeseWheelMovement wheel))
        {
            Debug.Log($"{wheel.name} came in contact with honey");
            wheel.ApplySlowness(3);
            Destroy(gameObject);
        }
    }
}
