using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public struct PlayerComponents
{
    private GameObject _object;
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
        yield return new WaitUntil(() => Player1.Inputs != null && Player2.Inputs != null || 1 == 1);
        WaitingScreen.SetActive(false);
        Player1.Inputs.ActivateInput();
        //Player2.Inputs.ActivateInput();
        //Player1.WheelMovement.ResetPosition();
        //Player2.WheelMovement.ResetPosition();
    }
}
