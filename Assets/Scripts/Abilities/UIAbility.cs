using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAbility : AAbility
{
    public bool UseOnSelf = false;

    public override bool UseAbility(AbilityUser user)
    {
        Canvas Target;
        if (UseOnSelf)
        {
            Target = user.UICanvas;
        }
        else
        {

            Target = GameManager.Instance.GetOtherPlayerObject(user.gameObject)?.GetComponent<AbilityUser>()?.UICanvas;
        }

        if (Target == null)
        {
            return false;
        }

        AAbility ability;

        ability = Instantiate(this);
        ability.transform.SetParent(Target.transform, false);

        ability.Caster = user;
        return true;
    }

    public override IEnumerator DisableAfterDuration(float duration)
    {
        if (duration >= 0)
        {
            yield return new WaitForSeconds(duration);
            Destroy(gameObject);
        }
    }
}
