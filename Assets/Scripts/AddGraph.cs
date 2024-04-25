using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddGraph : MonoBehaviour
{
    public GameObject tileContainer;
    public GameObject tilePrefab;
    public GameObject graphPrefab;

    public void onPress(){
        Debug.Log("AddGraph.cs :: Instantiate a new tile");
        GameObject newTile = Instantiate(
            tilePrefab,
            new Vector3(0, 0, 0),
            Quaternion.identity);

        Debug.Log("AddGraph.cs :: Instantiate a new Graph");
        GameObject newGraph = Instantiate(
            graphPrefab,
            new Vector3(0, 0, 0),
            Quaternion.identity);

        Debug.Log("AddGraph.cs :: Set tile parent");
        newTile.transform.SetParent(tileContainer.transform, false);

        Debug.Log("AddGraph.cs :: Link tile to Graph");
        newTile.GetComponent<InputHandler>().setGraph(newGraph);
    }
}