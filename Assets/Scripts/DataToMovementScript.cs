using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataToMovementScript : MonoBehaviour
{
    public bool enableCOMCommunication = false;
    
    private RotatorScript rotator;

    void Start()
    {
        rotator = GameObject.Find("Capsule").GetComponent<RotatorScript>();
    }

    public void SendSerialMessage(string message)
    {
        if(enableCOMCommunication)
            rotator.GetMessageFromHardware(message);
    }
}
