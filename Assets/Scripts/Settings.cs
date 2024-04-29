using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    // Graph to modify
    public Graph graph;

    // Various setting dropdowns and buttons
    public TMP_Dropdown fileDropdown;
    public FileDropdown fileDropdownScript;
    public TMP_Dropdown dataTypeDropdown;
    public TMP_Dropdown graphTypeDropdown;
    public TMP_Dropdown colourDropdown;
    public Toggle visibilityToggle;
    public Toggle deleteToggle;

    // Scroll Manager, to scroll back to the main graph menu
    public PageScroll scrollManager;

    // Graph and Tile gameobjects, to delete
    public GameObject graphTile;
    public GameObject graphObject;

    void Start()
    {
		changeGraph(graphTile, graphObject);
    }

    public void changeGraph(GameObject newTile, GameObject newGraphObject)
    {
		graphTile = newTile;
		graphObject = newGraphObject;
		Graph newGraph = newGraphObject.GetComponent<Graph>();
		graph = newGraph;
		Debug.Log("Settings.cs :: New graph has been selected");
		fileDropdownScript.updateOptions();

		Debug.Log("Settings.cs :: graph filename is " + graph.filename);
		fileDropdownScript.setIndexFromFilename(graph.filename);

		switch(graph.datatype)
		{
			case DataType.None:
				dataTypeDropdown.value = 0;
				break;
			case DataType.Scatter:
				dataTypeDropdown.value = 1;
				break;
			case DataType.Orderbooks:
				dataTypeDropdown.value = 2;
				break;
			case DataType.TimeValue:
				dataTypeDropdown.value = 3;
				break;
		}
		switch(graph.graphtype)
		{
			case GraphType.None:
				graphTypeDropdown.value = 0;
				break;
			case GraphType.Scatter:
				graphTypeDropdown.value = 1;
				break;
			case GraphType.Mesh:
				graphTypeDropdown.value = 2;
				break;
		}

		switch(graph.graphcolor)
		{
			case GraphColor.None:
				colourDropdown.value = 0;
				break;
			case GraphColor.Magma:
				colourDropdown.value = 1;
				break;
			case GraphColor.Inferno:
				colourDropdown.value = 2;
				break;
			case GraphColor.Plasma:
				colourDropdown.value = 3;
				break;
			case GraphColor.Viridis:
				colourDropdown.value = 4;
				break;
			case GraphColor.Static:
				colourDropdown.value = 5;
				break;
			case GraphColor.ByLabel:
				colourDropdown.value = 6;
				break;
		}

		visibilityToggle.isOn = graph.visibility;
		deleteToggle.isOn = false;
    }

    public void changeFile()
    {
		string fileName = fileDropdownScript.getFileName();
		if(fileName != null && fileName != graph.filename)
		{
			Debug.Log("Settings.cs :: Setting file name to " + fileName);
			graph.updateFile(fileName);
		}
		else
		{
			Debug.Log("Settings.cs :: File name hasn't changed or is null, doing nothing");
		}
    }

    public void changeDatatype()
    {
		DataType newType = DataType.None;
		switch(dataTypeDropdown.value)
		{
			case 1:
				newType = DataType.Scatter;
				break;
			case 2:
				newType = DataType.Orderbooks;
				break;
			case 3:
				newType = DataType.TimeValue;
				break;
			default:
				newType = DataType.None;
				break;
		}

		if(newType != graph.datatype){
			Debug.Log("Settings.cs :: Changing Data Type");
			graph.updateDatatype(newType);
		}
		else
		{
			Debug.Log("Settings.cs :: Data Type didn't change");
		}
    }

    public void changeGraphType()
    {
		GraphType newType = GraphType.None;
		switch(graphTypeDropdown.value)
		{
			case 1:
				newType = GraphType.Scatter;
				break;
			case 2:
				newType = GraphType.Mesh;
				break;
			default:
				newType = GraphType.None;
				break;
		}

		if(newType != graph.graphtype){
			Debug.Log("Settings.cs :: Changing Graph Type");
			graph.updateGraphRenderer(newType);
		}
		else
		{
			Debug.Log("Settings.cs :: Graph Type didn't change");
		}
    }

    public void changeColour()
    {
		GraphColor newColour = GraphColor.None;
		switch(colourDropdown.value)
		{
			case 1:
				newColour = GraphColor.Magma;
				break;
			case 2:
				newColour = GraphColor.Inferno;
				break;
			case 3:
				newColour = GraphColor.Plasma;
				break;
			case 4:
				newColour = GraphColor.Viridis;
				break;
			case 5:
				newColour = GraphColor.Static;
				break;
			case 6:
				newColour = GraphColor.ByLabel;
				break;
			default:
				newColour = GraphColor.None;
				break;
		}

		if(newColour != graph.graphcolor){
			Debug.Log("Settings.cs :: Changing Colour");
			graph.updateGraphColor(newColour);
		}
		else
		{
			Debug.Log("Settings.cs :: Colour didn't change");
		}
    }

    public void changeVisibility()
    {
		if(visibilityToggle.isOn != graph.visibility)
		{
			Debug.Log("Settings.cs :: Changing Visibility");
			graph.updateVisibility(visibilityToggle.isOn);
		}
		else
		{
			Debug.Log("Settings.cs :: Visibility didn't change");
		}
    }

    public void deleteGraph()
    {
		if(deleteToggle.isOn)
		{
			Debug.Log("Settings.cs :: Deleting Graph");
			Destroy(graphObject);
			Destroy(graphTile);
			scrollManager.SetPageIndex(0);
		}
    }

    public void backButton()
    {
		scrollManager.SetPageIndex(0);
    }    
}
