using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CheeseWheelControllsMapper : MonoBehaviour
{
    public CheeseWheelMovement Movement;
    public AbilityUser AbilityUser;

    private void Start()
    {
        if (Movement == null)
        {
            Movement = gameObject.GetComponentInChildren<CheeseWheelMovement>();
        }
        if (AbilityUser == null)
        {
            AbilityUser = gameObject.GetComponentInChildren<AbilityUser>();
        }
    }

    public void OnMove(InputValue movementValue)
    {
        if (Movement != null)
        {
            Movement.OnMove(movementValue);
        }
    }

    public void OnResetPosition()
    {
        if (Movement != null)
        {
            Movement.OnResetPosition();
        }
    }

    public void OnUseAbility()
    {
        if (AbilityUser != null)
        {
            AbilityUser.OnUseAbility();
        }
    }
}
