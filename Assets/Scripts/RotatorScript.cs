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
    
    // Kalman Filter values
    [Header("Kalman Filter")]
    public float q = 0.000001f;
    public float r = 0.01f;
    [Space(20)]

    public float forceScaling = 1f; // multiplies force
    public float rotationDampening = 1f; // divides angles
    public bool rotateByAbsoluteValues = false;
    public bool useKalmanFilter = true;

    private float rotationProgress = -1;

    private Vector3 forceVector;

    private Quaternion startRotation;
    private Quaternion endRotation;

    public KalmanFilterFloat KalmanFilter;

    // Start is called before the first frame update
    void Start()
    {
        transform = gameObject.transform;
        rb = gameObject.GetComponent<Rigidbody>();
        KalmanFilter = new KalmanFilterFloat(q, r);
    }

    private void Update()
    {
        Rotate();
        
        if(Input.GetKeyDown(KeyCode.R))
            ResetCapsuleTransform();
    }

    private void FixedUpdate()
    {
       //StartRotating(xAngle, yAngle, zAngle);
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
            //dampen rotations
            xAngle *= rotationDampening;
            yAngle *= rotationDampening;
            zAngle *= rotationDampening;
            
            //apply Kalman filter if toggled
            if (useKalmanFilter)
            {
                xAngle = KalmanFilter.Update(xAngle);
                yAngle = KalmanFilter.Update(yAngle);
                zAngle = KalmanFilter.Update(zAngle);
            }
            

            transform.Rotate(xAngle,yAngle,zAngle);
            //Debug.Log(xAngle+" "+yAngle+" "+zAngle);

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
        // scale forces
        xForce *= forceScaling;
        yForce *= forceScaling;
        zForce *= forceScaling;
        // use Kalman filter if toggled
        if (useKalmanFilter)
        {
            xForce = KalmanFilter.Update(xForce);
            yForce = KalmanFilter.Update(yForce);
            zForce = KalmanFilter.Update(zForce);
        }
        
        forceVector = new Vector3(xForce, yForce, zForce);
        
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


        // swap y and z axis
        xAngle = float.Parse(sStrings[0]);
        zAngle = float.Parse(sStrings[1]);
        yAngle = float.Parse(sStrings[2]);
        
        xForce = float.Parse(sStrings[3]);
        zForce = float.Parse(sStrings[4]);
        yForce = float.Parse(sStrings[5]);

        xForce -= 5.80f;
        zForce += 4.8f;

        // gravity
        yForce -= 9.81f;
        
        //invert forces
        xForce *= -1f;
        zForce *= -1f;
        yForce *= -1f;
        

    }

    void ResetCapsuleTransform()
    {
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
}
