using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class GetData : MonoBehaviour
{
    // Indices for columns to be assigned
    static int xAxis = 0;
    static int yAxis = 1;
    static int zAxis = 2;
    static int labelIndex = 3;

    public static (List<Vector3>, List<string>) process(List<Dictionary<string, object>> pointList)
    {        
        // Choose column names for the data
        Debug.Log("GetData.cs :: Get names of columns");
        var columnList = new List<string>(pointList[1].Keys);
        var xAxisKey = columnList[xAxis];
        var yAxisKey = columnList[yAxis];
        var zAxisKey = columnList[zAxis];

        // Declare a labels list
        var labels = new List<string>();
        
        // Get minimum and maximum of the data in each axis
        Debug.Log("GetData.cs :: Find normalisation constants");
        (float xMin, float xRange) = normalConstants(pointList, xAxisKey);
        (float yMin, float yRange) = normalConstants(pointList, yAxisKey);
        (float zMin, float zRange) = normalConstants(pointList, zAxisKey);

        // Loop thruough pointList and normalise the data
        Debug.Log("GetData.cs :: Extract and normalise vectors");
        var vectorList = new List<Vector3>();
        for (var i = 0; i < pointList.Count; i++)
        {   
            vectorList.Add(
                scale(new Vector3(
                    (Convert.ToSingle(pointList[i][xAxisKey]) - xMin) / xRange,
                    (Convert.ToSingle(pointList[i][yAxisKey]) - yMin) / yRange,
                    (Convert.ToSingle(pointList[i][zAxisKey]) - zMin) / zRange)));
        }

        // Check if there is a fourth column for labels
        if (columnList.Count > labelIndex) {
            var labelKey = columnList[labelIndex];

            Debug.Log("GetData.cs :: Extract labels");
            labels = new List<string>();

            for (var i = 0; i < pointList.Count; i++) {
                labels.Add(Convert.ToString(pointList[i][labelKey]));
            }
        }

        // Return
        Debug.Log("GetData.cs :: Return vectors and labels");
        return (vectorList, labels);
    }

    private static (float, float) normalConstants(List<Dictionary<string, object>> list, string key)
    {
        //set initial value to first value
        float max = Convert.ToSingle(list[0][key]);
        float min = max;

        for (var i = 1; i < list.Count; i++)
        {
            var newVal = Convert.ToSingle(list[i][key]);

            if (max < newVal)
            {
               max = newVal;
            }
            if (min > newVal) 
            {
                min = newVal;
            }
        }
 
        return (min, max - min);
    }

    private static Vector3 scale(Vector3 position)
	{
		return (position * 40) + new Vector3(-10, 0, 0);
	}
}
