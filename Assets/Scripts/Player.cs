using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public enum EPlayerState { UI = 0, Wheel = 1 }

[Serializable]
public class PlayerStateInputCameraCanvasMapping
{
    public EPlayerState State;
    public string InputActionMapName;
    public Camera Camera;
    public Canvas Canvas;
}

public class Player : MonoBehaviour
{
    [SerializeField]
    public List<PlayerStateInputCameraCanvasMapping> InputCameraCanvasMaps;
    private PlayerStateInputCameraCanvasMapping currentICC;

    public GameObject CheeseWheel;

    private CheeseWheelControllsMapper _wheelControllsMapper;
    public CheeseWheelControllsMapper WheelControllsMapper
    {
        get
        {
            if (_wheelControllsMapper == null)
                _wheelControllsMapper = CheeseWheel.GetComponent<CheeseWheelControllsMapper>();
            return _wheelControllsMapper;
        }
    }


    public PlayerInput Inputs;

    public Camera PlayerCamera;

    private EPlayerState _state;
    public EPlayerState State
    {
        get { return _state; }
        private set
        {
            _state = value;
            currentICC = InputCameraCanvasMaps.Find(m => m.State == _state);

            if (currentICC != null)
            {
                if (currentICC.Camera != null)
                {
                    Transform newCameraTransform = currentICC.Camera.transform;
                    PlayerCamera.transform.position = newCameraTransform.position;
                    PlayerCamera.transform.rotation = newCameraTransform.rotation;
                    PlayerCamera.transform.parent = newCameraTransform.parent;
                }
                if (!String.IsNullOrEmpty(currentICC.InputActionMapName))
                {
                    Inputs.SwitchCurrentActionMap(currentICC.InputActionMapName);
                }
                if (currentICC.Canvas != null)
                {
                    currentICC.Canvas.worldCamera = PlayerCamera;
                }
            }
        }
    }

    public CheeseWheelMovement WheelMovement { get { return WheelControllsMapper?.Movement; } }
    public AbilityUser WheelAbilityUser { get { return WheelControllsMapper?.AbilityUser; } }


    private void Start()
    {
        State = EPlayerState.UI;
    }

    public void SpawnCheeseWheel(GameObject StartPoint)
    {
        if (Inputs.playerIndex % 2 == 0)
        {
            WheelMovement.SetPlayerSpecificResetPositionOffset(Vector3.left * 2 * (Inputs.playerIndex + 1));
        }
        else
        {
            WheelMovement.SetPlayerSpecificResetPositionOffset(Vector3.right * 2 * (Inputs.playerIndex + 1));
        }

        WheelMovement.SetResetPoint(StartPoint);
        CheeseWheel.gameObject.SetActive(true);
        WheelMovement.ResetPosition();

        State = EPlayerState.Wheel;
    }

    #region CheeseWheelInputMap

    public void OnMove(InputValue movementValue)
    {
        if (WheelControllsMapper != null)
        {
            WheelControllsMapper.OnMove(movementValue);
        }
    }

    public void OnResetPosition()
    {
        if (WheelControllsMapper != null)
        {
            WheelControllsMapper.OnResetPosition();
        }
    }

    public void OnUseAbility()
    {
        if (WheelControllsMapper != null)
        {
            WheelControllsMapper.OnUseAbility();
        }
    }

    #endregion

}
