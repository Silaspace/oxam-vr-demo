using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AxesNew : MonoBehaviour, GraphRenderer
{
    // Public properties
    public string xAxisLabel = "X axis";
    public string yAxisLabel = "Y axis";
    public string zAxisLabel = "Z axis";
    public bool bidirectional = false;
    public GameObject linePrefab;

    // Private properties
    private GameObject xAxis;
    private LineRenderer xAxisLine;

    private (Vector3[], Vector3[], Vector3[]) positions;
    private bool updated = false;

    void Start()
    {
        Debug.Log("AxesNew.cs :: Instantiate Lines");

        xAxis = Instantiate(
            linePrefab,
            new Vector3(0, 0, 0),
            Quaternion.identity);

        // Set Parent and get line renderer
        xAxis.transform.SetParent(gameObject.transform, false);
        xAxisLine = xAxis.GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (updated)
        {
            (var xPos, var yPos, var zPos) = positions;
            xAxisLine.SetPositions(xPos);
        }
    }

    public void update(Graph graphData)
    {
        //(Vector3 min, Vector3 max) = graphData.getMinMax();
        //Vector3 position = graphData.getPosition();
        Vector3 startPosition = new Vector3(0, 0, 0);
        (Vector3 min, Vector3 max) = (new Vector3(0, 0, 0), new Vector3(1, 1, 1));

        Vector3 length = max-min;

        positions = (
            new [] {startPosition, startPosition + new Vector3(length.x, 0, 0)},
            new [] {startPosition, startPosition + new Vector3(0, length.y, 0)},
            new [] {startPosition, startPosition + new Vector3(0, 0, length.z)});

        updated = true;
    }
}
