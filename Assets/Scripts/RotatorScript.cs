using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class RotatorScript : MonoBehaviour
{
    private Transform transform;
    private Rigidbody rb;
    public TextMeshProUGUI calibratingText;

    private bool beginCalibration = false;
    private float[] yArray = new float[2000];
    private float[] xArray = new float[2000];
    private float[] zArray = new float[2000];
    
    public float forceMargin = 0.2f;

    float xAngle = 0f;
    float yAngle = 0f;
    float zAngle = 0f;
    private float initialYAngle = 0f;

    float xForce = 0f;
    float yForce = 0f;
    float zForce = 0f;

    public Vector3 gravityCompensator = new Vector3(0, 9.81f, 0);
    [SerializeField]
    private Vector3 errorCompensator = new Vector3(0, 0, 0);

    public UIText uiText;
    
    // Kalman Filter values
    [Header("Kalman Filter")]
    public float q = 0.000001f;
    public float r = 0.01f;
    [Space(20)]

    public float forceScaling = 1f; // multiplies force
    public float rotationDampening = 1f; // divides angles
    public bool rotateByAbsoluteValues = false;
    public bool useKalmanFilter = true;
    public bool useNewFilter = false;

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

        StartCoroutine(WaitXTime(5));
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
            //Vector3 eulers = new Vector3(xAngle, transform.rotation.y, zAngle);

            //yAngle = yAngle / 2;
            //transform.rotation = Quaternion.Euler(xAngle,yAngle,zAngle);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(xAngle, (yAngle - initialYAngle)/2, zAngle),
                Time.deltaTime * 5f);

            //transform.rotation = Quaternion.Euler(eulers);
            //transform.Rotate(0,yAngle,0);
        }
    }
    void StartRotating(float x, float y, float z)
    {
        startRotation = transform.rotation;
        endRotation = Quaternion.Euler(x,y,z);
    }

    void Accelerate()
    {
        // scale forces
        /*
        xForce *= forceScaling;
        yForce *= forceScaling;
        zForce *= forceScaling;
        */
        // use Kalman filter if toggled
        if (useKalmanFilter)
        {
            xForce = KalmanFilter.Update(xForce);
            yForce = KalmanFilter.Update(yForce);
            zForce = KalmanFilter.Update(zForce);
        }



        forceVector = new Vector3(xForce - errorCompensator.x, yForce - errorCompensator.y, zForce - errorCompensator.z);
        //forceVector.y -= gravityCompensator.y;
        forceVector -= Quaternion.Euler(xAngle, yAngle - initialYAngle, zAngle) * gravityCompensator;

        forceVector = Quaternion.Euler(xAngle, yAngle - initialYAngle, zAngle) * forceVector;
        
        
        //forceVector += gravityCompensator.y * transform.up;
        /*
        forceVector += new Vector3(gravityCompensator.y * Vector3.Dot(transform.right,
                Vector3.down), gravityCompensator.y * Vector3.Dot(transform.up, Vector3.up), gravityCompensator.y *
            Vector3.Dot(transform.forward,
                Vector3.down));*/


        uiText.forceVector = forceVector;
/*
        if (forceVector.x <= forceMargin && forceVector.x >= -forceMargin)
        {
            forceVector.x = 0;
        }
        if (forceVector.y <= forceMargin && forceVector.y >= -forceMargin)
        {
            forceVector.y = 0;
        }
        if (forceVector.z <= forceMargin && forceVector.z >= -forceMargin)
        {
            forceVector.z = 0;
        }*/
        

        //rb.velocity += forceVector * forceScaling;
        
        //rb.AddForce(forceVector * forceScaling);
        
        
        if(useNewFilter)
            rb.AddForce(NewFilter(forceVector * forceScaling));
        else
        {
            rb.AddForce(forceVector * forceScaling);
        }
        //rb.AddForce(forceVector * forceScaling, ForceMode.Impulse);
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

        yAngle *= -1;
    }

    void ResetCapsuleTransform()
    {
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        rb.velocity = Vector3.zero;
    }

    void CalibrateGravity()
    {
        float[] yAngleArray = new float[2000];
        Vector3 tempGravityRotated = Quaternion.Euler(xAngle, yAngle, zAngle) * gravityCompensator;
        for (int i = 0; i < 2000; i++)
        {
            xArray[i] = xForce - tempGravityRotated.x;
            yArray[i] = yForce - tempGravityRotated.y;
            zArray[i] = zForce - tempGravityRotated.z;

            yAngleArray[i] = yAngle;
        }

        errorCompensator.x = xArray.Average();
        errorCompensator.y = yArray.Average();
        errorCompensator.z = zArray.Average();

        initialYAngle = yAngleArray.Average();
        
        ResetCapsuleTransform();
        calibratingText.gameObject.SetActive(false);
    }

    IEnumerator WaitXTime(float amount)
    {
        yield return new WaitForSeconds(amount);
        beginCalibration = true;
        
        //calibrate after X seconds
        CalibrateGravity();
    }

    Vector3 NewFilter(Vector3 inputVector)
    {
        Vector3 result;
        Vector3 minus = new Vector3(-1f, -1f, -1f);
        
        if (inputVector.x >= 0)
            minus.x = 1f;
        if (inputVector.y >= 0)
            minus.y = 1f;
        if (inputVector.z >= 0)
            minus.z = 1f;
        result = new Vector3(inputVector.x * inputVector.x * minus.x,
            inputVector.y * inputVector.y * minus.y,
            inputVector.z * inputVector.z * minus.z);

        return result;

    }
}
