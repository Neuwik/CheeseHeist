using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CheeseWheelMovement : MonoBehaviour
{
    private Rigidbody rb;
    private float movementForward;
    private float movementTurn;

    public float ForwardSpeed = 1;
    public float TurnSpeed = 1;

    public Vector3 Forward;
    public GameObject WheelCenter;
    public GameObject ResetPoint;
    public Vector3 ResetPositionOffset = new Vector3(0,2,0);

    void Start()
    {
        ResetPoint = transform.parent.gameObject;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, movementTurn * TurnSpeed * Time.fixedDeltaTime, 0));
        rb.MoveRotation(deltaRotation * transform.rotation);

        Forward = Vector3.Cross(transform.up, Vector3.down).normalized;
        //Debug.DrawRay(transform.position, forward, Color.yellow, Time.fixedDeltaTime);

        rb.AddForce(Forward * movementForward * ForwardSpeed * Time.fixedDeltaTime * rb.mass);

        //Camera
        WheelCenter.transform.position = transform.position;
        WheelCenter.transform.LookAt(transform.position + Forward);
    }

    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementForward = movementVector.y;
        movementTurn = movementVector.x;
    }

    private void OnResetPosition()
    {
        ResetPosition();
    }

    private void ResetPosition()
    {
        rb.Sleep();
        transform.position = ResetPoint.transform.position + ResetPositionOffset;
        transform.LookAt(transform.position + ResetPoint.transform.forward);
        transform.Rotate(transform.forward, 90);
    }
}
