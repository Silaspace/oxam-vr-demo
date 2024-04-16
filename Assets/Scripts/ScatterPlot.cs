using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScatterPlot : MonoBehaviour, GraphRenderer
{

    // Particle system
	ParticleSystem.Particle[] cloud;
	bool pointsUpdated = false;

    // Dictionary storing different colours for different labels
    private Dictionary<string, Color> labelColorMap;

    // Predefined colours for labels, which are used before falling back to random colours for points
    // From here:  https://jfly.uni-koeln.de/color/
    private List<Color> predefinedColors = new List<Color> {
        new Color32(0xE6, 0x9F, 0x00, 0xFF), // orange
        new Color32(0x56, 0xB4, 0xE9, 0xFF), // skyblue
        new Color32(0x00, 0x9E, 0x73, 0xFF), // bluishgreen
        new Color32(0xF0, 0xE4, 0x42, 0xFF), // yellow
        new Color32(0x00, 0x72, 0xB2, 0xFF), // blue
        new Color32(0xD5, 0x5E, 0x00, 0xFF), // vermillion
        new Color32(0xCC, 0x79, 0xA7, 0xFF), // reddishpurple
        new Color32(0x99, 0x99, 0x99, 0xFF)  // gray
    };

    private int nextColorIndex = 0;
	
    void Start()
    {
        labelColorMap = new Dictionary<string, Color>();
    }

	void Update () 
	{
		if (pointsUpdated)
		{
            Debug.Log("ScatterPlot.cs :: Update");
			GetComponent<ParticleSystem>().SetParticles(cloud, cloud.Length);
			pointsUpdated = false;
		}
	}
	
	public void update(List<Vector3> positions, List<string> labels)
	{
        // Assign label colors
        Debug.Log("ScatterPlot.cs :: Assign label colors");
        AssignLabelColors(labels);

        // Particle system built
        Debug.Log("ScatterPlot.cs :: Set Particles");
		cloud = new ParticleSystem.Particle[positions.Count];
		
		for (int i = 0; i < positions.Count; ++i)
		{
			cloud[i].position = positions[i];			
			cloud[i].startSize = 0.5f;

            if (labels != null && labels.Count > i) {
                cloud[i].startColor = labelColorMap[labels[i]];
            }
            else {
                cloud[i].startColor = Color.red;
            }
		}

		pointsUpdated = true;
	}

    private void AssignLabelColors(List<string> labels)
    {
        if (labels == null)
        {
            return;
        }

        foreach (string label in labels)
        {
            Debug.Log(labelColorMap);
            if (!labelColorMap.ContainsKey(label))
            {
                labelColorMap.Add(label, GetUniqueColor());
            }
        }
    }

    private Color GetUniqueColor()
    {
        if (nextColorIndex < predefinedColors.Count)
        {
            // If we can still use predefined colours, use one and increment the index
            nextColorIndex++;
            return predefinedColors[nextColorIndex - 1];
        }

        // Otherwise pick a random color
        return new Color(
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f)
        );
    }
}