using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ScatterPlot : MonoBehaviour
{
    // Name of the input file, no extension
    public string inputfile;

    // Scale for the plot
    public float plotScale = 1;

    // The prefab for the data points to be instantiated
    public GameObject PointPrefab;
    
    // Object which will contain instantiated prefabs in hiearchy
    public GameObject PointHolder;

    public void plot()
    {
        // Get vectorList to plot
        Debug.Log("ScatterPlot.cs :: Fetch Data");
        var vectorList = GetData.fetch(inputfile);
        
        // Loop through Pointlist
        Debug.Log("ScatterPlot.cs :: Plot Data");
        for (var i = 0; i < vectorList.Count; i++)
        {
            // Create new game object
            GameObject dataPoint = Instantiate(
                    PointPrefab, 
                    vectorList[i] * plotScale, 
                    Quaternion.identity);

            // Make dataPoint child of PointHolder object 
            dataPoint.transform.parent = PointHolder.transform;
        }

    }
}
