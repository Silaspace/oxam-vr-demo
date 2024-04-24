using System;
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

public class GetOrderbookData : MonoBehaviour
{
    private static float dateTimeToFloat(DateTime dt)
    {
        return (float)0.1*(dt.Minute + 0.01666f*dt.Second);//We do this to the nearest minute, but possible to include any amount of time in the calculation
    }

    public static List<Vector3> process(List<Dictionary<string, object>> pointList)
    {
        //dictionary of time to (price, count) for bids and asks
        SortedDictionary<float, SortedDictionary<float, int>> bids = new SortedDictionary<float, SortedDictionary<float, int>>();
        SortedDictionary<float, SortedDictionary<float, int>> asks = new SortedDictionary<float, SortedDictionary<float, int>>();

        float minPrice = float.MaxValue;
        //float maxPrice = float.MinValue;
        //float minSize = float.MaxValue;
        //float maxSize = float.MinValue;
        //float minTime = float.MaxValue;
        //float maxTime = float.MinValue;

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
                BidTime = dateTimeToFloat(DateTime.ParseExact(Convert.ToString(pointList[i][bidTimeAxisKey]).Substring(0,15), "yyyyMMdd-HHmmss", null)),
                BidPrice = Convert.ToSingle(pointList[i][bidPriceAxisKey]),
                BidSize = Convert.ToInt16(pointList[i][bidSizeAxisKey]),
                AskTime = dateTimeToFloat(DateTime.ParseExact(Convert.ToString(pointList[i][askTimeAxisKey]).Substring(0,15), "yyyyMMdd-HHmmss", null)),
                AskPrice = Convert.ToSingle(pointList[i][askPriceAxisKey]),
                AskSize = Convert.ToInt16(pointList[i][askSizeAxisKey])
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

            minPrice = Math.Min(minPrice, Math.Min(entry.AskPrice, entry.AskPrice));

        }

        //TODO for now, we have bids and asks in the same list. we need to decide whether to do this as
        //two meshes or one big mesh with two colour gradients
        //get a list of points to plot
        //for now we have:
        //x-axis: price
        //y-axis: size
        //z-axis: time
        List<Vector3> positions = new List<Vector3>();

        //TODO current inefficient implementation to test cumulative look
        //for each time value
        foreach(KeyValuePair<float, SortedDictionary<float, int>> timePoint in bids)
        {
            //for each price in decreasing order (so it is cumulative)
            int current = 0;
            foreach(KeyValuePair<float, int> bid in timePoint.Value.Reverse())
            {
                float xPos = bid.Key - minPrice;
                current += bid.Value; //increase current by the size of this bid
                float yPos = current;
                float zPos = timePoint.Key;

                //minPrice = Math.Min(minPrice, xPos);
                //maxPrice = Math.Max(maxPrice, xPos);
                //minSize = Math.Min(minSize, yPos);
                //maxSize = Math.Max(maxSize, yPos);
                //minTime = Math.Min(minTime, zPos);
                //maxTime = Math.Max(maxTime, zPos);

                positions.Add(new Vector3(xPos, yPos, zPos));
            }
        }

        //repeat for asks but don't reverse dictionary
        foreach(KeyValuePair<float, SortedDictionary<float, int>> timePoint in asks)
        {
            //for each price in increasing order (so it is cumulative)
            int current = 0;
            foreach(KeyValuePair<float, int> ask in timePoint.Value)
            {
                float xPos = ask.Key - minPrice;
                current += ask.Value; //increase current by the size of this bid
                float yPos = current;
                float zPos = timePoint.Key;

                //minPrice = Math.Min(minPrice, xPos);
                //maxPrice = Math.Max(maxPrice, xPos);
                //minSize = Math.Min(minSize, yPos);
                //maxSize = Math.Max(maxSize, yPos);
                //minTime = Math.Min(minTime, zPos);
                //maxTime = Math.Max(maxTime, zPos);

                positions.Add(new Vector3(xPos, yPos, zPos));
            }
        }

        // Remove duplicate point
        return positions.Distinct(new Vector3EqualityComparer()).ToList();
    }

    public static float MapRange(float value, float leftMin, float leftMax, float rightMin, float rightMax)
    {
        float leftSpan = leftMax - leftMin;
        float rightSpan = rightMax - rightMin;
        float valueScaled = (value - leftMin) / leftSpan;
        
        return rightMin + (valueScaled * rightSpan);
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
