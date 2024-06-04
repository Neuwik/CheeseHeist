using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum EGameManagerState { None = 0, WatingRoom = 1, Racing = 11, Finished = 21 }

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
    }

    private void Start()
    {
        WaitingForPlayerPanel.SetActive(false);
        WaitingForReadyPanel.SetActive(false);
        ResultPanel.SetActive(false);
        WaitingForPlayersToJoin();
    }

    private EGameManagerState _state;
    public EGameManagerState State
    {
        get { return _state; }
        set
        {
            if (_state != value)
            {
                _state = value;
                OnStateChanged?.Invoke(_state);
            }
        }
    }

    public GameObject StartPoint;
    public float CheeseMassNeeded;
    private float _deliveredCheeseMass;

    [HideInInspector]
    public Player Player1;
    [HideInInspector]
    public Player Player2;

    public GameObject WaitingForPlayerPanel;
    public GameObject WaitingForReadyPanel;
    public GameObject ResultPanel;

    #region Multiplayer

    public event Action<EGameManagerState> OnStateChanged;

    public void OnPlayerChangedState(Player player, EPlayerState newPlayerState)
    {
        switch (State)
        {
            case EGameManagerState.WatingRoom:
                if (Player1 != null && Player2 != null)
                {
                    if (Player1.State == EPlayerState.Ready && Player2.State == EPlayerState.Ready)
                    {
                        StartRace();
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
            case EGameManagerState.Racing:
                break;
            case EGameManagerState.Finished:
                if (newPlayerState == EPlayerState.Won)
                {
                    ResultPanel.GetComponentInChildren<TMP_Text>().text = ResultPanel.GetComponentInChildren<TMP_Text>().text.Replace("{winner}", $"Player{player.Inputs.playerIndex + 1}");
                    ResultPanel.SetActive(true);
                }
                break;
            case EGameManagerState.None:
            default:
                break;
        }
    }

    public void OnPlayerJoined(PlayerInput player)
    {
        Debug.Log("Player joined " + player.gameObject.name);
        if (Player1 == null)
        {
            Player1 = player.GetComponent<Player>();
            Player1.OnStateChanged += OnPlayerChangedState;
        }
        else if (Player2 == null)
        {
            Player2 = player.GetComponent<Player>();
            Player2.OnStateChanged += OnPlayerChangedState;
        }
        else
        {
            Debug.LogWarning("Another Player tried to connect.");
        }
    }

    public void WaitingForPlayersToJoin()
    {
        State = EGameManagerState.WatingRoom;
        WaitingForPlayerPanel.SetActive(true);
        WaitingForReadyPanel.SetActive(false);
    }

    public void WaitingForPlayersToGetReady()
    {
        WaitingForPlayerPanel.SetActive(false);
        WaitingForReadyPanel.SetActive(true);
    }

    public void StartRace()
    {
        WaitingForReadyPanel.SetActive(false);
        State = EGameManagerState.Racing;
    }

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
    public List<AAbility> AbilityPrefabs;

    public AAbility GetRandomAbility()
    {
        if (AbilityPrefabs != null)
        {
            return AbilityPrefabs[UnityEngine.Random.Range(0, AbilityPrefabs.Count)];
        }
        return null;
    }
    #endregion

    public void DeliveredCheese(Player player, MassController cheese)
    {
        // Calculate cheese worth
        float cheeseWorth = cheese.CurrentMass;

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
