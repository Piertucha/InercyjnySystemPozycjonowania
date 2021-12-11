using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorScript : MonoBehaviour
{
    private Transform transform;
    private Rigidbody rb;

    float xAngle = 0f;
    float yAngle = 0f;
    float zAngle = 0f;
    
    float xForce = 0f;
    float yForce = 0f;
    float zForce = 0f;

    private float rotationProgress = -1;

    private Vector3 forceVector;

    private Quaternion startRotation;
    private Quaternion endRotation;

    // Start is called before the first frame update
    void Start()
    {
        transform = gameObject.transform;
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Rotate();
    }

    private void FixedUpdate()
    {
        StartRotating(xAngle, yAngle, zAngle);
        Accelerate();
    }

    public void Rotate()
    {
        /*
        if (rotationProgress < 1 && rotationProgress >= 0)
        {
            rotationProgress += Time.deltaTime * 5;
        } */

        transform.rotation = Quaternion.Lerp(startRotation, endRotation, .1f);
    }

    void Accelerate()
    {
        forceVector = new Vector3(xForce, yForce, zForce);
        rb.AddForce(forceVector);
        //rb.AddForceAtPosition(forceVector,transform.position);
        forceVector = Vector3.zero;

    }

    void StartRotating(float x, float y, float z)
    {
        startRotation = transform.rotation;
        endRotation = Quaternion.Euler(x,y,z);
    }

    public void GetMessageFromHardware(string message)
    {
        StringToFloat(message);
    }

    void StringToFloat(string input)
    {
        input = input.Replace('.', ',');
        
        var sStrings = input.Split(" "[0]);

        xAngle = float.Parse(sStrings[0]);
        yAngle = float.Parse(sStrings[1]);
        zAngle = float.Parse(sStrings[2]);
        
        xForce = float.Parse(sStrings[3]);
        yForce = float.Parse(sStrings[4]);
        zForce = float.Parse(sStrings[5]);

    }
}
