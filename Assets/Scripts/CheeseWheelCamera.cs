using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheeseWheelCamera : MonoBehaviour
{
    private Vector3 CameraOffset;
    public Transform CheeseWheel;

    // Start is called before the first frame update
    void Start()
    {
        CameraOffset = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position =  CheeseWheel.position + new Vector3(0, 1.5f, -2);
    }
}
