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

    public float ForwardSpeed = 1;
    public float TurnSpeed = 1;

    public LayerMask GroundLayer;
    public Vector3 Down = Vector3.down;
    public Vector3 Forward = Vector3.forward;
    public GameObject WheelCenter;
    public GameObject ResetPoint;
    public Vector3 ResetPositionOffset = new Vector3(0,2,0);
    public float AutoResetAngle = 45;

    void Start()
    {
        ResetPoint = transform.parent.gameObject;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, movementTurn * TurnSpeed * Time.fixedDeltaTime, 0));
        rb.MoveRotation(deltaRotation * transform.rotation);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, GroundLayer.value))
        {
            Down = hit.transform.up * -1;
        }
        Debug.DrawRay(transform.position, Down, Color.yellow, Time.fixedDeltaTime);

        Forward = Vector3.Cross(transform.up, Down).normalized;
        Debug.DrawRay(transform.position, Forward, Color.blue, Time.fixedDeltaTime);

        rb.AddForce(Forward * movementForward * ForwardSpeed * Time.fixedDeltaTime * rb.mass);

        //Camera
        WheelCenter.transform.position = transform.position;
        WheelCenter.transform.LookAt(transform.position + Forward);

        float angle = Vector3.Angle(Down, transform.up);
        Debug.Log(angle);
        if (angle + AutoResetAngle >= 180 || angle - AutoResetAngle <= 0)
        {
            ResetPosition();
        }
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
