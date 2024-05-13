using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AAbility : MonoBehaviour
{
    public Sprite Icon;
    public string Name;
    public AbilityUser Caster;
    public float Duration;

    protected void Start()
    {
        if (Duration >= 0)
        {
            StartCoroutine(DisableAfterDuration(Duration));
        }
    }

    public virtual bool UseAbility(AbilityUser user)
    {
        return false;
    }

    public virtual IEnumerator DisableAfterDuration(float duration)
    {
        if (duration >= 0)
        {
            yield return new WaitForSeconds(duration);
        }
    }
}
