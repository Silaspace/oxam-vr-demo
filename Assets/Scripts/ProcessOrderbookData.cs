using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class Vector3EqualityComparer : IEqualityComparer<Vector3>
{
    public bool Equals(Vector3 a, Vector3 b)
    {
        if (a == null && b == null)
            return true;
        if (a == null || b == null)
            return false;
        return a.x == b.x && a.z == b.z;
    }

    public int GetHashCode(Vector3 a)
    {
        if (a == null)
            return 0;
        return a.x.GetHashCode() ^ a.z.GetHashCode();
    }
}

public class ProcessOrderbookData
{
    public static float convertToUnixTimestamp(string date)
    {
        DateTime dt = DateTime.ParseExact(date, "yyyyMMdd-HHmmss", null);
        return (float) dt.Minute + 0.3333333f*(dt.Second/20);
    }

    public static List<Vector3> process(List<Dictionary<string, object>> pointList)
    {
        //dictionary of time to (price, count) for bids and asks
        SortedDictionary<float, SortedDictionary<float, int>> bids = new SortedDictionary<float, SortedDictionary<float, int>>();
        SortedDictionary<float, SortedDictionary<float, int>> asks = new SortedDictionary<float, SortedDictionary<float, int>>();

        //min and max prices to have flat edges of mesh
        float minPrice = float.MaxValue;
        float maxPrice = float.MinValue;

        //TODO change the indices being used here when Diane has cleaned data up
        //Get column data
        var columnList = new List<string>(pointList[1].Keys);
        var bidPriceAxisKey = columnList[5];
        var askPriceAxisKey = columnList[9];
        var bidTimeAxisKey = columnList[3];
        var askTimeAxisKey = columnList[7];
        var bidSizeAxisKey = columnList[6];
        var askSizeAxisKey = columnList[10];

        // Get each entry from the points list
        // Also maintain a dict for prices
        for (int i = 1; i < pointList.Count; i++)
        {
            //AskPrice, BidPrice, AskSize, BidSize, AskTime, BidTime
            var entry = new Entry
            {
                BidTime = convertToUnixTimestamp(Convert.ToString(pointList[i][bidTimeAxisKey]).Substring(0,15)),
                BidPrice = Convert.ToSingle(pointList[i][bidPriceAxisKey]),
                BidSize = Convert.ToInt16(pointList[i][bidSizeAxisKey]),
                AskTime = convertToUnixTimestamp(Convert.ToString(pointList[i][askTimeAxisKey]).Substring(0,15)),
                AskPrice = Convert.ToSingle(pointList[i][askPriceAxisKey]),
                AskSize = Convert.ToInt16(pointList[i][askSizeAxisKey])
            };

            //update min and max price
            minPrice = Math.Min(minPrice, entry.BidPrice);
            maxPrice = Math.Max(maxPrice, entry.AskPrice);

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
                SortedDictionary<float, int> newDict = new SortedDictionary<float, int>
                {
                    { entry.BidPrice, entry.BidSize }
                };
                bids.Add(entry.BidTime, newDict);
            }

            //repeat for asks
            if (asks.ContainsKey(entry.AskTime))
            {
                //then look if we have an amount for this time
                if (asks[entry.AskTime].ContainsKey(entry.AskPrice))
                {
                    asks[entry.AskTime][entry.AskPrice] += entry.AskSize;
                }
                else //make a new dictionary for this price
                {
                    asks[entry.AskTime].Add(entry.AskPrice, entry.AskSize);
                }
            }
            else //make a dictionary for this time
            {
                SortedDictionary<float, int> newDict = new SortedDictionary<float, int>
                {
                    { entry.AskPrice, entry.AskSize }
                };
                asks.Add(entry.AskTime, newDict);
            }
        }

        //TODO for now, we have bids and asks in the same list. we need to decide whether to do this as
        //TODO two meshes or one big mesh with two colour gradients
        //get a list of points to plot
        //for now we have:
        //x-axis: price
        //y-axis: size
        //z-axis: time
        List<Vector3> positions = new List<Vector3>();

        //we use [leftIndex, rightIndex) for each triangulation
        List<(int, int)> indices = new List<(int, int)>();
        int startIndex = 0;

        //for each time value
        foreach(KeyValuePair<float, SortedDictionary<float, int>> timePoint in bids)
        {
            //for each price in decreasing order (so it is cumulative)
            int currentHeight = 0;
            float lowPrice = 0.0f;
            int currentIndex = startIndex;
            foreach(KeyValuePair<float, int> bid in timePoint.Value.Reverse())
            {
                float xPos = bid.Key;
                lowPrice = xPos;
                currentHeight += bid.Value; //increase currentHeight by the size of this bid
                float yPos = currentHeight;
                float zPos = timePoint.Key;

                positions.Add(new Vector3(xPos, yPos, zPos));
                currentIndex += 1;
            }

            //add points to make a straight line on the left
            for (float i = lowPrice - 0.1f; i > minPrice; i -= 0.1f) 
            {
                positions.Add(new Vector3(i, currentHeight, timePoint.Key));
            }
            if (lowPrice != minPrice) positions.Add(new Vector3(minPrice, currentHeight, timePoint.Key));

            //add index to indices
            indices.Add((startIndex, currentIndex));
            startIndex = currentIndex;
        }
        
        //repeat for asks but don't reverse dictionary
        foreach(KeyValuePair<float, SortedDictionary<float, int>> timePoint in asks)
        {
            //for each price in increasing order (so it is cumulative)
            int currentHeight = 0;
            float highPrice = 0.0f;
            int currentIndex = startIndex;
            foreach(KeyValuePair<float, int> ask in timePoint.Value)
            {
                float xPos = ask.Key;
                highPrice = xPos;
                currentHeight += ask.Value; //increase currentHeight by the size of this bid
                float yPos = currentHeight;
                float zPos = timePoint.Key;

                positions.Add(new Vector3(xPos, yPos, zPos));
                currentIndex += 1;
            }
            for (float i = highPrice + 0.1f; i < maxPrice; i+= 0.1f)
            {
                positions.Add(new Vector3(i, currentHeight, timePoint.Key));
            }
            positions.Add(new Vector3(maxPrice, currentHeight, timePoint.Key));

            //add index to indices
            indices.Add((startIndex, currentIndex));
            startIndex = currentIndex;
        }

        // Return positions and the indices of the points we must use for each triangulation
        return (positions, indices);
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
