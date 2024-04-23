using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text.RegularExpressions;

public class GetOrderbookData : MonoBehaviour
{
    private static float dateTimeToFloat(DateTime dt)
    {
        return (float)0.1*(dt.Minute + 0.01666f*dt.second);//We do this to the nearest minute, but possible to include any amount of time in the calculation
    }

    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r"; // Define line delimiters, regular experession craziness

    public static (List<Vector3>, float, float, float, float, float, float) ReadOrderbook()
    {
        var list = new List<Dictionary<string, object>>(); //declare dictionary list

        TextAsset data = Resources.Load("Data/AppleOrderbook/orderbooks_appl") as TextAsset; //Loads the TextAsset named in the file argument of the function

        Debug.Log("GetOrderbookData.cs :: Data loaded:" + data.text); // Print raw data, make sure parsed correctly

        string[] csvLines = Regex.Split(data.text, LINE_SPLIT_RE); // Split data.text into lines using LINE_SPLIT_RE characters

        // Read the contents of the CSV files as individual lines
        var entries = new List<Entry>();
        //dictionary of time to (price, count) for bids and asks
        Dictionary<float, Dictionary<float, int>> bids = new Dictionary<float, Dictionary<float, int>>();
        Dictionary<float, Dictionary<float, int>> asks = new Dictionary<float, Dictionary<float, int>>();

        float minPrice = float.MaxValue;
        float maxPrice = float.MinValue;
        float minSize = float.MaxValue;
        float maxSize = float.MinValue;
        float minTime = float.MaxValue;
        float maxTime = float.MinValue;

        // Get each entry from csv. Placed in a list for now, maybe unnecessary
        // Also maintain a dict for prices
        for (int i = 1; i < csvLines.Length; i++)
        {
            string[] rowData = csvLines[i].Split(',');

            // Debug.Log(rowData[3]);

            //TODO change the indices being used here when Diane has cleaned data up
            //AskPrice, BidPrice, AskSize, BidSize, AskTime, BidTime
            //Create entry for list (maybe not used)
            var entry = new Entry
            {
                BidTime = dateTimeToFloat(DateTime.ParseExact(rowData[3].Substring(0,15), "yyyyMMdd-HHmmss", null)),
                BidPrice = Convert.ToSingle(rowData[5]),
                BidSize = Convert.ToInt16(rowData[6]),
                AskTime = dateTimeToFloat(DateTime.ParseExact(rowData[7].Substring(0,15), "yyyyMMdd-HHmmss", null)),
                AskPrice = Convert.ToSingle(rowData[9]),
                AskSize = Convert.ToInt16(rowData[10])
            };

            //TODO maybe find another method for this due to float equality check
            //add ask and bid to each dict
            //dict is used to calculate total size of bids at same time and same price

            //first check for existing dictionary at this time
            if (bids.ContainsKey(entry.BidTime))
            {
                //then look if we have an amount for this time
                if (bids[entry.BidTime].ContainsKey(entry.BidPrice))
                {
                    bids[entry.BidTime][entry.BidPrice] += entry.BidSize;
                }
                else //make a new dictionary for this price
                {
                    bids[entry.BidTime].Add(entry.BidPrice, entry.BidSize);
                }
            }
            else //make a dictionary for this time
            {
                Dictionary<float, int> newDict = new Dictionary<float, int>();
                newDict.Add(entry.BidPrice, entry.BidSize);
                bids.Add(entry.BidTime, newDict);
            }

            //repeat for asks
            if (bids.ContainsKey(entry.BidTime))
            {
                //then look if we have an amount for this time
                if (bids[entry.BidTime].ContainsKey(entry.BidPrice))
                {
                    bids[entry.BidTime][entry.BidPrice] += entry.BidSize;
                }
                else //make a new dictionary for this price
                {
                    bids[entry.BidTime].Add(entry.BidPrice, entry.BidSize);
                }
            }
            else //make a dictionary for this time
            {
                Dictionary<float, int> newDict = new Dictionary<float, int>();
                newDict.Add(entry.BidPrice, entry.BidSize);
                bids.Add(entry.BidTime, newDict);
            }

            minBidPrice = Math.Min(minBidPrice, entry.BidPrice);

        }

        List<Vector3> positions = new List<Vector3>();

        

        //get a list of points to plot
        //for now we have:
        //x-axis: price
        //y-axis: size
        //z-axis: time
        foreach(KeyValuePair<(float, float), int> point in points)
        {
            float xPos = point.Key.Item1 - minBidPrice;
            float yPos = point.Value;
            float zPos = point.Key.Item2;
            
            minPrice = Math.Min(minPrice, xPos);
            maxPrice = Math.Max(maxPrice, xPos);
            minSize = Math.Min(minSize, yPos);
            maxSize = Math.Max(maxSize, yPos);
            minTime = Math.Min(minTime, zPos);
            maxTime = Math.Max(maxTime, zPos);

            positions.Add(new Vector3(xPos, yPos, zPos));
        }

        return (positions, minPrice, maxPrice, minSize, maxSize, minTime, maxTime);
    }
}

// All data required for an orderbooks file
public class Entry
{
    public float BidTime {get; set;}
    public float BidPrice {get; set;}
    public int BidSize {get; set;}
    public float AskTime {get; set;}
    public float AskPrice {get; set;}
    public int AskSize {get; set;}
}
