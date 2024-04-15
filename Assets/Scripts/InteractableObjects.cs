using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
//isTrigger should be checked on the sphere collider of dataBall,
// and the mesh renderer unchecked
public class InteractableObjects : MonoBehaviour
{
    // Name of the input file, no extension
    public string inputfile;

    
    // The prefab for the data points to be instantiated
    public GameObject PointPrefab;
    
    // Object which will contain instantiated prefabs in hiearchy
    public GameObject PointHolder;
    

    public void Start()
    {
        // Get vectorList to plot
        Debug.Log("ScatterPlot.cs :: Fetch Data");
        (var vectorList, var labels, var namesList) = GetNames.fetch(inputfile);
        
        // Loop through Pointlist
        Debug.Log("ScatterPlot.cs :: Plot Data");
        for (var i = 0; i < vectorList.Count; i++)
        {
            // Create new game object
            GameObject dataPoint = Instantiate(
                    PointPrefab, 
                    scale(vectorList[i]), 
                    Quaternion.identity);

            // Assigns name to the prefab
            dataPoint.transform.name = namesList[i];
            // Make dataPoint child of PointHolder object 
            dataPoint.transform.parent = PointHolder.transform;
        }

    }
    private Vector3 scale(Vector3 position)
	{
		return (position * 25) + new Vector3(-10, 0, 0);
	}
}