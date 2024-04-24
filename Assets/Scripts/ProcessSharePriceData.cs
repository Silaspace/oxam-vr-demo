using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ProcessSharePriceData : DataProcesser
{
    // Converts strings of the form "YYYY-MM-DD" to their corresponding unix timestamp (seconds since Jan 1st, 1970).
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

    // Process readies data from the mesh in CSVs with columns
    //      Date Price
    // Where:
    //      Date is a string in YYYY-MM-DD format
    //      Price a float
    // It takes as (proposed) parameters:
    //      repeatPeriod:   how long to make repeating window
    //      takeEvery:      only uses every nth value from the dataset useful for smoothing out chaotic data
    // And returns a vectorList of format
    //      (timeShort, price, timeLong)
    // Where:
    //   timeShort is the repeating time window (e.g. how far through a particular year we are), as the number of seconds through current period
    //   timeLong is the longer time window (e.g. what particular year it is), as a unix timestamp
    //   price is the value that should be the height of the mesh
    public static List<Vector3> process(List<Dictionary<string, object>> pointList)
    {
        // Proposed Parameters
        float repeatPeriod = (float)(60.0 * 60.0 * 24.0 * 365.25);
        int takeEvery = 10;

        // Get column data
        var columnList = new List<string>(pointList[1].Keys);
        var dateAxisKey = columnList[0];
        var priceAxisKey = columnList[1];

        // Extract the data as tuples of date and price
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

        // Find start-end times
        float startTimestamp = float.MaxValue;
        float endTimestamp = float.MinValue;

        for (var i = 0; i < datePricePairs.Count; i++)
        {
            startTimestamp = Math.Min(startTimestamp, datePricePairs[i][0]);
            endTimestamp = Math.Max(endTimestamp, datePricePairs[i][0]);
        }
        
        // Process data into (time, value, time) tuples
        var processedPoints = new List<Vector3>();

        for (var i = 0; i < datePricePairs.Count; i += takeEvery)
        {
            // Get date and price
            float currentTimestamp = datePricePairs[i][0];
            float price = datePricePairs[i][1];

            // Map the short time axis down
            int periodIndex = (int)Math.Floor((currentTimestamp - startTimestamp) / repeatPeriod);
            float periodStart = startTimestamp + periodIndex * repeatPeriod;
            float periodEnd = startTimestamp + (periodIndex + 1) * repeatPeriod;
            float timeShort = (currentTimestamp - periodStart) / (periodEnd - periodStart) * repeatPeriod
            
            // Add point
            processedPoints.Add(new Vector3(timeShort, price, periodStart));
        }

        return processedPoints;
    }
}
