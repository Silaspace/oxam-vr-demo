using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AxesNew : MonoBehaviour
{
    public string xAxisLabel = "X axis";
    public string yAxisLabel = "Y axis";
    public string zAxisLabel = "Z axis";

    public bool bidirectional = false;

    private GameObject xAxisObject;
    private GameObject yAxisObject;
    private GameObject zAxisObject;

    private TextMeshPro xAxisTextMesh;
    private TextMeshPro yAxisTextMesh;
    private TextMeshPro zAxisTextMesh;

    private LineRenderer xAxisLine;
    private LineRenderer yAxisLine;
    private LineRenderer zAxisLine;

    private static Vector3[] unidirectionalPositions = new Vector3[]
    {
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(0.93f, 0.07f, 0),
        new Vector3(1, 0, 0),
        new Vector3(0.93f, -0.07f, 0)
    };

    private static Vector3[] bidirectionalPositions = new Vector3[]
    {
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(0.93f, 0.07f, 0),
        new Vector3(1, 0, 0),
        new Vector3(0.93f, -0.07f, 0),
        new Vector3(1, 0, 0),
        new Vector3(-1, 0, 0),
        new Vector3(-0.93f, -0.07f, 0),
        new Vector3(-1, 0, 0),
        new Vector3(-0.93f, 0.07f, 0)
    };

    void Start()
    {
        Debug.Log("AxesNew");

        xAxisObject = GameObject.Find("Axis (X)/Label");
        xAxisTextMesh = xAxisObject.GetComponent<TextMeshPro>();
        xAxisLine = GameObject.Find("Axis (X)/Line").GetComponent<LineRenderer>();

        yAxisObject = GameObject.Find("Axis (Y)/Label");
        yAxisTextMesh = yAxisObject.GetComponent<TextMeshPro>();
        yAxisLine = GameObject.Find("Axis (Y)/Line").GetComponent<LineRenderer>();

        zAxisObject = GameObject.Find("Axis (Z)/Label");
        zAxisTextMesh = zAxisObject.GetComponent<TextMeshPro>();
        zAxisLine = GameObject.Find("Axis (Z)/Line").GetComponent<LineRenderer>();
    }

    void SetLabels(string x, string y, string z)
    {
        xAxisTextMesh.text = x;
        yAxisTextMesh.text = y;
        zAxisTextMesh.text = z;
    }

    void setPositions(Vector3[] positions) {
        xAxisLine.SetPositions(positions);
        yAxisLine.SetPositions(positions);
        zAxisLine.SetPositions(positions);
    }

    void Update()
    {
        SetLabels(xAxisLabel, yAxisLabel, zAxisLabel);

        if (bidirectional)
        {
            setPositions(bidirectionalPositions);
        } else
        {
            setPositions(unidirectionalPositions);
        }
    }
}
