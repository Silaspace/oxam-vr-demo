using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DataType
{
    None,
    Scatter,
    Orderbooks,
    TimeValue,
    Raw
}

public interface GraphRenderer
{
    public void update(List<Vector3> vectorList, List<string> labelList);
}

public class Graph : MonoBehaviour
{
    // Graph properties
    public string displayName;
    public string filename;
    public GraphRenderer graphRenderer;
    public DataType datatype = DataType.None;
    public int scale = 1;
    public Vector3 position = new Vector3(0, 0, 0);

    // Internal state
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

    void Start () 
	{
        Debug.Log("Graph.cs :: Locate scatter plot renderer");
        var scatterRenderer = GameObject.Find("/Particle System");
        graphRenderer = scatterRenderer.GetComponent<GraphRenderer>();
	}

    public void onPress(){
        Debug.Log("Graph.cs :: Pressed");

        getData();
        processData();

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

    public void updateGraphRenderer(GraphRenderer newRenderer)
    {
        Debug.Log("Graph.cs :: Update datatype attribute");
        graphRenderer = newRenderer;
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
        (vectorList, labelList) = GetData.process(rawData, datatype);
    }
}
