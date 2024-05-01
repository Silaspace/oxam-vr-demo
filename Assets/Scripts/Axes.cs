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
    private (Vector3[], Vector3[], Vector3[]) positions;
    private bool visibility = false;
    private bool updated = false;

    void Start()
    {
        Debug.Log("AxesNew.cs :: Instantiate Lines");

        GameObject xAxis = Instantiate(
            linePrefab,
            new Vector3(0, 0, 0),
            Quaternion.identity);

        // Set Parent and get line renderer
        xAxis.transform.SetParent(gameObject.transform, false);
        xAxisLine = xAxis.GetComponent<LineRenderer>();
        xAxisLine.enabled = false;

        GameObject yAxis = Instantiate(
            linePrefab,
            new Vector3(0, 0, 0),
            Quaternion.identity);

        // Set Parent and get line renderer
        yAxis.transform.SetParent(gameObject.transform, false);
        yAxisLine = yAxis.GetComponent<LineRenderer>();
        yAxisLine.enabled = false;

        GameObject zAxis = Instantiate(
            linePrefab,
            new Vector3(0, 0, 0),
            Quaternion.identity);

        // Set Parent and get line renderer
        zAxis.transform.SetParent(gameObject.transform, false);
        zAxisLine = zAxis.GetComponent<LineRenderer>();
        zAxisLine.enabled = false;
    }

    void Update()
    {
        if (updated)
        {
            (var xPos, var yPos, var zPos) = positions;
            xAxisLine.SetPositions(xPos);
            xAxisLine.enabled = visibility;

            yAxisLine.SetPositions(yPos);
            yAxisLine.enabled = visibility;

            zAxisLine.SetPositions(zPos);
            zAxisLine.enabled = visibility;
        }
    }

    public void update(Graph graphData)
    {
        (Vector3 min, Vector3 max) = graphData.getMinMax();
        Vector3 startPosition = graphData.getPosition();
        visibility = graphData.getVisibility();

        Vector3 length = max-min;

        positions = (
            new [] {startPosition, startPosition + new Vector3(length.x, 0, 0)},
            new [] {startPosition, startPosition + new Vector3(0, length.y, 0)},
            new [] {startPosition, startPosition + new Vector3(0, 0, length.z)});

        updated = true;
    }
}
