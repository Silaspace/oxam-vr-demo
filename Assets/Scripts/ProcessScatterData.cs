using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class ProcessScatterData : DataProcesser
{
    public static (List<Vector3>, List<string>) processScatterPlot(List<Dictionary<string, object>> pointList)
    {
        // Indices for columns to be assigned - potential parameters
        int xAxis = 0;
        int yAxis = 1;
        int zAxis = 2;
        int labelIndex = 3;

        // Choose column names for the data
        Debug.Log("GetData.cs :: Get names of columns");
        var columnList = new List<string>(pointList[1].Keys);

        var xAxisKey = columnList[xAxis];
        var yAxisKey = columnList[yAxis];
        var zAxisKey = columnList[zAxis];
        var labelKey = columnList[labelIndex];

        // Declare a labels list
        var labels = new List<string>();

        // Loop thruough pointList extract the data
        Debug.Log("GetData.cs :: Extract vectors");
        var vectorList = new List<Vector3>();
        var labels = new List<string>();

        for (var i = 0; i < pointList.Count; i++)
        {   
            vectorList.Add(
                new Vector3(
                    Convert.ToSingle(pointList[i][xAxisKey]),
                    Convert.ToSingle(pointList[i][yAxisKey]) ,
                    Convert.ToSingle(pointList[i][zAxisKey])));
            
            labels.Add(
                Convert.ToString(pointList[i][labelKey]));
        }

        // Return
        Debug.Log("GetData.cs :: Return vectors and labels");
        return (vectorList, labels);
    }
}
