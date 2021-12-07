using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataToMovementScript : MonoBehaviour
{
    private RotatorScript rotator;

    void Start()
    {
        rotator = GameObject.Find("Capsule").GetComponent<RotatorScript>();
    }

    public void SendSerialMessage(string message)
    {
        rotator.GetMessageFromHardware(message);
    }
}
