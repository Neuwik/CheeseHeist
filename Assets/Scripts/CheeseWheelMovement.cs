using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class CheeseWheelMovement : MonoBehaviour
{
    private Rigidbody rb;
    private float movementForward;
    private float movementTurn;
    private float honeyModifier = 1;

    public float ForwardSpeed = 1;
    public float TurnSpeed = 1;
    public float SlownessEffect = 1;

    public LayerMask GroundLayer;
    public Vector3 Down = Vector3.down;
    public Vector3 Forward = Vector3.forward;
    public GameObject WheelCenter;
    public GameObject ResetPoint;
    public Vector3 ResetPositionOffset = new Vector3(0,2,0);
    public float AutoResetAngle = 10;

    private bool controlsInverted = false;

    void Start()
    {
        ResetPoint = transform.parent.gameObject;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Apply control inversion if active
        float actualMovementTurn = controlsInverted ? -movementTurn : movementTurn;
        float actualMovementForward = controlsInverted ? -movementForward : movementForward;

        Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, movementTurn * TurnSpeed * Time.fixedDeltaTime, 0));
        Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, movementTurn * TurnSpeed * Time.fixedDeltaTime * honeyModifier, 0 ));
        rb.MoveRotation(deltaRotation * transform.rotation);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, GroundLayer.value))
        {
            Down = hit.transform.up * -1;
        }
        Debug.DrawRay(transform.position, Down, Color.yellow, Time.fixedDeltaTime);

        Forward = Vector3.Cross(transform.up, Down).normalized;
        // Debug.DrawRay(transform.position, Forward, Color.blue, Time.fixedDeltaTime);

        rb.AddForce(Forward * movementForward * ForwardSpeed * Time.fixedDeltaTime * rb.mass * honeyModifier);

        //Camera
        WheelCenter.transform.position = transform.position;
        WheelCenter.transform.LookAt(transform.position + Forward);

        float angle = Vector3.Angle(Down, transform.up);
        //Debug.Log(angle);
        if (angle + AutoResetAngle >= 180 || angle - AutoResetAngle <= 0)
        {
            ResetPosition();
        }
    }

    //private void OnMove(InputValue movementValue)
    //{
    //    Vector2 movementVector = movementValue.Get<Vector2>();

    //    movementForward = movementVector.y;
    //    movementTurn = movementVector.x;
       
    //}


    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        // Check if controls are inverted
        if (controlsInverted)
        {
            // Invert the movement input
            movementForward = -movementVector.y; // Inverts forward/backward (W/S or Up/Down)
            movementTurn = -movementVector.x; // Inverts turning left/right (A/D or Left/Right)
        }
        else
        {
            movementForward = movementVector.y;
            movementTurn = movementVector.x;
        }
    }

    private void OnResetPosition()
    {
        ResetPosition();
    }

    private void ResetPosition()
    {
        if (!FinishLine.isFinished)  // Check if the game has ended
        {
            rb.Sleep();  // Stop all physics activity
            transform.position = ResetPoint.transform.position + ResetPositionOffset;  // Reset position
            transform.LookAt(transform.position + ResetPoint.transform.forward);  // Reset orientation
            transform.Rotate(transform.forward, 90);  // Correct rotation to original setup
        }
        else
        {
            Debug.Log("Reset not allowed. Game has finished.");
        }
    }


    // Method to invert controls
    public void InvertControls(float duration)
    {
        StartCoroutine(InvertControlsRoutine(duration));
    }

    // Coroutine to handle control inversion duration
    private IEnumerator InvertControlsRoutine(float duration)
    {
        controlsInverted = true;
        yield return new WaitForSeconds(duration);
        controlsInverted = false;
    }
    public void ApplySlowness(float timer)
    {
        StartCoroutine(CalcSlowness(timer));
    }
    private IEnumerator CalcSlowness(float timer)
    {
        if(honeyModifier != 1.0f)
        {
            yield return new WaitUntil(() => honeyModifier == 1.0f);
        }
        rb.drag += 1.5f;
        honeyModifier = 0.70f;
        Debug.Log($"vorm wait");
        yield return new WaitForSeconds(timer);
        Debug.Log($"nach wait");
        honeyModifier = 1.0f;
        rb.drag -= 1.5f;
    }
}
