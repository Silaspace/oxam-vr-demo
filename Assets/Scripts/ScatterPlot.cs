using UnityEngine;
using System.Collections;

public class ScatterPlot : MonoBehaviour, isPlotter
{

    // Particle system
	ParticleSystem.Particle[] cloud;
	bool pointsUpdated = false;
	
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
        // Get vectorList to plot
        Debug.Log("ScatterPlot.cs :: Fetch Data");
        var positions = GetData.fetch(inputFile);

        // Particle system built
        Debug.Log("ScatterPlot.cs :: Set Particles");
		cloud = new ParticleSystem.Particle[positions.Count];
		
		for (int i = 0; i < positions.Count; ++i)
		{
			cloud[i].position = scale(positions[i]);			
			cloud[i].startColor = Color.red;
			cloud[i].startSize = 2f;
		}

		pointsUpdated = true;
	}

	private Vector3 scale(Vector3 position)
	{
		return (position * 25) + new Vector3(-10, 0, 0);
	}
}