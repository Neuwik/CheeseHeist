using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.CullingGroup;

public enum EGameManagerState { None = 0, WatingRoom = 1, Starting = 2, Racing = 11, Finished = 21 }

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("Game Manger is NULL");
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnSceneReloaded;
    }

    private void Start()
    {
        ReloadReferences();

        if (Testing)
        {
            State = TestState;
        }
        else
        {
            WaitingForPlayersToJoin();
        }
    }

    private EGameManagerState _state;
    public EGameManagerState State
    {
        get { return _state; }
        set
        {
            if (Testing)
            {
                _state = TestState;
                OnStateChanged?.Invoke(_state);
            }
            else if (_state != value)
            {
                _state = value;
                if (_state == EGameManagerState.WatingRoom)
                {
                    _deliveredCheeseMass = 0;
                }
                OnStateChanged?.Invoke(_state);
            }
        }
    }
    
    public float CheeseMassNeeded;
    public int StartCountdownLength;
    [HideInInspector]
    public Player Player1;
    [HideInInspector]
    public Player Player2;
    public List<AAbility> AbilityPrefabs;

    [Header("Level Start Point")]
    public StartPoint StartPoint;
    private float _deliveredCheeseMass;

    public Lobby Lobby;

    [Header("State you want to Test (None => play normaly")]
    public EGameManagerState TestState;
    public bool Testing { get => TestState != EGameManagerState.None; }

    private IEnumerator _startCountdown;

    public event Action<EGameManagerState> OnStateChanged;

    public void OnPlayerChangedState(Player player, EPlayerState newPlayerState)
    {
        if (Testing)
        {
            State = TestState;
            return;
        }
        switch (State)
        {
            case EGameManagerState.WatingRoom:
                if (Player1 != null && Player2 != null)
                {
                    if (Player1.State == EPlayerState.Ready && Player2.State == EPlayerState.Ready)
                    {
                        _startCountdown = StartRace();
                        StartCoroutine(_startCountdown);
                    }
                    else
                    {
                        WaitingForPlayersToGetReady();
                    }
                }
                else
                {
                    WaitingForPlayersToJoin();
                }
                break;
            case EGameManagerState.Starting:
                if (Player1 != null && Player2 != null)
                {
                    if (Player1.State != EPlayerState.Ready || Player2.State != EPlayerState.Ready)
                    {
                        StopCoroutine(_startCountdown);
                        WaitingForPlayersToGetReady();
                    }
                }
                else
                {
                    StopCoroutine(_startCountdown);
                    WaitingForPlayersToJoin();
                }
                break;
            case EGameManagerState.Racing:
                break;
            case EGameManagerState.Finished:
                if (newPlayerState == EPlayerState.Won)
                {
                    Lobby.ResultPanel.GetComponentInChildren<TMP_Text>().text = $"Player{player.Inputs.playerIndex + 1} has won!";
                    Lobby.ResultPanel.SetActive(true);
                }
                if (newPlayerState == EPlayerState.Waiting)
                {
                    ResetGame();
                }
                break;
            case EGameManagerState.None:
            default:
                break;
        }
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log($"Player{playerInput.playerIndex + 1} joined.");
        if (Player1 == null)
        {
            playerInput.deviceLostEvent.AddListener(OnPlayerLeft);
            Player1 = playerInput.GetComponent<Player>();
            Player1.Avatar = Lobby.Player1Avatar;
            Lobby.Player1Avatar.gameObject.SetActive(true);
            Player1.OnStateChanged += OnPlayerChangedState;
        }
        else if (Player2 == null)
        {
            playerInput.deviceLostEvent.AddListener(OnPlayerLeft);
            Player2 = playerInput.GetComponent<Player>();
            Player2.Avatar = Lobby.Player2Avatar;
            Lobby.Player2Avatar.gameObject.SetActive(true);
            Player2.OnStateChanged += OnPlayerChangedState;
        }
        else
        {
            Debug.LogWarning("Another Player tried to connect.");
        }

        if(Testing)
        {
            State = TestState;
        }
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        // never gets triggered, not even with playerInput.deviceLostEvent.AddListener(OnPlayerLeft);
        Debug.Log($"Player{playerInput.playerIndex + 1} left.");

        /*
        Player player = null;
        Animator avatar = null;

        if (Player1 != null && Player1.Inputs == playerInput)
        {
            player = Player1;
        }
        else if (Player2 != null && Player2.Inputs == playerInput)
        {
            player = Player2;
        }
        else
        {
            Debug.LogWarning("Another Player left");
            return;
        }

        switch (State)
        {
            case EGameManagerState.WatingRoom:
                Destroy(player.gameObject);
                avatar?.gameObject?.SetActive(false);
                break;
            case EGameManagerState.Starting:
                Destroy(player.gameObject);
                State = EGameManagerState.WatingRoom;
                avatar?.gameObject?.SetActive(false);
                break;
            case EGameManagerState.Racing:
                State = EGameManagerState.Finished;
                avatar?.gameObject?.SetActive(false);
                break;
            case EGameManagerState.Finished:
                Destroy(player.gameObject);
                State = EGameManagerState.WatingRoom;
                avatar?.gameObject?.SetActive(false);
                break;
            case EGameManagerState.None:
            default:
                break;
        }
        */
    }

    public void WaitingForPlayersToJoin()
    {
        State = EGameManagerState.WatingRoom;
        Lobby.StartCountdownPanel.SetActive(false);
        Lobby.WaitingForPlayerPanel.SetActive(true);
        Lobby.WaitingForReadyPanel.SetActive(false);
    }

    public void WaitingForPlayersToGetReady()
    {
        State = EGameManagerState.WatingRoom;
        Lobby.ResultPanel.SetActive(false);
        Lobby.StartCountdownPanel.SetActive(false);
        Lobby.WaitingForPlayerPanel.SetActive(false);
        Lobby.WaitingForReadyPanel.SetActive(true);
    }

    public void ReloadReferences()
    {
        OnStateChanged = null;

        //Debug.Log("Reload References");
        StartPoint = FindAnyObjectByType<StartPoint>();

        Lobby = FindAnyObjectByType<Lobby>();
        Lobby.WaitingForPlayerPanel.SetActive(false);
        Lobby.WaitingForReadyPanel.SetActive(false);
        Lobby.StartCountdownPanel.SetActive(false);
        Lobby.ResultPanel.SetActive(false);
        Lobby.Player1Avatar.gameObject.SetActive(false);
        Lobby.Player2Avatar.gameObject.SetActive(false);
    }

    public IEnumerator StartRace()
    {
        State = EGameManagerState.Starting;
        Lobby.WaitingForReadyPanel.SetActive(false);
        Lobby.StartCountdownPanel.SetActive(true);
        for (int i = StartCountdownLength; i > 0; i--)
        {
            Lobby.StartCountdownPanel.GetComponentInChildren<TMP_Text>().text = $"{i}";
            yield return new WaitForSeconds(1);
        }
        Lobby.StartCountdownPanel.GetComponentInChildren<TMP_Text>().text = "Go!";
        yield return new WaitForSeconds(1);
        State = EGameManagerState.Racing;
        Lobby.StartCountdownPanel.SetActive(false);
    }

    private void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
    }

    private void OnSceneReloaded(Scene scene, LoadSceneMode mode)
    {
        ReloadReferences();
        WaitingForPlayersToGetReady();
    }


    #region Multiplayer
    public CheeseWheelMovement GetPlayerWheelMovement(AbilityUser playerAbilityUser)
    {
        if (playerAbilityUser != null)
        {
            if (playerAbilityUser == Player1.WheelAbilityUser)
            {
                return Player1.WheelMovement;
            }
            if (playerAbilityUser == Player2.WheelAbilityUser)
            {
                return Player2.WheelMovement;
            }
        }

        return null;
    }

    public CheeseWheelMovement GetPlayerWheelMovement(GameObject player)
    {
        return GetPlayer(player).WheelMovement;
    }

    public Player GetPlayer(GameObject player)
    {
        if (player != null)
        {
            if (player == Player1)
            {
                return Player1;
            }
            if (player == Player2)
            {
                return Player2;
            }
        }

        return null;
    }
    public Player GetPlayer(CheeseWheelMovement movement)
    {
        if (movement != null)
        {
            if (movement == Player1.WheelMovement)
            {
                return Player1;
            }
            if (movement == Player2.WheelMovement)
            {
                return Player2;
            }
        }

        return null;
    }

    public CheeseWheelMovement GetOtherPlayerWheelMovement(AbilityUser playerAbilityUser)
    {
        if (playerAbilityUser != null)
        {
            if (playerAbilityUser == Player1.WheelAbilityUser)
            {
                return Player2.WheelMovement;
            }
            if (playerAbilityUser == Player2.WheelAbilityUser)
            {
                return Player1.WheelMovement;
            }
        }

        return null;
    }

    public AbilityUser GetOtherPlayerAbilityUser(AbilityUser playerAbilityUser)
    {
        if (playerAbilityUser != null)
        {
            if (playerAbilityUser == Player1.WheelAbilityUser)
            {
                return Player2.WheelAbilityUser;
            }
            if (playerAbilityUser == Player2.WheelAbilityUser)
            {
                return Player1.WheelAbilityUser;
            }
        }

        return null;
    }

    public CheeseWheelMovement GetOtherPlayerWheelMovement(GameObject player)
    {
        return GetOtherPlayer(player).WheelMovement;
    }

    public Player GetOtherPlayer(GameObject player)
    {
        if (player != null)
        {
            if (player == Player1)
            {
                return Player2;
            }
            if (player == Player2)
            {
                return Player1;
            }
        }

        return null;
    }

    #endregion

    #region Abilities
    public AAbility GetRandomAbility()
    {
        if (AbilityPrefabs != null)
        {
            return AbilityPrefabs[UnityEngine.Random.Range(0, AbilityPrefabs.Count)];
        }
        return null;
    }
    #endregion

    public void DeliveredCheese(Player player, CheeseMass cheese)
    {
        // Calculate cheese worth
        float cheeseWorth = cheese.Mass;

        if (cheeseWorth + _deliveredCheeseMass > CheeseMassNeeded)
        {
            cheeseWorth = CheeseMassNeeded - _deliveredCheeseMass;
        }

        player.GainPoints(cheeseWorth);
        _deliveredCheeseMass += cheeseWorth;


        if (_deliveredCheeseMass >= CheeseMassNeeded)
        {
            State = EGameManagerState.Finished;
        }
    }

    public bool HaveIWon(Player player)
    {
        return player.Points >= Player1.Points && player.Points >= Player2.Points;
    }
}
