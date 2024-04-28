using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class FileDropdown : MonoBehaviour
{
    // Create the options for the input file dropdown

    public void updateOptions(){
	Debug.Log("FileDropdown.cs :: Making Options");
	List<string> options = new List<string>();
	string[] directories = Directory.GetDirectories("Assets/Resources/Data","*");
	foreach (string directory in directories) {
	    string[] files = Directory.GetFiles(directory, "*.csv");
	    foreach (string file in files) {
		options.Add(file.Substring(22));
	    }
	}
	TMP_Dropdown dropdown = GetComponent<TMP_Dropdown>();
	if (dropdown == null) {
	    Debug.Log("FileDropdown.cs :: Dropdown wasn't returned");
	}
	dropdown.ClearOptions();
	dropdown.AddOptions(options);
	Debug.Log("FileDropdown.cs :: Options Made");
	
    }
    
}
