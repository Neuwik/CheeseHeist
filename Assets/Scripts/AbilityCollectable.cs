using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCollectable : MonoBehaviour
{
    public float RespawnTime = 20;
    public Collider Collider;
    public GameObject Animation;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<AbilityUser>(out AbilityUser cheeseWheel))
        {
            if(cheeseWheel.SetAbility(GameManager.Instance.GetRandomAbility()))
            {
                StartCoroutine(DespawnAndRespawn());
            }
        }
    }

    private IEnumerator DespawnAndRespawn()
    {
        Collider.enabled = false;
        Animation.SetActive(false);
        yield return new WaitForSeconds(RespawnTime);
        Animation.SetActive(true);
        Collider.enabled = true;
    }
}
