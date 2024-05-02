using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DataType
{
    None,
    Scatter,
    Orderbooks,
    TimeValue
}

public enum GraphType
{
    Scatter,
    Mesh,
    None
}

public enum GraphColor
{
    Magma,
    Inferno,
    Plasma,
    Viridis,
    Static,
    ByLabel,
    None
}

public interface GraphRenderer
{
    public void update(Graph graphData);
}

public interface DataProcesser
{
    public List<Vector3> process(List<Dictionary<string, object>> rawData);
}

public class Graph : MonoBehaviour
{
    // Graph properties
    public string displayName;
    public string filename;
    public GraphType graphtype = GraphType.None;
    public DataType datatype = DataType.None;
    public GraphColor graphcolor = GraphColor.None;
    public Vector3 scale = new Vector3(1, 1, 1);
    public Vector3 position = new Vector3(0, 0, 0);
    public bool visibility = false;
    public bool showAxis = false;

    // Internal state
    private GraphRenderer graphRenderer;
    private GraphRenderer axisRenderer;
    private List<Dictionary<string, object>> rawData;

    private List<Vector3> vectorList;
    private List<Vector3> vectorScaledList;
    private List<(int, int)> indexList;
    private List<string> labelList;
    private List<Color> colorList;
    private Vector3 vectorMax;
    private Vector3 vectorMin;
    private Vector3 vectorScaledMax;
    private Vector3 vectorScaledMin;
    private bool graphUpdated;


    void Update() 
	{
		if (graphUpdated)
		{
            graphRenderer.update(this);
            axisRenderer.update(this);
            graphUpdated = false;
		}
	}

    void Start()
    {
        Debug.Log("Graph.cs :: Process all data on Start");

        // Find the appropriate rendering components
        chooseRenderer();
        axisRenderer = GetComponent<Axes>();

        // Fetch and process initial data
        getData();      // 1
        processData();  // 2
        scaleData();    // 3
        colorGraph();   // 4
    }

    public List<Vector3> getVectorList()
    {
        return vectorScaledList;
    }

    public List<(int, int)> getIndicesList()
    {
        return indexList;
    }

    public List<string> getLabels()
    {
        return labelList;
    }
    
    public List<Color> getColors()
    {
        return colorList;
    }

    public bool getVisibility()
    {
        return visibility;
    }

    public Vector3 getPosition()
    {
        return position;
    }

    public Vector3 getScale()
    {
        return scale;
    }

    public (Vector3, Vector3) getMinMax()
    {
        return (vectorScaledMin, vectorScaledMax);
    }

    public void updateFile(string newFilename)
    {
        Debug.Log("Graph.cs :: Update filename attribute");
        filename = newFilename;
        getData();
        processData();
        colorGraph();
        scaleData();
        graphUpdated = true;
    }

    public void updateDatatype(DataType newDatatype)
    {
        Debug.Log("Graph.cs :: Update datatype attribute");
        datatype = newDatatype;
        processData();
        colorGraph();
        scaleData();
        graphUpdated = true;
    }

    public void updateGraphRenderer(GraphType newGraphtype)
    {
        Debug.Log("Graph.cs :: Update datatype attribute");
        graphtype = newGraphtype;
        chooseRenderer();
        graphUpdated = true;
    }

    public void updateGraphColor(GraphColor newGraphColor)
    {
        Debug.Log("Graph.cs :: Update datatype attribute");
        graphcolor = newGraphColor;
        colorGraph();
        graphUpdated = true;
    }

    public void updateVisibility(bool newVisibility)
    {
        Debug.Log("Graph.cs :: Update graph visibility");
        visibility = newVisibility;
        graphUpdated = true;
    }

    public void updateScale(Vector3 newScale)
    {
        Debug.Log("Graph.cs :: Update graph visibility");
        scale = newScale;
        scaleData();
        colorGraph();
        graphUpdated = true;
    }

    public void updatePosition(Vector3 newPosition)
    {
        Debug.Log("Graph.cs :: Update graph visibility");
        position = newPosition;
        scaleData();
        colorGraph();
        graphUpdated = true;
    }

    private void getData()
    {
        Debug.Log("Graph.cs :: Fetch raw data from CSV");
        rawData = CSVReader.Read(filename);
    }

    private void processData()
    {
        Debug.Log("Graph.cs :: Process the rawData into a vectorList");

        // Proces rawData into the vectorList
        switch(datatype) 
        {
        case DataType.Scatter:
            (vectorList, labelList) = ProcessScatterData.process(rawData);
            break;
        case DataType.TimeValue:
            (vectorList, indexList) = ProcessSharePriceData.process(rawData);
            break;
        case DataType.Orderbooks:
            (vectorList, indexList) = ProcessOrderbookData.process(rawData);
            break;
        case DataType.None:
            Debug.Log("Graph.cs :: No Datatype selected");
            break;
        }

        // Update other properties based on the new vectorList
        Debug.Log("Graph.cs :: Set vectorMax and vectorMin");
        vectorMax = vectorList[0];
        vectorMin = vectorMax;
        for (int i = 1; i < vectorList.Count; i++){
            vectorMax = Vector3.Max(vectorMax, vectorList[i]);
            vectorMin = Vector3.Min(vectorMin, vectorList[i]);
        }

        Debug.Log("Graph.cs :: Scale data to common values");
        for (int i = 0; i < vectorList.Count; i++)
        {
            var vector = vectorList[i];
            vector.x = map(vector.x, vectorMin.x, vectorMax.x, -1, 1);
            vector.y = map(vector.y, vectorMin.y, vectorMax.y, 0, 2);
            vector.z = map(vector.z, vectorMin.z, vectorMax.z, -1, 1);
            vectorList[i] = Vector3.Scale(vector, scale) + position;
        }
    }

    private void chooseRenderer()
    {
        Debug.Log("Graph.cs :: Switch renderer");
        switch(graphtype) 
        {
        case GraphType.Scatter:
            Debug.Log("Graph.cs :: Locate scatter plot renderer");
            graphRenderer = GetComponent<ScatterPlot>();
            break;
        case GraphType.Mesh:
            Debug.Log("Graph.cs :: Locate mesh generator");
            graphRenderer = GetComponent<MeshGenerator>();
            break;
        case GraphType.None:
            Debug.Log("Graph.cs :: No Graphtype selected");
            break;
        }
    }

    private void colorGraph()
    {
        Debug.Log("Graph.cs :: Assign a color to every point in the vectorList");
        colorList = new List<Color>();

        switch(graphcolor) 
        {
        case GraphColor.Magma:
        case GraphColor.Inferno:
        case GraphColor.Plasma:
        case GraphColor.Viridis:
            for (int i = 0; i < vectorList.Count; i += 1)
            {
                colorList.Add(
                    CustomColors.GetColor(
                        vectorList[i].y,
                        vectorScaledMin.y,
                        vectorScaledMax.y,
                        graphcolor));
            }
            break;
        case GraphColor.Static:
            for (int i = 0; i < vectorList.Count; i += 1)
            {
                colorList.Add(Color.red);
            }
            break;
        case GraphColor.ByLabel:
            Dictionary<string, Color> labelColors = new Dictionary<string, Color>();

            for (int i = 0; i < vectorList.Count; i += 1)
            {
                string label = labelList[i];

                if (labelColors.ContainsKey(label))
                {
                    colorList.Add(labelColors[label]);
                }
                else
                {
                    Color newColor = CustomColors.GetLabelColor(label);
                    labelColors[label] = newColor;
                    colorList.Add(newColor);
                }
            }
            break;
        case GraphColor.None:
            Debug.Log("Graph.cs :: No color selected");
            break;
        }
    }

    private void scaleData()
    {
        vectorScaledMax = position;
        vectorScaledMin = vectorScaledMax;

        for (int i = 0; i < vectorList.Count; i++)
        {
            // Scale vector
            vectorScaledList.Add(
                Vector3.Scale(vectorList[i], scale) + position);

            // Set scaledMin and scaledMax
            vectorScaledMax = Vector3.Max(vectorScaledMax, vectorList[i]);
            vectorScaledMin = Vector3.Min(vectorScaledMin, vectorList[i]);
        }
    }

    private float map(float x, float input_start, float input_end, float output_start, float output_end)
    {
        return (x - input_start) / (input_end - input_start) * (output_end - output_start) + output_start;
    }
}
