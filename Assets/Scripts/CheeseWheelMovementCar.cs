using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public class CheeseWheelMovementCar : CheeseWheelMovement
{
    #region Car Script Copy
    [Header("From Car")]
    public float motorTorque = 2000;
    public float brakeTorque = 2000;
    public float maxSpeed = 20;
    public float steeringRange = 30;
    public float steeringRangeAtMaxSpeed = 10;
    public float centreOfGravityOffset = -1f;

    List<CarWheel> wheels;
    public Transform CheeseWheelMesh;
    WheelCollider AnyBackWheel;
    public bool ShowWheels = false;
    #endregion

    protected new void Start()
    {
        #region Car Script Copy
        // Adjust center of mass vertically, to help prevent the car from rolling
        rb.centerOfMass += Vector3.up * centreOfGravityOffset;

        // Find all child GameObjects that have the WheelControl script attached
        wheels = GetComponentsInChildren<CarWheel>().ToList();
        wheels.ForEach(w => w.WheelModel.gameObject.SetActive(ShowWheels));
        AnyBackWheel = wheels.First(w => !w.steerable).WheelCollider;
        #endregion
    }

    protected new void FixedUpdate()
    {
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

        foreach (var wheel in wheels)
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


        AnyBackWheel.GetWorldPose(out pos, out wheelRot);
        CheeseWheelMesh.position = new Vector3(CheeseWheelMesh.position.x, pos.y, CheeseWheelMesh.position.z);
        CheeseWheelMesh.rotation = wheelRot;

        #endregion

        float angle = Vector3.Angle(transform.up * -1, transform.right);
        //Debug.Log(angle);
        if (angle + AutoResetAngle >= 180 || angle - AutoResetAngle <= 0)
        {
            ResetPosition();
        }
    }

    public new void ResetPosition()
    {
        rb.Sleep();  // Stop all physics activity
        transform.position = ResetPoint.transform.position + ResetPositionOffset;  // Reset position
        transform.LookAt(transform.position + ResetPoint.transform.forward);  // Reset orientation
    }
}
