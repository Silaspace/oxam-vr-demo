using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public string inputFile;

    public GameObject plotContainer;

    public void onPress(){
        Debug.Log("InputHandler.cs :: " + inputFile);
        if (plotContainer.TryGetComponent(out isPlotter plotter))
        {
            plotter.plot(inputFile);
        }
        else
        {
            Debug.Log("InputHandler.cs :: Can't find plot function");
        }
    }
}