using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public enum ESpawnParent { None = 0, WheelRotating = 1, WheelStable = 2, EnemyWheelRotating = 3, EnemyWheelStable = 4 };
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
            case ESpawnParent.EnemyWheelRotating:
            case ESpawnParent.EnemyWheelStable:
                target = GameManager.Instance.GetOtherPlayerAbilityUser(user);
                break;
            case ESpawnParent.None:
            case ESpawnParent.WheelRotating:
            case ESpawnParent.WheelStable:
            default:
                target = user;
                break;
        }
        if (target == null)
        {
            return false;
        }
        SpawnOffset += target.WheelStable.transform.right * SpawnOffsetFromWheelCenter.x;
        SpawnOffset += target.WheelStable.transform.up * SpawnOffsetFromWheelCenter.y;
        SpawnOffset += target.WheelStable.transform.forward * SpawnOffsetFromWheelCenter.z;

        AAbility ability;
        switch (SpawnParent)
        {
            case ESpawnParent.WheelRotating:
            case ESpawnParent.EnemyWheelRotating:
                ability = Instantiate(this, target.WheelRotating.transform);
                ability.transform.position = target.WheelStable.transform.position + SpawnOffset;
                break;
            case ESpawnParent.WheelStable:
            case ESpawnParent.EnemyWheelStable:
                ability = Instantiate(this, target.WheelStable.transform);
                ability.transform.position = target.WheelStable.transform.position + SpawnOffset;
                break;
            case ESpawnParent.None:
            default:
                ability = Instantiate(this, target.WheelStable.transform.position + SpawnOffset, user.WheelStable.transform.rotation);
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
