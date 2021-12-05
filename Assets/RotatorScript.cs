using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorScript : MonoBehaviour
{
    private Transform transform;

    string xString ="0";
    string yString="0";
    string zString="0";

    float xAngle = 0f;
    float yAngle=0f;
    float zAngle=0f;

    // Start is called before the first frame update
    void Start()
    {
        transform = this.transform;
    }


    void FixedUpdate()
    {
        Rotate(xAngle, yAngle, zAngle);
    }

    public void Rotate(float x, float y, float z)
    {
        transform.Rotate(x, y, z);
    }

    public void GetMessageFromHardware(string message)
    {/*
                int i = 0;
        int j = 0;
        foreach(char c in message)
        {
            if (c != " ")
            {
                switch (j)
                {
                    case 0:
                        xString[i] = c;
                        break;
                    case 1:
                        yString[i] = c;
                        break;
                    case 2:
                        zString[i] = c;
                }
            }
            else
            {
                j++;
            }
        }

         xAngle = xString.ToFloat();
         yAngle = yString.ToFloat();
         zAngle = zString.ToFloat();
        */

        StringToFloat(message);
    }

    void StringToFloat(string string)
    {
        var sStrings = string.Split(" "[0]);

        xAngle = float.Parse(sStrings[0]);
        yAngle = float.Parse(sStrings[1]);
        zAngle = float.Parse(sStrings[2]);

    }
}
