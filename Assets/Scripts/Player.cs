using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using static UnityEngine.CullingGroup;

public enum EPlayerState { None = 0, Waiting = 1, Ready = 2, Racing = 10, UI = 11, Wheel = 12, Finished = 20, Won = 21, Lost = 22 }

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

    public PlayerInput Inputs;

    public Camera PlayerCamera;

    public event Action<Player, EPlayerState> OnStateChanged;

    private EPlayerState _state;
    public EPlayerState State
    {
        get { return _state; }
        private set
        {
            if (_state != value)
            {
                _state = value;
                currentICC = InputCameraCanvasMaps.Find(m => m.State == _state);
                Avatar?.SetInteger("StateEnum", (int)_state);

                OnStateChanged?.Invoke(this, _state);

                if (currentICC != null)
                {
                    if (currentICC.Camera != null)
                    {
                        PlayerCamera.enabled = true;
                        Transform newCameraTransform = currentICC.Camera.transform;
                        PlayerCamera.transform.position = newCameraTransform.position;
                        PlayerCamera.transform.rotation = newCameraTransform.rotation;
                        PlayerCamera.transform.parent = newCameraTransform.parent;
                        PlayerCamera.enabled = true;
                    }
                    else
                    {
                        PlayerCamera.enabled = false;
                    }

                    if (!String.IsNullOrEmpty(currentICC.InputActionMapName))
                    {
                        Inputs.SwitchCurrentActionMap(currentICC.InputActionMapName);
                    }
                    else
                    {
                        Inputs.SwitchCurrentActionMap(Inputs.defaultActionMap);
                    }

                    if (currentICC.Canvas != null)
                    {
                        currentICC.Canvas.worldCamera = PlayerCamera;
                    }
                }
            }
        }
    }

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

    public CheeseWheelMovement WheelMovement { get { return WheelControllsMapper?.Movement; } }
    public AbilityUser WheelAbilityUser { get { return WheelControllsMapper?.AbilityUser; } }

    [HideInInspector]
    public Animator Avatar;

    public float Points { get; private set; }

    private void Start()
    {
        State = EPlayerState.Waiting;
        GameManager.Instance.OnStateChanged += OnGameManagerStateChanged;
    }

    public void SpawnCheeseWheel()
    {
        if (Inputs.playerIndex % 2 == 0)
        {
            WheelMovement.SetPlayerSpecificResetPositionOffset(Vector3.left * 2 * (Inputs.playerIndex + 1));
        }
        else
        {
            WheelMovement.SetPlayerSpecificResetPositionOffset(Vector3.right * 2 * (Inputs.playerIndex + 1));
        }

        WheelMovement.SetResetPoint(GameManager.Instance.StartPoint);
        CheeseWheel.gameObject.SetActive(true);
        WheelMovement.ResetPosition();

        State = EPlayerState.Wheel;
    }

    public void OnGameManagerStateChanged(EGameManagerState newGMState)
    {
        switch (newGMState)
        {
            case EGameManagerState.WatingRoom:
                State = EPlayerState.Waiting;
                break;
            case EGameManagerState.Racing:
                State = EPlayerState.Racing;
                SpawnCheeseWheel();
                break;
            case EGameManagerState.Finished:
                if (GameManager.Instance.HaveIWon(this))
                {
                    State = EPlayerState.Won;
                }
                else
                {
                    State = EPlayerState.Lost;
                }
                break;
            case EGameManagerState.None:
            default:
                break;
        }
    }

    public void GainPoints(float gain)
    {
        Points += gain;
    }

    #region UIInputs

    public void OnSubmit()
    {
        if (State == EPlayerState.Waiting)
        {
            State = EPlayerState.Ready;
            return;
        }
        if (State == EPlayerState.Ready)
        {
            State = EPlayerState.Waiting;
            return;
        }
        if (State == EPlayerState.Won)
        {
            State = EPlayerState.Waiting;
            return;
        }
    }

    #endregion


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
