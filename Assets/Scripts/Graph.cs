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
    None
}

public interface GraphRenderer
{
    public void update(Graph graphData);
}

public interface DataProcesser
{
    public void process(List<Dictionary<string, object>> rawData);
    public void scale(List<Vector3> vectorList);
}

public class Graph : MonoBehaviour
{
    // Graph properties
    public string displayName;
    public string filename;
    public GraphType graphtype = GraphType.None;
    public DataType datatype = DataType.None;
    public GraphColor graphcolor = GraphColor.None;
    public int scale = 1;
    public Vector3 position = new Vector3(0, 0, 0);

    // Internal state
    private GraphRenderer graphRenderer;
    private List<Dictionary<string, object>> rawData;
    private List<Vector3> vectorList;
    private List<string> labelList;
    private List<Color> colorList;
    private Vector3 vectorMax;
    private Vector3 vectorMin;
    private bool graphUpdated;


    void Update() 
	{
		if (graphUpdated)
		{
            graphRenderer.update(this);
            graphUpdated = false;
		}
	}

    void onPress()
    {
        Debug.Log("Graph.cs :: Pressed");

        getData();
        processData();
        colorGraph();
        chooseRenderer();

        graphUpdated = true;

    }

    public List<Vector3> getVectorList()
    {
        return vectorList;
    }

    public List<string> getLabels()
    {
        return labelList;
    }
    
    public List<Color> getColors()
    {
        return colorList;
    }

    public void updateFile(string newFilename)
    {
        Debug.Log("Graph.cs :: Update filename attribute");
        filename = newFilename;
        getData();
        processData();
        colorGraph();
        graphUpdated = true;
    }

    public void updateDatatype(DataType newDatatype)
    {
        Debug.Log("Graph.cs :: Update datatype attribute");
        datatype = newDatatype;
        processData();
        colorGraph();
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
            (vectorList, labelList) = GetData.process(rawData);
            break;
        case DataType.TimeValue:
            vectorList = GetSharePriceData.process(rawData);
            break;
        case DataType.Orderbooks:
            vectorList = GetOrderbookData.process(rawData);
            break;
        case DataType.None:
            Debug.Log("Graph.cs :: No Datatype selected");
            break;
        }

        // Update other properties bsed on the new vectorList
        Debug.Log("Graph.cs :: Set vectorMax and vectorMin");
        vectorMax = vectorList[0];
        vectorMin = vectorMax;
        for (int i = 1; i < vectorList.Count; i++){
            vectorMax = Vector3.Max(vectorMax, vectorList[i]);
            vectorMin = Vector3.Min(vectorMin, vectorList[i]);
        }
    }

    private void chooseRenderer()
    {
        Debug.Log("Graph.cs :: Process the rawData into a vectorList");
        switch(graphtype) 
        {
        case GraphType.Scatter:
            Debug.Log("Graph.cs :: Locate scatter plot renderer");
            var scatterRenderer = GameObject.Find("/Particle System");
            graphRenderer = scatterRenderer.GetComponent<GraphRenderer>();
            break;
        case GraphType.Mesh:
            Debug.Log("Graph.cs :: Locate mesh generator");
            var meshRenderer = GameObject.Find("/Mesh Generator");
            graphRenderer = meshRenderer.GetComponent<GraphRenderer>();
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
                        vectorMin.y,
                        vectorMax.y,
                        graphcolor));
            }
            break;
        case GraphColor.Static:
            for (int i = 0; i < vectorList.Count; i += 1)
            {
                colorList.Add(Color.red);
            }
            break;
        case GraphColor.None:
            Debug.Log("Graph.cs :: No color selected");
            break;
        }
    }
}
