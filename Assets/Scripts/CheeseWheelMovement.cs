using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CheeseWheelMovement : MonoBehaviour
{
    private Rigidbody rb;
    private float movemntForward;
    private float movementTurn;

    public float ForwardSpeed = 1;
    public float TurnSpeed = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rb.AddForce(transform.forward * movemntForward * ForwardSpeed);
        rb.MoveRotation(rb.rotation * new Quaternion(0, movementTurn, 0, TurnSpeed));
    }

    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movemntForward = movementVector.y;
        movementTurn = movementVector.x;
    }
}
