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
        return (float) dt.Minute + 0.166666f*(dt.Second/10);
    }

    public static (List<Vector3>, List<(int, int)>, List<string>, List<string>) process(List<Dictionary<string, object>> pointList)
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

        //temporary, used to shift the asks and amplify the valley
        SortedDictionary<float, float> maxBids = new SortedDictionary<float, float>();
        SortedDictionary<float, float> minAsks = new SortedDictionary<float, float>();

        // Get each entry from the points list
        // Maintain a dict for prices
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

            //temporary, used for shifting valley
            if (minAsks.ContainsKey(entry.AskTime))
            {
                minAsks[entry.AskTime] = Math.Min(minAsks[entry.AskTime], entry.AskPrice);
            } else 
            {
                minAsks.Add(entry.AskTime, entry.AskPrice);
            }
            if (maxBids.ContainsKey(entry.BidTime))
            {
                maxBids[entry.BidTime] = Math.Max(maxBids[entry.BidTime], entry.BidPrice);
            } else 
            {
                maxBids.Add(entry.BidTime, entry.BidPrice);
            }

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

        //temp valley shift code
        float maxShift = float.MinValue;
        foreach(var pair in minAsks.Zip(maxBids, Tuple.Create))
        {
            maxShift = Math.Max(maxShift, pair.Item2.Value - pair.Item1.Value);
        }
        maxPrice = maxPrice + maxShift;

        //get a list of points to plot
        //for now we have:
        //x-axis: price
        //y-axis: size
        //z-axis: time
        List<Vector3> positions = new List<Vector3>();

        //we use [leftIndex, currentIndex) for each triangulation
        List<(int, int)> indices = new List<(int, int)>();
        List<string> labels = new List<string>();

        int startIndex = 0;
        int leftIndex = 0;
        int leftBorder = 0;
        int rightBorder = bids.Count;
        int currentBid = 0;

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
                labels.Add("bid");
                currentIndex += 1;
            }

            //add points to make a straight line on the left
            for (float i = lowPrice - 0.1f; i > minPrice; i -= 0.1f) 
            {
                currentIndex += 1;
                positions.Add(new Vector3(i, currentHeight, timePoint.Key));
                labels.Add("bid");
            }

            //if this isn't the time with the minimum price
            if (lowPrice != minPrice) 
            {
                currentIndex += 1;
                positions.Add(new Vector3(minPrice, currentHeight, timePoint.Key));
                labels.Add("bid");
            }

            //add index pair to indices
            if (currentBid > leftBorder && currentBid <= rightBorder-1) 
            {
                indices.Add((leftIndex, currentIndex));
                leftIndex = startIndex;
                labels.Add("bid");
            }
            startIndex = currentIndex;
            currentBid += 1;
        }
        
        leftIndex = startIndex;
        leftBorder = 0;
        rightBorder = asks.Count;
        currentBid = 0;
        //repeat for asks but don't reverse dictionary
        foreach(KeyValuePair<float, SortedDictionary<float, int>> timePoint in asks)
        {
            //temporary valley shift
            float valleyShift = maxBids[timePoint.Key] - minAsks[timePoint.Key];

            //for each price in increasing order (so it is cumulative)
            int currentHeight = 0;
            float highPrice = 0.0f;
            int currentIndex = startIndex;
            foreach(KeyValuePair<float, int> ask in timePoint.Value)
            {
                float xPos = ask.Key + valleyShift;
                highPrice = xPos;
                currentHeight += ask.Value; //increase currentHeight by the size of this bid
                float yPos = currentHeight;
                float zPos = timePoint.Key;

                positions.Add(new Vector3(xPos, yPos, zPos));
                labels.Add("ask");
                currentIndex += 1;
            }

            //add points to make a straight line on the right
            for (float i = highPrice + 0.1f; i < maxPrice; i+= 0.1f)
            {
                currentIndex += 1;
                positions.Add(new Vector3(i, currentHeight, timePoint.Key));
                labels.Add("ask");
            }

            //if this isn't the time with the maximum price
            if (highPrice != maxPrice) 
            {
                currentIndex += 1;
                positions.Add(new Vector3(maxPrice, currentHeight, timePoint.Key));
                labels.Add("ask");
            }

            //add index to indices
            if (currentBid > leftBorder && currentBid <= rightBorder-1)
            {
                indices.Add((leftIndex, currentIndex));
                leftIndex = startIndex;
            }
            currentBid += 1;
            startIndex = currentIndex;
        }

        // Return positions and the indices of the points we must use for each triangulation
        return (positions, indices, labels, new List<string>{"Price", "Size", "Time"});
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
