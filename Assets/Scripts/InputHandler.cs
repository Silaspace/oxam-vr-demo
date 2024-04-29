using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public GameObject graphObject;
    public PageScroll scrollManager;
    public Settings settings;
    private Graph graphScript;
    public void onPress(){
        if(!graphScript){
            Debug.Log("InputHandler.cs :: graphScript null");
            setGraph(graphObject);
        }

        // Debug.Log("InputHandler.cs :: Update the visibility of a graph");
        // graphScript.updateVisibility(
        //    graphScript.getVisibility() ? false : true);

        Debug.Log("InputHandler.cs :: Changing the currently selected graph");
        settings.changeGraph(gameObject, graphObject);

        Debug.Log("InputHandler.cs :: Change the page to show settings");
        scrollManager.SetPageIndex(1);
    }

    public void setGraph(GameObject newGraph){
        Debug.Log("InputHandler.cs :: Change linked graph");
        graphObject = newGraph;
        graphScript = newGraph.GetComponent<Graph>();
    }
}
