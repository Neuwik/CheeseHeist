using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ability : MonoBehaviour
{
    public Sprite Icon;
    public string Name;
    public AbilityUser Caster;
    public bool SpawnAsCasterChild = false;
    public Vector3 SpawnOffsetFromCaster;
    public float Duration;

    protected void Start()
    {
        if (Duration >= 0)
        {
            StartCoroutine(DestroyAfterDuration(Duration));
        }
    }

    public bool UseAbility(AbilityUser user)
    {
        Vector3 SpawnOffset = Vector3.zero;
        SpawnOffset += user.WheelCenter.transform.right * SpawnOffsetFromCaster.x;
        SpawnOffset += user.WheelCenter.transform.up * SpawnOffsetFromCaster.y;
        SpawnOffset += user.WheelCenter.transform.forward * SpawnOffsetFromCaster.z;

        Ability ability;
        if (SpawnAsCasterChild)
        {
            ability = Instantiate(this, user.transform);
            ability.transform.position = user.WheelCenter.transform.position + SpawnOffset;
        }
        else
        {
            ability = Instantiate(this, user.WheelCenter.transform.position + SpawnOffset, user.WheelCenter.transform.rotation);
        }

        ability.Caster = user;
        return true;
    }

    public IEnumerator DestroyAfterDuration(float duration)
    {
        if (duration >= 0)
        {
            yield return new WaitForSeconds(duration);
            Destroy(gameObject);
        }
    }
}
