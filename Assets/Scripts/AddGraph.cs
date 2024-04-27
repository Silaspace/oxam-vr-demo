using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddGraph : MonoBehaviour
{
    public GameObject tileContainer;

    public GameObject tilePrefab;

    public void onPress(){
        Debug.Log("AddGraph.cs :: Instantiate");
        GameObject newTile = Instantiate(
            tilePrefab,
            new Vector3(0, 0, 0),
            Quaternion.identity);

        Debug.Log("AddGraph.cs :: Set tile parent");
        newTile.transform.SetParent(tileContainer.transform, false);
    }
}