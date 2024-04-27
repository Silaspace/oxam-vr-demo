using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScatterPlot : MonoBehaviour, GraphRenderer
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
	
	public void update(Graph graphData)
	{
		// Get appropriate data from graph object
		var labels = graphData.getLabels();
		var positions = graphData.getVectorList();
		var colors = graphData.getColors();

        // Particle system built
        Debug.Log("ScatterPlot.cs :: Set Particles");
		cloud = new ParticleSystem.Particle[positions.Count];
		
		for (int i = 0; i < positions.Count; ++i)
		{
			cloud[i].position = positions[i];			
			cloud[i].startSize = 0.5f;
			cloud[i].startColor = colors[i];
		}

		pointsUpdated = true;
	}
}