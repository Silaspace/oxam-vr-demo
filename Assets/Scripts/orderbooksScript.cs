using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class orderbooksScript : MonoBehaviour
{
    private float dateTimeToFloat(DateTime dt)
    {
        return (float)0.2*(dt.Minute + (1/60)*dt.Second); //fix
    }

    void ReadOrderbook()
    {
        // Read the contents of the CSV files as individual lines
        //TODO figure out relative file locations in c#
        string[] csvLines = File.ReadAllLines(@"C:\Users\jimmy\OneDrive - Nexus365\Work\Group Practical\oxam-vr-demo\Assets\Resources\Data\orderbooks_appl.csv");
        var entries = new List<Entry>();
        //dictionary of (price, time) to count
        Dictionary<(float, DateTime), int> points = new Dictionary<(float, DateTime), int>();

        float minBidPrice = 10000.0F;

        // Get each entry from csv. Placed in a list for now, maybe unnecessary
        // Also maintain a dict for prices
        //TODO change to accept input from CSVReader
        for (int i = 1; i < csvLines.Length; i++)
        {
            string[] rowData = csvLines[i].Split(',');

            //TODO change the indices being used here when Diane has cleaned data up
            //AskPrice, BidPrice, AskSize, BidSize, AskTime, BidTime
            //Create entry for list (maybe not used)
            var entry = new Entry
            {
                BidTime = DateTime.ParseExact(rowData[3].Substring(0,15), "yyyyMMdd-HHmmss", null),
                BidPrice = Convert.ToSingle(rowData[5]),
                BidSize = Convert.ToInt16(rowData[6]),
                AskTime = DateTime.ParseExact(rowData[7].Substring(0,15), "yyyyMMdd-HHmmss", null),
                AskPrice = Convert.ToSingle(rowData[9]),
                AskSize = Convert.ToInt16(rowData[10])
            };
            entries.Add(entry);

            //TODO maybe find another method for this due to float equality check
            //add point to dict
            //dict is used to calculate total size of bids at same time and same price
            if (points.ContainsKey((entry.BidPrice, entry.BidTime))){
                points[(entry.BidPrice, entry.BidTime)] += 1;
            } else {
                points.Add((entry.BidPrice, entry.BidTime), 1);
            }

            minBidPrice = Math.Min(minBidPrice, entry.BidPrice);

        }

        Vector3[] positions = new Vector3[csvLines.Length * 2]; //TODO maybe change length of this array

        //get a list of points to plot
        //for now we have:
        //x-axis: price
        //y-axis: size
        //z-axis: time
        int j = 0;
        foreach(KeyValuePair<(float, DateTime), int> point in points)
        {
            float xPos = point.Key.Item1 - minBidPrice;
            float yPos = dateTimeToFloat(point.Key.Item2);
            int zPos = point.Value;

            positions[j] = new Vector3(xPos, yPos, zPos);
            j++;
        }
    }
}

// All of the data stored in the csv file orderbooks_appl.
// Could be improved for other orderbooks data
public class Entry
{
    public DateTime BidTime {get; set;}
    public float BidPrice {get; set;}
    public int BidSize {get; set;}
    public DateTime AskTime	{get; set;}
    public float AskPrice {get; set;}
    public int AskSize {get; set;}
}
