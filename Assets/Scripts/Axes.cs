using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Axes : MonoBehaviour, GraphRenderer
{
    // Public properties
    public GameObject linePrefab;

    // Private properties
    private LineRenderer xAxisLine;
    private LineRenderer yAxisLine;
    private LineRenderer zAxisLine;

    private TextMeshPro xAxisTextMesh;
    private TextMeshPro yAxisTextMesh;
    private TextMeshPro zAxisTextMesh;

    private (Vector3[], Vector3[], Vector3[]) positions;
    private bool visibility = false;
    private bool updated = false;

    void Start()
    {
        Debug.Log("Axes.cs :: Instantiate Lines");

        GameObject xAxis = Instantiate(
            linePrefab,
            new Vector3(0, 0, 0),
            Quaternion.identity);

        // Set Parent and get line renderer
        xAxis.transform.SetParent(gameObject.transform, false);
        xAxisLine = xAxis.GetComponent<LineRenderer>();
        xAxisLine.enabled = false;
        
        xAxisTextMesh = xAxis.transform.GetChild(0).GetComponent<TextMeshPro>();
        xAxisTextMesh.enabled = false;

        GameObject yAxis = Instantiate(
            linePrefab,
            new Vector3(0, 0, 0),
            Quaternion.identity);

        // Set Parent and get line renderer
        yAxis.transform.SetParent(gameObject.transform, false);
        yAxisLine = yAxis.GetComponent<LineRenderer>();
        yAxisLine.enabled = false;

        yAxisTextMesh = yAxis.transform.GetChild(0).GetComponent<TextMeshPro>();
        yAxisTextMesh.enabled = false;

        GameObject zAxis = Instantiate(
            linePrefab,
            new Vector3(0, 0, 0),
            Quaternion.identity);

        // Set Parent and get line renderer
        zAxis.transform.SetParent(gameObject.transform, false);
        zAxisLine = zAxis.GetComponent<LineRenderer>();
        zAxisLine.enabled = false;

        Debug.Log("Axes.cs :: attempting to set text");

        zAxisTextMesh = zAxis.transform.GetChild(0).GetComponent<TextMeshPro>();
        zAxisTextMesh.enabled = false;
    }

    void Update()
    {
        if (updated)
        {
            (var xPos, var yPos, var zPos) = positions;
            xAxisLine.SetPositions(xPos);
            xAxisLine.enabled = visibility;
            xAxisTextMesh.enabled = visibility;

            yAxisLine.SetPositions(yPos);
            yAxisLine.enabled = visibility;
            yAxisTextMesh.enabled = visibility;

            zAxisLine.SetPositions(zPos);
            zAxisLine.enabled = visibility;
            zAxisTextMesh.enabled = visibility;
        }
    }

    public void update(Graph graphData)
    {
        Debug.Log("Axes.cs :: Recalculate Lines " + graphData.filename);
        (Vector3 min, Vector3 max) = graphData.getMinMax();
        Debug.Log("Graph min: " + min);
        Debug.Log("Graph max: " + max);
        visibility = graphData.getVisibility();

        Vector3 length = max-min;

        float offset = 0.07f;
        float heightOffGround = min.y - offset;
        
        xAxisTextMesh.transform.localPosition = min + new Vector3(0, -offset, length.z / 2);
        yAxisTextMesh.transform.localPosition = min + new Vector3(- offset/1.414f, length.y / 2, - offset/1.414f);
        zAxisTextMesh.transform.localPosition = min + new Vector3(length.x / 2, -offset, 0);

        xAxisTextMesh.transform.rotation = Quaternion.Euler(0, 90, 0);
        yAxisTextMesh.transform.rotation = Quaternion.Euler(0, 45, 90);
        zAxisTextMesh.transform.rotation = Quaternion.Euler(0, 0, 0);

        // TODO: work out why this needs to be the wrong way around
        xAxisTextMesh.text = graphData.zAxisLabel;
        yAxisTextMesh.text = graphData.yAxisLabel;
        zAxisTextMesh.text = graphData.xAxisLabel;

        positions = (
            new [] {min, min + new Vector3(length.x, 0, 0)},
            new [] {min, min + new Vector3(0, length.y, 0)},
            new [] {min, min + new Vector3(0, 0, length.z)});

        updated = true;
    }
}
