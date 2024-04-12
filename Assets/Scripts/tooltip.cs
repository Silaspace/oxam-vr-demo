using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//isTrigger on the sphere collider on dataBall should be checked
public class tooltip : MonoBehaviour
{
    public string toolTipText = ""; // set this in the Inspector

    private string currentToolTipText = "";
    private GUIStyle guiStyleFore;// : \GUIStyle;
    private GUIStyle guiStyleBack;// : GUIStyle;

    // Start is called before the first frame update
    void Start()
    {
        guiStyleFore = new GUIStyle();
        guiStyleFore.normal.textColor = Color.white;  
        guiStyleFore.alignment = TextAnchor.UpperCenter ;
        guiStyleFore.wordWrap = true;
        guiStyleBack = new GUIStyle();
        guiStyleBack.normal.textColor = Color.black;  
        guiStyleBack.alignment = TextAnchor.UpperCenter ;
        guiStyleBack.wordWrap = true;
    }
    void OnMouseEnter ()
    {
        currentToolTipText = gameObject.name;
    }

    void OnMouseExit ()
    {
        currentToolTipText = "";
    }

    void OnGUI()
    {
        if (currentToolTipText != "")
        {
            var x = Event.current.mousePosition.x;
            var y = Event.current.mousePosition.y;
            GUI.Label (new Rect (x-149,y+21,300,60), currentToolTipText, guiStyleBack);
            GUI.Label (new Rect (x-150,y+20,300,60), currentToolTipText, guiStyleFore);
        }
    }

}
