using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class FileDropdown : MonoBehaviour
{
    // Create the options for the input file dropdown

    private List<Dictionary<string, object>> metaData;
    private TMP_Dropdown dropdown;

    void Start()
    {
	dropdown = GetComponent<TMP_Dropdown>();
    }
    
    public void updateOptions()
    {
	Debug.Log("FileDropdown.cs :: Get metadata of the datasets available");
	metaData = CSVReader.Read("Data/_data");
	
        Debug.Log("FileDropdown.cs :: Extract name,path data");
	List<string> options = new List<string>();
        for (var i = 0; i < metaData.Count; i++)
        {
		options.Add(
			Convert.ToString(metaData[i]["#=Name"]));
        }

	Debug.Log("FileDropdown.cs :: Update dropdown");

	if (dropdown == null)
	{
		Debug.Log("FileDropdown.cs :: Dropdown wasn't returned");
	}
	else
	{
		dropdown.ClearOptions();
		dropdown.AddOptions(options);
	}
    }

    public string getFileName()
    {
	int index = dropdown.value;
	if(index >= metaData.Count)
	{
		return null;
	}
	else
	{
		return Convert.ToString(metaData[dropdown.value]["Path"]); 
	}
    }

    public void setIndexFromFilename(string fileName)
    {
		int index = metaData.FindIndex(file => (string)file["Path"] == fileName);

		if(index != -1)
		{
			Debug.Log("FileDropdown.cs :: File option exists: setting dropdown to it, index is " + index);
			dropdown.value = index;
		}
		else
		{
			Debug.Log("FileDropdown.cs :: File option doesn't exist: creating new option");
			dropdown.AddOptions(new List<string> {"Deleted File"});
			dropdown.value = dropdown.options.Count - 1;
		}
    }
    
}
