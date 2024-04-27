using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public GameObject graphObject;
    private Graph graphScript;
    public void onPress(){
        if(!graphScript){
            Debug.Log("InputHandler.cs :: graphScript null");
            setGraph(graphObject);
        }

        Debug.Log("InputHandler.cs :: Update the visibility of a graph");
        graphScript.updateVisibility(
            graphScript.getVisibility() ? false : true);
    }

    public void setGraph(GameObject newGraph){
        Debug.Log("InputHandler.cs :: Change linked graph");
        graphObject = newGraph;
        graphScript = newGraph.GetComponent<Graph>();
    }
}
