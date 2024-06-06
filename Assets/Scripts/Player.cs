using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

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
                Avatar?.SetInteger("StateEnum", (int)_state);
                Avatar?.transform.LookAt(GameManager.Instance.Lobby.LobbyCenter);

                if (currentICC != null && currentICC.Canvas != null)
                {
                    currentICC.Canvas.enabled = false;
                }

                currentICC = InputCameraCanvasMaps.Find(m => m.State == _state);
                if (currentICC != null)
                {
                    if (currentICC.Camera != null)
                    {
                        PlayerCamera.enabled = false;
                        Transform newCameraTransform = currentICC.Camera.transform;
                        PlayerCamera.transform.position = newCameraTransform.position;
                        PlayerCamera.transform.rotation = newCameraTransform.rotation;
                        PlayerCamera.transform.parent = newCameraTransform.parent;
                        PlayerCamera.enabled = true;
                    }
                    else
                    {
                        PlayerCamera.enabled = false;
                        Transform newCameraTransform = GameManager.Instance.Lobby.LobbyCamera.transform;
                        PlayerCamera.transform.position = newCameraTransform.position + _playerOffset;
                        PlayerCamera.transform.rotation = newCameraTransform.rotation;
                        PlayerCamera.transform.parent = newCameraTransform.parent;
                        PlayerCamera.enabled = true;
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
                        currentICC.Canvas.enabled = true;
                    }
                }
                else
                {
                    PlayerCamera.enabled = false;
                    Inputs.SwitchCurrentActionMap(Inputs.defaultActionMap);
                }

                OnStateChanged?.Invoke(this, _state);
            }
        }
    }

    private Vector3 _playerOffset = Vector3.zero;

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

    private CheeseMass SelectedCheeseMass = new CheeseMass(20, new Vector3(0.1f, 0, 0.3f));
    private MassController _massController;
    public CheeseMass CurrentCheeseMass { get { return _massController.CheeseMass; } }

    private void Start()
    {
        GameManager.Instance.OnStateChanged += OnGameManagerStateChanged;
        _massController = WheelMovement.GetComponent<MassController>();

        if (Inputs.playerIndex % 2 == 0)
        {
            _playerOffset = Vector3.left;
        }
        else
        {
            _playerOffset = Vector3.right;
        }

        float playerMult = (float)Math.Floor((double)Inputs.playerIndex / 2);
        _playerOffset *= (playerMult + 1);

        State = EPlayerState.Waiting;
    }

    public void SpawnCheeseWheel()
    {
        WheelMovement.SetPlayerSpecificResetPositionOffset(_playerOffset);
        WheelMovement.SetResetPoint(GameManager.Instance.StartPoint.gameObject);

        _massController.CheeseMass = SelectedCheeseMass;

        CheeseWheel.gameObject.SetActive(true);
        WheelMovement.ResetPosition();

        State = EPlayerState.Wheel;
    }

    public void OnGameManagerStateChanged(EGameManagerState newGMState)
    {
        switch (newGMState)
        {
            case EGameManagerState.WatingRoom:
                if (State != EPlayerState.Waiting && State != EPlayerState.Ready)
                {
                    Points = 0;
                    State = EPlayerState.Waiting;
                }
                break;
            case EGameManagerState.Racing:
                if (State != EPlayerState.Wheel && State != EPlayerState.Racing && State != EPlayerState.UI)
                {
                    ResetCheeseWheelAndChooseNext();
                    Points = 0;
                    State = EPlayerState.UI;
                }
                break;
            case EGameManagerState.Finished:
                if (State != EPlayerState.Finished && State != EPlayerState.Won && State != EPlayerState.Lost)
                {
                    ResetCheeseWheel();
                    if (GameManager.Instance.HaveIWon(this))
                    {
                        State = EPlayerState.Won;
                    }
                    else
                    {
                        State = EPlayerState.Lost;
                    }
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
        if (State == EPlayerState.UI)
        {
            SpawnCheeseWheel();
            return;
        }
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

    public void ResetCheeseWheelAndChooseNext()
    {
        ResetCheeseWheel();
        State = EPlayerState.UI;
    }

    public void ResetCheeseWheel()
    {
        CheeseWheel.gameObject.SetActive(false);
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
