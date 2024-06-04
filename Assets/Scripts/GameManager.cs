using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

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

    [HideInInspector]
    public Player Player1;
    [HideInInspector]
    public Player Player2;
    public GameObject WaitingScreen;

    #region Multiplayer
    public void OnPlayerJoined(PlayerInput player)
    {
        Debug.Log("Player joined " + player.gameObject.name);
        if (Player1 == null)
        {
            Player1 = player.GetComponent<Player>();
        }
        else if (Player2 == null)
        {
            Player2 = player.GetComponent<Player>();
        }
        else
        {
            Debug.LogWarning("Another Player tried to connect.");
        }
    }

    public IEnumerator WaitingForPlayers()
    {
        WaitingScreen.SetActive(true);
        yield return new WaitUntil(() => Player1 != null && Player2 != null);
        WaitingScreen.SetActive(false);
        Player1.SpawnCheeseWheel(StartPoint);
        Player2.SpawnCheeseWheel(StartPoint);
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
            return AbilityPrefabs[Random.Range(0, AbilityPrefabs.Count)];
        }
        return null;
    }
    #endregion
}
