using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GetSharePriceData : MonoBehaviour
{

    // fetchPoints fetches pre-processed point data for the mesh, which is a CSV with columns
    //   TimeShort TimeLong Price
    // where: 
    //   TimeShort is the repeating time window (e.g. how far through a particular year we are)
    //   TimeLong is the longer time window (e.g. what particular year it is)
    //   Price is the value that should be the height of the mesha
    // Example data of this form can be found in `RoyalMailSharePrice/price_processed.csv`, but in
    // practice you should use use the `fetch` function.
    public static List<Vector3> fetchPreprocessedPoints(string inputfile)
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

    // process readies data from the mesh in CSVs with columns
    //   Date Price
    // where:
    //   Date is a string in YYYY-MM-DD format
    //   Price a float
    // It takes as parameters:
    //   inputfile: location of dataset
    //   repeatPeriod: how long to make repeating window
    //   takeEvery: setting this to 1 will use every value in the dataset, setting this to 10 will use every 10th value, etc.
    //              useful for smoothing out chaotic data
    // And returns (points, timeShortMin, timeShortMax, timeLongMin, timeLongMax, priceMin, priceMax):
    //   points: List of Vector3s representing each point
    //   timeShortMin, timeShortMax, timeLongMin, timeLongMax, priceMin, priceMax: min/maxes as expected
    // Points are of the format
    //   (timeShort, timeLong, price)
    // where:
    //   timeShort is the repeating time window (e.g. how far through a particular year we are), as the number of seconds through current period
    //   timeLong is the longer time window (e.g. what particular year it is), as a unix timestamp
    //   price is the value that should be the height of the mesha
    public static List<Vector3> process(List<Dictionary<string, object>> pointList)
    {
        //Proposed Parameters
        float repeatPeriod = (float)(60.0 * 60.0 * 24.0 * 365.25);
        int takeEvery = 10; // Use every nth value in the dataset
        float xSize = 2f;
        float ySize = 0.5f;
        float zSize = 2f;

        //Get column data
        var columnList = new List<string>(pointList[1].Keys);
        var dateAxisKey = columnList[0];
        var priceAxisKey = columnList[1];

        var datePricePairs = new List<Vector2>();
        for (var i = 0; i < pointList.Count; i++)
        {
            datePricePairs.Add(
                new Vector3(
                    convertToUnixTimestamp(Convert.ToString(pointList[i][dateAxisKey])),
                    Convert.ToSingle(pointList[i][priceAxisKey])
                )
            );
        }

        float startTimestamp = float.MaxValue;
        float endTimestamp = float.MinValue;
        float minPrice = float.MaxValue;
        float maxPrice = float.MinValue;

        for (var i = 0; i < datePricePairs.Count; i++)
        {
            startTimestamp = Math.Min(startTimestamp, datePricePairs[i][0]);
            endTimestamp = Math.Max(endTimestamp, datePricePairs[i][0]);
            minPrice = Math.Min(minPrice, datePricePairs[i][1]);
            maxPrice = Math.Max(maxPrice, datePricePairs[i][1]);
        }

        int numberOfPeriods = (int)(Math.Floor((endTimestamp - startTimestamp) / repeatPeriod)) + 1;

        var processedPoints = new List<Vector3>();

        for (var i = 0; i < datePricePairs.Count; i++)
        {
            if (i % takeEvery != 0) {
                continue;
            }

            float currentTimestamp = datePricePairs[i][0];
            float price = datePricePairs[i][1];

            int periodIndex = (int)Math.Floor((currentTimestamp - startTimestamp) / repeatPeriod);

            float periodStart = startTimestamp + periodIndex * repeatPeriod;
            float periodEnd = startTimestamp + (periodIndex + 1) * repeatPeriod;

            float timeShort = MapRange(currentTimestamp, periodStart, periodEnd, 0, repeatPeriod);

            // Map to x,y.z
            float x = MapRange(timeShort, 0, repeatPeriod, 0, xSize);
            float y = MapRange(price, minPrice, maxPrice, 0, ySize);
            float z = MapRange(periodStart, startTimestamp, endTimestamp, 0, zSize);
            
            processedPoints.Add(new Vector3(x, y, z));
        }

        return processedPoints;
    }

    public static float MapRange(float value, float leftMin, float leftMax, float rightMin, float rightMax)
    {
        float leftSpan = leftMax - leftMin;
        float rightSpan = rightMax - rightMin;
        float valueScaled = (value - leftMin) / leftSpan;
        
        return rightMin + (valueScaled * rightSpan);
    }

    // converts strings of the form "YYYY-MM-DD" to their corresponding unix timestamp (seconds since Jan 1st, 1970).
    public static float convertToUnixTimestamp(string date)
    {
        DateTime dateTime;
        if (DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime)) {
            DateTime epoch = new DateTime(1970, 1, 1);
            TimeSpan timeSpan = dateTime - epoch;
            return (float)timeSpan.TotalSeconds;
        } else {
            throw new ArgumentException("Invalid date format " + date);
        }
    }
}
