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

public interface GraphRenderer
{
    public void update(List<Vector3> vectorList, List<string> labelList);
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
    public int scale = 1;
    public Vector3 position = new Vector3(0, 0, 0);

    // Internal state
    private GraphRenderer graphRenderer;
    private List<Dictionary<string, object>> rawData;
    private List<Vector3> vectorList;
    private List<string> labelList;
    private bool graphUpdated;


    void Update () 
	{
		if (graphUpdated)
		{
            graphRenderer.update(vectorList, labelList);
            graphUpdated = false;
		}
	}

    public void onPress(){
        Debug.Log("Graph.cs :: Pressed");

        getData();
        processData();
        chooseRenderer();

        graphUpdated = true;

    }

    public void updateFile(string newFilename)
    {
        Debug.Log("Graph.cs :: Update filename attribute");
        filename = newFilename;
        //get new data
        graphUpdated = true;
    }

    public void updateDatatype(DataType newDatatype)
    {
        Debug.Log("Graph.cs :: Update datatype attribute");
        datatype = newDatatype;
        processData();
        graphUpdated = true;
    }

    public void updateGraphRenderer(GraphType newGraphtype)
    {
        Debug.Log("Graph.cs :: Update datatype attribute");
        graphtype = newGraphtype;
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
        switch(datatype) 
        {
        case DataType.Scatter:
            (vectorList, labelList) = GetData.process(rawData);
            break;
        case DataType.TimeValue:
            vectorList = GetSharePriceData.process(rawData);
            break;
        case DataType.Orderbooks:
            //vectorList = GetOrderbookData.process(rawData);
            break;
        case DataType.None:
            Debug.Log("Graph.cs :: No Datatype selected");
            break;
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
}
