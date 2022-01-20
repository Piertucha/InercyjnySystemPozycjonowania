using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartesianPlane : MonoBehaviour
{
    private Coordinates startPointX;
    private Coordinates endPointX;

    private Coordinates startPointY;
    private Coordinates endPointY;

    private float zPosition;

    public Color lineColor = Color.white;
    public float lineThickness = 0.05f;
    public int lineSpacing = 10;
    public int lineCountUp = 10;
    private GameObject lineParent;

    private void Start()
    {
        lineParent = GameObject.Find("LineParent");
        // get gameobject's z coordinate
        zPosition = gameObject.transform.position.z;
        
        startPointX = new Coordinates(160, 0);
        endPointX = new Coordinates(-160, 0);
        startPointY = new Coordinates(0, 100);
        endPointY = new Coordinates(0, -100);

        Coordinates.DrawCartesianPlane(startPointX,endPointX,lineColor, lineThickness, zPosition, lineParent);
        Coordinates.DrawCartesianPlane(startPointY,endPointY,lineColor, lineThickness, zPosition, lineParent);

        for (int i = 1; i <= lineCountUp; i++)
        {
            // vertical lines:
            // up
            startPointX = new Coordinates(160, i * lineSpacing);
            endPointX = new Coordinates(-160, i * lineSpacing);
            Coordinates.DrawCartesianPlane(startPointX,endPointX,lineColor, lineThickness, zPosition, lineParent);
            // down
            startPointX = new Coordinates(160, i * -1 * lineSpacing);
            endPointX = new Coordinates(-160, i * -1 * lineSpacing);
            Coordinates.DrawCartesianPlane(startPointX,endPointX,lineColor, lineThickness, zPosition, lineParent);
            
            
            // horizontal lines:
            // right
            startPointY = new Coordinates(i * lineSpacing, 100);
            endPointY = new Coordinates(i * lineSpacing, -100);
            Coordinates.DrawCartesianPlane(startPointY,endPointY,lineColor, lineThickness, zPosition, lineParent);
            // left
            startPointY = new Coordinates(i * -1 * lineSpacing, 100);
            endPointY = new Coordinates(i * -1 * lineSpacing, -100);
            Coordinates.DrawCartesianPlane(startPointY,endPointY,lineColor, lineThickness, zPosition, lineParent);
            
        }
    }
}
