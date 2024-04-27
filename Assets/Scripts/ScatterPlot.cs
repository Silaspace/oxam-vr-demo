using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScatterPlot : MonoBehaviour, GraphRenderer
{
    // Particle system
	private ParticleSystem.Particle[] cloud;
	private ParticleSystem ps;
	private bool pointsUpdated = false;
	
	void Start () 
	{
		ps = GetComponent<ParticleSystem>();
	}

	void Update () 
	{
		if (pointsUpdated)
		{
            Debug.Log("ScatterPlot.cs :: Update");
			ps.SetParticles(cloud, cloud.Length);
			pointsUpdated = false;
		}
	}
	
	public void update(Graph graphData)
	{
		// Get appropriate data from graph object
		var labels = graphData.getLabels();
		var positions = graphData.getVectorList();
		var colors = graphData.getColors();
		var visibility = graphData.getVisibility();

        // Particle system built
        Debug.Log("ScatterPlot.cs :: Set Particles");
		cloud = new ParticleSystem.Particle[positions.Count];

		if (visibility)
		{
			for (int i = 0; i < positions.Count; ++i)
			{
				cloud[i].position = positions[i];			
				cloud[i].startSize = 0.05f;
				cloud[i].startColor = colors[i];
			}
		}

		pointsUpdated = true;
	}
}