using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddGraph : MonoBehaviour
{
    public GameObject tileContainer;
    public GameObject tilePrefab;
    public GameObject graphPrefab;
    public Settings settings;
    public PageScroll scrollManager;

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

        Debug.Log("AddGraph.cs :: Set tile parent");
        newTile.transform.SetParent(tileContainer.transform, false);

        Debug.Log("AddGraph.cs :: Link tile to Graph, settings and scroll manager");
        InputHandler inputHandler = newTile.GetComponent<InputHandler>();
        inputHandler.setGraph(newGraph);
        inputHandler.settings = settings;
        inputHandler.scrollManager = scrollManager;
    }
}
