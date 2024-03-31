using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class orderbooksScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Read the contents of the CSV files as individual lines
        //TODO figure out relative file locations in c#
        string[] csvLines = File.ReadAllLines(@"file location");
        var entries = new List<Entry>();

        // Get each entry from csv
        for (int i = 1; i < csvLines.Length; i++)
        {
            string[] rowData = csvLines[i].Split(',');
            var entry = new Entry
            {
                Quote = rowData[0],
                Symbol = rowData[1],
                EventTime = Convert.ToDateTime(rowData[2]),
                BidTime = Convert.ToDateTime(rowData[3]),
                BidCode = Convert.ToChar(rowData[4]),
                BidPrice = Convert.ToDouble(rowData[5]),
                BidSize = Convert.ToInt16(rowData[6]),
                AskTime = Convert.ToDateTime(rowData[7]),
                AskCode = Convert.ToChar(rowData[8]),
                AskPrice = Convert.ToDouble(rowData[9]),
                AskSize = Convert.ToInt16(rowData[10])
            };
            entries.Add(entry);
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
    public double BidPrice {get; set;}
    public int BidSize {get; set;}
    public DateTime AskTime	{get; set;}
    public char AskCode	{get; set;}
    public double AskPrice {get; set;}
    public int AskSize {get; set;}
}
