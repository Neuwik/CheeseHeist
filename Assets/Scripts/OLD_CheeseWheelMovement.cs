using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OLD_CheeseWheelMovement : MonoBehaviour
{
    public Rigidbody rb;
    protected float movementForward;
    protected float movementTurn;
    protected float honeyModifier = 1;

    public float ForwardSpeed = 1;
    public float TurnSpeed = 1;
    public float SlownessEffect = 1;

    public LayerMask GroundLayer;
    public Vector3 Down = Vector3.down;
    public Vector3 Forward = Vector3.forward;
    public GameObject WheelCenter;


    protected GameObject ResetPoint;
    public void SetResetPoint(GameObject reset) { ResetPoint = reset; }
    private Vector3 PlayerSpecificResetPositionOffset = Vector3.zero;
    public void SetPlayerSpecificResetPositionOffset(Vector3 offset) { PlayerSpecificResetPositionOffset = offset; }
    public Vector3 ResetPositionOffset { get { return PlayerSpecificResetPositionOffset + Vector3.up * 2; } }
    public float AutoResetAngle = 45;
    public float AutoAdjustAngle = 80;

    private bool controlsInverted = false;

    protected void Start()
    {
        if (ResetPoint == null)
        {
            ResetPoint = GameManager.Instance.StartPoint.gameObject;
        }
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    protected void FixedUpdate()
    {
        // Apply control inversion if active
        // float actualMovementTurn = controlsInverted ? -movementTurn : movementTurn;
        // float actualMovementForward = controlsInverted ? -movementForward : movementForward;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, GroundLayer.value))
        {
            Down = hit.transform.up * -1;
        }
        // Debug.DrawRay(transform.position, Down, Color.yellow, Time.fixedDeltaTime);

        Forward = Vector3.Cross(transform.up, Down).normalized;
        // Debug.DrawRay(transform.position, Forward, Color.blue, Time.fixedDeltaTime);

        //OLD Rotation
        //Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, movementTurn * TurnSpeed * Time.fixedDeltaTime * honeyModifier, 0));


        //transform.Rotate(Down * -1 * movementTurn * TurnSpeed * Time.fixedDeltaTime * honeyModifier);

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


        Vector3 v3RotationStandup = Vector3.zero;
        if (angle + AutoAdjustAngle >= 180 || angle - AutoAdjustAngle <= 0)
        {
            v3RotationStandup = Forward * (angle - 90) * TurnSpeed * Time.fixedDeltaTime * honeyModifier;
        }

        Vector3 v3RotationInputs = Down * -1 * movementTurn * TurnSpeed * Time.fixedDeltaTime * honeyModifier;

        Vector3 v3Rotation = v3RotationStandup + v3RotationInputs * 2;
        v3Rotation.Normalize();

        Quaternion deltaRotation = Quaternion.Euler(v3Rotation);

        rb.MoveRotation(deltaRotation * transform.rotation);
    }

    //private void OnMove(InputValue movementValue)
    //{
    //    Vector2 movementVector = movementValue.Get<Vector2>();

    //    movementForward = movementVector.y;
    //    movementTurn = movementVector.x;

    //}


    public void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        Debug.Log(movementVector);

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

    public void OnResetPosition()
    {
        ResetPosition();
    }

    public void ResetPosition()
    {
        rb.Sleep();  // Stop all physics activity
        transform.position = ResetPoint.transform.position + ResetPositionOffset;  // Reset position
        transform.LookAt(transform.position + ResetPoint.transform.forward);  // Reset orientation
        transform.Rotate(transform.forward, 90);  // Correct rotation to original setup
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
        if (honeyModifier != 1.0f)
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

    public void Jump(float power)
    {
        rb.AddForce(Down * -1 * power * rb.mass, ForceMode.Impulse);
    }
}
