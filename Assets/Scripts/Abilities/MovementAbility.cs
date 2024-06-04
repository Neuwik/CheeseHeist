using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EMovementAbility { None = 0, InvertControlls = 1, Jump = 2 };
public class MovementAbility : SpawningAbility
{
    public bool UseOnSelf = false;
    public EMovementAbility Ability;
    public float Power = 10;
    private CheeseWheelMovement Target;

    protected new void Start()
    {
        base.Start();

        if (UseOnSelf)
        {
            Target = GameManager.Instance.GetPlayerWheelMovement(Caster);
        }
        else
        {
            Target = GameManager.Instance.GetOtherPlayerWheelMovement(Caster);
        }

        if (Target == null)
        {
            Destroy(gameObject);
        }

        switch (Ability)
        {
            case EMovementAbility.InvertControlls:
                Target.InvertControls(Duration);
                break;
            case EMovementAbility.Jump:
                Target.Jump(Power);
                break;
            default:
                Debug.Log("Movement Ability not implemented");
                Destroy(gameObject);
                break;
        }
    }
}
