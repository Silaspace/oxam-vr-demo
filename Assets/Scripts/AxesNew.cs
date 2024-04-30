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
    private GameObject yAxis;
    private GameObject zAxis;
    private TextMeshPro xAxisTextMesh;
    private TextMeshPro yAxisTextMesh;
    private TextMeshPro zAxisTextMesh;
    private LineRenderer xAxisLine;
    private LineRenderer yAxisLine;
    private LineRenderer zAxisLine;

    private (Vector3, Vector3) positions;
    private bool updated = false;

    void Start()
    {
        Debug.Log("AxesNew.cs :: Instantiate Lines");

        xAxis = Instantiate(
            linePrefab,
            new Vector3(0, 0, 0),
            Quaternion.identity);

        // Set Parent
        xAxis.transform.SetParent(gameObject.transform, false);
        //xAxisTextMesh = xAxisObject.GetComponent<TextMeshPro>();
        xAxisLine = xAxis.GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (updated)
        {
            xAxisLine.SetPositions(positions);
        }
    }

    void update(Graph graphData)
    {
        //var scale = graphData.getScale();
        //var position = graphData.getPosition();
        var scale = new Vector3(1, 1, 1);
        var startPosition = new Vector3(0, 0.5, 0);

        var endPosition = Vector3.Scale(startPosition, scale);
        positions = (startPosition, endPosition);

        updated = true;
    }
}
