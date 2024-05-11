using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedAbility : Ability
{
    public float Speed;
    private GameObject Target;

    protected new void Start()
    {
        base.Start();

        Target = GameManager.Instance.GetOtherPlayerObject(Caster.gameObject);
        if (Target == null)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        Vector3 direction = Target.transform.position - transform.position;
        transform.LookAt(Target.transform.position);
        transform.position += direction * Speed * Time.deltaTime;
    }
}
