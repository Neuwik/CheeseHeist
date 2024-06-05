using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CheeseWheelMovement : MonoBehaviour
{
    public Rigidbody rb;
    protected float movementForward;
    protected float movementTurn;
    protected float honeyModifier = 1;
    private bool controlsInverted = false;

    protected GameObject ResetPoint;
    public void SetResetPoint(GameObject reset) { ResetPoint = reset; }

    private Vector3 PlayerSpecificResetPositionOffset = Vector3.zero;
    public void SetPlayerSpecificResetPositionOffset(Vector3 offset) { PlayerSpecificResetPositionOffset = offset; }
    public Vector3 ResetPositionOffset { get { return PlayerSpecificResetPositionOffset + Vector3.up * ResetPositionVerticalOffset; } }
    public float ResetPositionVerticalOffset = 2;
    public float AutoResetAngle = 45;

    #region Car Script Copy
    [Header("From Car")]
    public float motorTorque = 2000;
    public float brakeTorque = 2000;
    public float maxSpeed = 20;
    public float steeringRange = 30;
    public float steeringRangeAtMaxSpeed = 10;
    public float centreOfGravityOffset = -1f;

    private List<CarWheel> _wheels;
    public Transform RotatingObject;
    public bool ShowWheels = false;
    private WheelCollider _anyBackWheel;
    #endregion

    protected void Start()
    {
        if (ResetPoint == null)
        {
            ResetPoint = GameManager.Instance.StartPoint;
        }
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        #region Car Script Copy
        // Adjust center of mass vertically, to help prevent the car from rolling
        rb.centerOfMass += Vector3.up * centreOfGravityOffset;

        // Find all child GameObjects that have the WheelControl script attached
        _wheels = GetComponentsInChildren<CarWheel>().ToList();
        _wheels.ForEach(w => w.WheelModel.gameObject.SetActive(ShowWheels));
        _anyBackWheel = _wheels.First(w => !w.steerable).WheelCollider;
        #endregion
    }

    protected void FixedUpdate()
    {
        movementTurn *= honeyModifier;
        movementForward *= honeyModifier;

        #region Car Script Copy

        // Calculate current speed in relation to the forward direction of the car
        // (this returns a negative number when traveling backwards)
        float forwardSpeed = Vector3.Dot(transform.forward, rb.velocity);


        // Calculate how close the car is to top speed
        // as a number from zero to one
        float speedFactor = Mathf.InverseLerp(0, maxSpeed, forwardSpeed);

        // Use that to calculate how much torque is available 
        // (zero torque at top speed)
        float currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor);

        // …and to calculate how much to steer 
        // (the car steers more gently at top speed)
        float currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

        // Check whether the user input is in the same direction 
        // as the car's velocity
        bool isAccelerating = Mathf.Sign(movementForward) == Mathf.Sign(forwardSpeed);

        foreach (var wheel in _wheels)
        {
            // Apply steering to Wheel colliders that have "Steerable" enabled
            if (wheel.steerable)
            {
                wheel.WheelCollider.steerAngle = movementTurn * currentSteerRange;
            }

            if (isAccelerating)
            {
                // Apply torque to Wheel colliders that have "Motorized" enabled
                if (wheel.motorized)
                {
                    wheel.WheelCollider.motorTorque = movementForward * currentMotorTorque;
                }
                wheel.WheelCollider.brakeTorque = 0;
            }
            else
            {
                // If the user is trying to go in the opposite direction
                // apply brakes to all wheels
                wheel.WheelCollider.brakeTorque = Mathf.Abs(movementForward) * brakeTorque;
                wheel.WheelCollider.motorTorque = 0;
            }
        }

        //Rotate Cheese Wheel Mesh
        Vector3 pos = transform.position;
        Quaternion wheelRot = transform.rotation;


        _anyBackWheel.GetWorldPose(out pos, out wheelRot);
        RotatingObject.position = new Vector3(RotatingObject.position.x, pos.y, RotatingObject.position.z);
        RotatingObject.rotation = wheelRot;

        #endregion

        float angle = Vector3.Angle(transform.up * -1, transform.right);
        //Debug.Log(angle);
        if (angle + AutoResetAngle >= 180 || angle - AutoResetAngle <= 0)
        {
            ResetPosition();
        }
    }

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
        //Debug.Log($"vorm wait");
        yield return new WaitForSeconds(timer);
        //Debug.Log($"nach wait");
        honeyModifier = 1.0f;
        rb.drag -= 1.5f;
    }

    public void Jump(float power)
    {
        rb.AddForce(transform.up * power * rb.mass, ForceMode.Impulse);
    }
}
