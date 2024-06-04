using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public enum ESpawnParent { None = 0, Wheel = 1, WheelCenter = 2, EnemyWheel = 3, EnemyWheelCenter = 4 };
public class SpawningAbility : AAbility
{
    public ESpawnParent SpawnParent;
    public Vector3 SpawnOffsetFromWheelCenter;

    public override bool UseAbility(AbilityUser user)
    {
        AbilityUser target;
        Vector3 SpawnOffset = Vector3.zero;
        switch (SpawnParent)
        {
            case ESpawnParent.EnemyWheel:
            case ESpawnParent.EnemyWheelCenter:
                target = GameManager.Instance.GetOtherPlayerAbilityUser(user);
                break;
            case ESpawnParent.None:
            case ESpawnParent.Wheel:
            case ESpawnParent.WheelCenter:
            default:
                target = user;
                break;
        }
        if (target == null)
        {
            return false;
        }
        SpawnOffset += target.WheelCenter.transform.right * SpawnOffsetFromWheelCenter.x;
        SpawnOffset += target.WheelCenter.transform.up * SpawnOffsetFromWheelCenter.y;
        SpawnOffset += target.WheelCenter.transform.forward * SpawnOffsetFromWheelCenter.z;

        AAbility ability;
        switch (SpawnParent)
        {
            case ESpawnParent.Wheel:
            case ESpawnParent.EnemyWheel:
                ability = Instantiate(this, target.transform);
                ability.transform.position = target.WheelCenter.transform.position + SpawnOffset;
                break;
            case ESpawnParent.WheelCenter:
            case ESpawnParent.EnemyWheelCenter:
                ability = Instantiate(this, target.WheelCenter.transform);
                ability.transform.position = target.WheelCenter.transform.position + SpawnOffset;
                break;
            case ESpawnParent.None:
            default:
                ability = Instantiate(this, target.WheelCenter.transform.position + SpawnOffset, user.WheelCenter.transform.rotation);
                break;
        }

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
