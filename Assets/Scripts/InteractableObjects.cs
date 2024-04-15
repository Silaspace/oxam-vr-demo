using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//isTrigger should be checked on the sphere collider of dataBall,
// and the mesh renderer unchecked

public class InteractableObjects : MonoBehaviour, GraphRenderer
{
    public GameObject pointPrefab;
    public GameObject pointContainer;

    public void update(List<Vector3> positions, List<string> labels)
	{
        // Loop through Pointlist
        Debug.Log("InteractableObjects.cs :: Plot Data");
        for (var i = 0; i < positions.Count; i++)
        {
            // Create new game object
            GameObject newPoint = Instantiate(
                    pointPrefab, 
                    scale(positions[i]), 
                    Quaternion.identity);

            // Assigns name and parent to new object
            newPoint.transform.name = labels[i];
            newPoint.transform.parent = pointContainer.transform;
        }
	}

    private Vector3 scale(Vector3 position)
	{
		return (position * 40) + new Vector3(-10, 0, 0);
	}
}