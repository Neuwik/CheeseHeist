using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ability : MonoBehaviour
{
    public Sprite Icon;
    public string Name;

    public bool UseAbility(AbilityUser user)
    {
        Vector3 SpanwOffset = (user.WheelCenter.transform.up + user.WheelCenter.transform.forward * -1) * 3;
        Instantiate(this, user.WheelCenter.transform.position + SpanwOffset, user.WheelCenter.transform.rotation);
        return true;
    }
}
