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

    public float forceScaling = 1f; // multiplies force
    public float rotationDampening = 1f; // divides angles
    public bool rotateByAbsoluteValues = false;

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
        
        if(Input.GetKeyDown(KeyCode.R))
            ResetCapuslePosition();
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

        //transform.rotation = Quaternion.Lerp(startRotation, endRotation, .1f);
        // TEMP TEST
        if (rotateByAbsoluteValues)
        {
            transform.Rotate(xAngle / rotationDampening,yAngle / rotationDampening,zAngle / rotationDampening);

        }
        else
        {
            transform.rotation = Quaternion.Euler(xAngle,yAngle,zAngle);
        }
        //transform.rotation = Quaternion.Euler(xAngle,yAngle,zAngle);
    }
    void StartRotating(float x, float y, float z)
    {
        startRotation = transform.rotation;
        endRotation = Quaternion.Euler(x,y,z);
    }

    void Accelerate()
    {
        forceVector = new Vector3(xForce * forceScaling, yForce * forceScaling, zForce * forceScaling);
        rb.AddForce(forceVector);
        //rb.AddForceAtPosition(forceVector,transform.position);
        forceVector = Vector3.zero;
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

    void ResetCapuslePosition()
    {
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
}
