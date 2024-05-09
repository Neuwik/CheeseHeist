using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCollectable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<AbilityUser>(out AbilityUser cheeseWheel))
        {
            if(cheeseWheel.SetAbility(GameManager.Instance.GetRandomAbility()))
            {
                Destroy(gameObject);
            }
        }
    }
}
