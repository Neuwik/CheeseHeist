using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class DamagingObject : MonoBehaviour
{
    public float MassDamage;
    public float StatGainPercent;
    public ECheeseMassStats Stat;
    public int Uses;
    [Min(0.1f)]
    public float TickRate; //Ticks per secons

    private Dictionary<MassController, bool> targets;

    private void Start()
    {
        targets = new Dictionary<MassController, bool>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Uses == 0)
        {
            Destroy(gameObject);
        }

        if (other.TryGetComponent<MassController>(out MassController wheel))
        {
            if (targets.TryAdd(wheel, true))
            {
                Debug.Log(wheel.name + " entered " + name + " for the first time");
                StartCoroutine(DamageTarget(wheel));
            }
            else
            {
                Debug.Log(wheel.name + " entered " + name + " again");
                targets[wheel] = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<MassController>(out MassController wheel))
        {
            if (targets.ContainsKey(wheel))
            {
                targets[wheel] = false;
                Debug.Log(wheel.name + " left " + name);
            }
        }
    }

    private IEnumerator DamageTarget(MassController target)
    {
        while (Uses != 0)
        {
            yield return new WaitUntil(() => targets[target]);
            target.LooseMass(MassDamage);
            target.GainStats(Stat, StatGainPercent);
            if (Uses > 0)
            {
                Uses--;
                if (Uses == 0)
                {
                    Destroy(gameObject);
                }
            }
            yield return new WaitForSeconds(1 / TickRate);
        }
    }
}
