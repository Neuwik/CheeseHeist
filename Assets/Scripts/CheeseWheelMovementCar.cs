using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheeseWheelMovementCar : CheeseWheelMovement
{
    public WheelCollider WheelCollider;
    public Transform WheeleMesh;

    private void Start()
    {
        if (rb == null)
        {  rb = GetComponent<Rigidbody>(); }
    }

    void FixedUpdate()
    {
        WheelCollider.steerAngle = movementTurn * TurnSpeed;
        WheelCollider.motorTorque = movementForward * ForwardSpeed;


        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;
        WheelCollider.GetWorldPose(out pos, out rot);
        //WheeleMesh.transform.position = pos;
        WheeleMesh.transform.rotation = rot;
    }
}
