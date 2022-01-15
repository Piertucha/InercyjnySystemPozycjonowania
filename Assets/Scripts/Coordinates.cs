using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coordinates : MonoBehaviour
{
    float x;
    float y;

    public Coordinates(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    static public void DrawCartesianPlane(Coordinates originPos, Coordinates endPos, Color color, float width, float zPosition, GameObject parent)
    {
        GameObject line = new GameObject();
        line.transform.parent = parent.transform;
        LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = color;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = width;
        lineRenderer.SetPosition(0, new Vector3(originPos.x,originPos.y,zPosition));
        lineRenderer.SetPosition(1, new Vector3(endPos.x,endPos.y,zPosition));
    }
}
