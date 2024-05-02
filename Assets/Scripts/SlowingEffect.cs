using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowingEffect : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CheeseWheelMovement>(out CheeseWheelMovement wheel))
        {
            wheel.ApplySlowness(3);
            new WaitForSeconds(1);
            Destroy(gameObject);
        }
    }
}
