using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class axes : MonoBehaviour
{
    public Material lineMaterial;
    public float widthOfLine = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        // Use position of parent game object, but offset to place the axis at the corner
        // Not sure how well this works with scaling currently
        draw3Axes(
            transform.position + new Vector3(-1, 1, -1),
                1,
                "Time (months)",
                "Price",
                "Time (years)",
                widthOfLine
            );
    }

    void drawAxis(Vector3 position0, int length, string label, int dir, Color col, float width)
    {
        //game object for the axis
        GameObject line = new GameObject("Axis - " + label);

        //line renderer to display the line
        LineRenderer lr = line.AddComponent<LineRenderer>();
        lr.material = lineMaterial;
        lr.material.color = col;
        lr.widthMultiplier = width;
        lr.SetPosition(0, position0);

        //end position of the line
        Vector3 position1 = position0;
        if (dir == 0) position1 = position0 + new Vector3(length,0,0);
        if (dir == 1) position1 = position0 + new Vector3(0,length,0);
        if (dir == 2) position1 = position0 + new Vector3(0,0,length);
        lr.SetPosition(1, position1);
        
        //position of the label
        Vector3 position2 = position0;
        if (dir == 0) position2 = position0 + new Vector3(length/2,0,0);
        if (dir == 1) position2 = position0 + new Vector3(0,length/2,0);
        if (dir == 2) position2 = position0 + new Vector3(0,0,length/2);
        
        line.transform.position = position2;
        //text mesh for the label

        TextMesh txt = line.AddComponent<TextMesh>();
        txt.text = label;
        txt.characterSize = 0.2f;
    }

    void draw3Axes (Vector3 position0, int length, string labelX, string labelY, string labelZ, float width)
    {
        //call the 3 axes
        drawAxis(position0, length, labelX, 0, Color.blue, width);
        drawAxis(position0, length, labelY, 1, Color.red, width);
        drawAxis(position0, length, labelZ, 2, Color.yellow, width);
    }

}