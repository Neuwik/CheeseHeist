using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedAbility : SpawningAbility
{
    public float Speed;
    private AbilityUser Target;

    protected new void Start()
    {
        base.Start();

        Target = GameManager.Instance.GetOtherPlayerAbilityUser(Caster);
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
