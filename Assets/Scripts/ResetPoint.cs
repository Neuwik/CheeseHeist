using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        CheeseWheelMovement wheep = other.gameObject.GetComponent<CheeseWheelMovement>();
        if (wheep != null)
        {
            Debug.Log($"{wheep.name} has reached a checkpoint {transform.position}");
            wheep.ResetPoint = gameObject;
        }
    }
}
