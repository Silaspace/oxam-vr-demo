using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class FileDropdown : MonoBehaviour
{
    // Create the options for the input file dropdown

    public void updateOptions()
	{
		Debug.Log("FileDropdown.cs :: Get metadata of the datasets available");
		var metaData = CSVReader.Read("Data/_data");

        // Loop through pointList extract the data
        Debug.Log("FileDropdown.cs :: Extract name,path data");
		List<string> options = new List<string>();

        for (var i = 0; i < pointList.Count; i++)
        {   
			options.Add(pointList[i]["Path"]);
        }

		Debug.Log("FileDropdown.cs :: Update dropdown");
		TMP_Dropdown dropdown = GetComponent<TMP_Dropdown>();

		if (dropdown == null) {
			Debug.Log("FileDropdown.cs :: Dropdown wasn't returned");
		}

		dropdown.ClearOptions();
		dropdown.AddOptions(options);
    }
    
}
