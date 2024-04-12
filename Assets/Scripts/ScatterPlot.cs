using UnityEngine;
using System.Collections;

public class ScatterPlot : MonoBehaviour, isPlotter
{

    // Particle system
	ParticleSystem.Particle[] cloud;
	bool pointsUpdated = false;

    // Dictionary storing different colours for different labels
    private Dictionary<string, Color> labelColorMap;

    // Predefined colours for labels, which are used before falling back to random colours for points
    private List<Color> predefinedColors = new List<Color> {
        Color.red, Color.blue, Color.green, Color.yellow, Color.magenta
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
	
	public void plot(string inputFile)
	{
        // Get vectorList and labels to plot
        Debug.Log("ScatterPlot.cs :: Fetch Data");
        (List<Vector3> positions, List<string> labels) = GetData.fetch(inputFile);

        // Assign unique colors to each label
        AssignLabelColors(labels);

        // Particle system built
        Debug.Log("ScatterPlot.cs :: Set Particles");
		cloud = new ParticleSystem.Particle[positions.Count];
		
		for (int i = 0; i < positions.Count; ++i)
		{
			cloud[i].position = scale(positions[i]);			
			cloud[i].startSize = 2f;

            if (labels != null && labels.Count > i) {
                cloud[i].startColor = labelColorMap[labels[i]];
            }
            else {
                cloud[i].startColor = Color.red;
            }
		}

		pointsUpdated = true;
	}

	private Vector3 scale(Vector3 position)
	{
		return (position * 25) + new Vector3(-10, 0, 0);
	}

    private void AssignLabelColors(List<string> labels)
    {
        if (labels == null)
        {
            return;
        }

        foreach (string label in labels)
        {
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