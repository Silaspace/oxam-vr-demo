using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AddGraph : MonoBehaviour
{
    public GameObject tileContainer;
    public GameObject tilePrefab;
    public GameObject graphPrefab;
    public Settings settings;
    public PageScroll scrollManager;

    private int graphNumber = 1;

    public void onPress(){
        Debug.Log("AddGraph.cs :: Instantiate a new tile");
        GameObject newTile = Instantiate(
            tilePrefab,
            new Vector3(0, 0, 0),
            Quaternion.identity);

        Debug.Log("AddGraph.cs :: Instantiate a new Graph");
        GameObject newGraph = Instantiate(
            graphPrefab,
            new Vector3(0, 0, 2.5f),
            Quaternion.identity);

        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
	if(text != null)
	{
	    Debug.Log("AddGraph.cs :: Setting text to Custom Graph " + (graphNumber + 1).ToString());
	    text.text = "Custom Graph " + (graphNumber + 1).ToString();
	    graphNumber += 1;
	}
	else
	{
	    Debug.Log("AddGraph.cs :: Returned text was null");
	}

        Debug.Log("AddGraph.cs :: Set tile parent");
        newTile.transform.SetParent(tileContainer.transform, false);

        Debug.Log("AddGraph.cs :: Link tile to Graph, settings and scroll manager");
        InputHandler inputHandler = newTile.GetComponent<InputHandler>();
        inputHandler.setGraph(newGraph);
        inputHandler.settings = settings;
        inputHandler.scrollManager = scrollManager;
    }
}
