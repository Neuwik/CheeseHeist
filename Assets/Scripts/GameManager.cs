using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public struct PlayerComponents
{
    private GameObject _object; //The Physics object
    public GameObject Object
    {
        get { return _object; }
        set
        {
            _object = value;
            Inputs = _object.GetComponent<PlayerInput>();
            WheelMovement = _object.GetComponent<CheeseWheelMovement>();
        }
    }
    public PlayerInput Inputs { get; private set; }
    public CheeseWheelMovement WheelMovement { get; private set; }
}

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
        StartCoroutine(WaitingForPlayers());
    }

    public GameObject StartPoint;

    #region Multiplayer
    public GameObject WaitingScreen;

    public PlayerComponents Player1;
    public PlayerComponents Player2;

    public void OnPlayerJoined(PlayerInput player)
    {
        Debug.Log("Player joined " + player.gameObject.name);
        player.DeactivateInput();
        if (Player1.Object == null)
        {
            Player1.Object = player.gameObject;
            Player1.WheelMovement.SetPlayerSpecificResetPositionOffset(Vector3.left * 2);
            Player1.WheelMovement.SetResetPoint(StartPoint);
        }
        else if (Player2.Object == null)
        {
            Player2.Object = player.gameObject;
            Player2.WheelMovement.SetPlayerSpecificResetPositionOffset(Vector3.right * 2);
            Player2.WheelMovement.SetResetPoint(StartPoint);
        }
        else
        {
            Debug.LogWarning("Another Player tried to connect.");
        }
    }

    public IEnumerator WaitingForPlayers()
    {
        WaitingScreen.SetActive(true);
        yield return new WaitUntil(() => Player1.Inputs != null && Player2.Inputs != null);
        WaitingScreen.SetActive(false);
        Player1.Inputs.ActivateInput();
        Player2.Inputs.ActivateInput();
        Player1.WheelMovement.ResetPosition();
        Player2.WheelMovement.ResetPosition();
    }

    public CheeseWheelMovement GetPlayerWheelMovement(GameObject player)
    {
        return GetPlayer(player).WheelMovement;
    }

    public GameObject GetPlayerObject(GameObject player)
    {
        return GetPlayer(player).Object;
    }

    public PlayerComponents GetPlayer(GameObject player)
    {
        if (player == null)
        {
            return new PlayerComponents();
        }

        if (player == Player1.Object)
        {
            return Player1;
        }
        if (player == Player2.Object)
        {
            return Player2;
        }

        return new PlayerComponents();
    }

    public CheeseWheelMovement GetOtherPlayerWheelMovement(GameObject player)
    {
        return GetOtherPlayer(player).WheelMovement;
    }

    public GameObject GetOtherPlayerObject(GameObject player)
    {
        return GetOtherPlayer(player).Object;
    }

    public PlayerComponents GetOtherPlayer(GameObject player)
    {
        if (player == null)
        {
            return new PlayerComponents();
        }

        if (player == Player1.Object)
        {
            return Player2;
        }
        if (player == Player2.Object)
        {
            return Player1;
        }

        return new PlayerComponents();
    }

    #endregion

    #region Abilities
    public List<AAbility> AbilityPrefabs;

    public AAbility GetRandomAbility()
    {
        if (AbilityPrefabs != null)
        {
            return AbilityPrefabs[Random.Range(0, AbilityPrefabs.Count)];
        }
        return null;
    }
    #endregion
}
