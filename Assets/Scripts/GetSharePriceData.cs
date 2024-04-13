using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetSharePriceData : MonoBehaviour
{

    public static List<Vector3> fetch(string inputfile)
    {
        var pointList = CSVReader.Read(inputfile);

        var columnList = new List<string>(pointList[1].Keys);

        var xAxisKey = columnList[0];
        var yAxisKey = columnList[1];
        var zAxisKey = columnList[2];

        var vectorList = new List<Vector3>();
        for (var i = 0; i < pointList.Count; i++)
        {
            vectorList.Add(
                new Vector3(
                    Convert.ToSingle(pointList[i][xAxisKey]),
                    Convert.ToSingle(pointList[i][yAxisKey]),
                    Convert.ToSingle(pointList[i][zAxisKey])
                )
            );
        }

        return vectorList;
    }
}
