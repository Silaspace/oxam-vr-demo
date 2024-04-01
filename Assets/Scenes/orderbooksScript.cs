using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UnityEngine;

public class orderbooksScript : MonoBehaviour
{
    //define public vars to be changed in inspector
    public GameObject pointPrefab;
    public float scale;
    public Material pointMaterial;

    private float processDateTime(DateTime dt)
    {
        return (float)0.0; //fix
    }



    // Start is called before the first frame update
    void Start()
    {
        // Read the contents of the CSV files as individual lines
        //TODO figure out relative file locations in c#
        string[] csvLines = File.ReadAllLines(@"C:\Users\jimmy\OneDrive - Nexus365\Work\Group Practical\oxam-vr-demo\Assets\Data\orderbooks_appl.csv");
        var entries = new List<Entry>();
        //dictionary of (price, time) to count
        Dictionary<(float, DateTime), int> points = new Dictionary<(float, DateTime), int>();

        // Get each entry from csv. Placed in a list for now, maybe unnecessary
        // Also maintain a dict for prices
        for (int i = 1; i < csvLines.Length; i++)
        {
            string[] rowData = csvLines[i].Split(',');

            //Create entry for list (maybe not used)
            var entry = new Entry
            {
                Quote = rowData[0],
                Symbol = rowData[1],
                EventTime = Convert.ToDateTime(rowData[2]),
                BidTime = Convert.ToDateTime(rowData[3]),
                BidCode = Convert.ToChar(rowData[4]),
                BidPrice = Convert.ToSingle(rowData[5]),
                BidSize = Convert.ToInt16(rowData[6]),
                AskTime = Convert.ToDateTime(rowData[7]),
                AskCode = Convert.ToChar(rowData[8]),
                AskPrice = Convert.ToSingle(rowData[9]),
                AskSize = Convert.ToInt16(rowData[10])
            };
            entries.Add(entry);

            //add point to dict
            if (points.ContainsKey((entry.BidPrice, entry.BidTime))){
                points[(entry.BidPrice, entry.BidTime)] += 1;
            } else {
                points.Add((entry.BidPrice, entry.BidTime), 1);
            }

        }

        //get a list of points to plot
        //for now we have:
        //x-axis: price of buy orders
        //y-axis: time
        //z-axis: number of buy orders
        //var positions = new List<Vector3>();
        foreach(KeyValuePair<(float, DateTime), int> point in points)
        {
            float xPos = point.Key.Item1;
            float yPos = processDateTime(point.Key.Item2);
            int zPos = point.Value;

            var p = Instantiate(pointPrefab, transform);
            p.transform.localPosition = new Vector3(xPos, yPos, zPos);
            p.GetComponent<MeshRenderer>().material = pointMaterial;
            //positions.Add(new Vector3(xPos, yPos, zPos));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

// All of the data stored in the csv file orderbooks_appl.
// Could be improved for other orderbooks data
public class Entry
{
    public string Quote {get; set;}
    public string Symbol {get; set;}
    public DateTime EventTime {get; set;}
    public DateTime BidTime {get; set;}
    public char BidCode {get; set;}
    public float BidPrice {get; set;}
    public int BidSize {get; set;}
    public DateTime AskTime	{get; set;}
    public char AskCode	{get; set;}
    public float AskPrice {get; set;}
    public int AskSize {get; set;}
}
