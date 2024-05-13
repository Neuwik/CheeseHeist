using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShieldAbility : SpawningAbility
{
    protected new void Start()
    {
        base.Start();

        MassController Target = Caster.gameObject.GetComponent<MassController>();
        if (Target == null)
        {
            Destroy(gameObject);
        }

        Target.Shield(Duration);
    }
}
