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
		var scale = graphData.getScale();

		// Calculate a single scale factor
		var scaleFactor = calculateScaleFactor(scale);

        // Particle system built
        Debug.Log("ScatterPlot.cs :: Set Particles");
		cloud = new ParticleSystem.Particle[positions.Count];

		if (visibility)
		{
			for (int i = 0; i < positions.Count; ++i)
			{
				cloud[i].position = positions[i];			
				cloud[i].startSize = scaleFactor;
				cloud[i].startColor = colors[i];
			}
		}

		pointsUpdated = true;
	}

	// Approximate scaling
	// sf:               16     8      4     2      1     1/2    1/4   1/8
	// log(sf):          4      3      2     1      0    -1     -2    -3
	// log(sf)+4:        8      7      6     5      4     3      2     1
	// 0.05(log(sf)+4):  0.04   0.035  0.03  0.025  0.02  0.015  0.01  0.005
	// return            0.04   0.035  0.03  0.025  0.02  0.015  0.01  0.005
	private float calculateScaleFactor(Vector3 scale)
	{
		var sf = scale.x * scale.y * scale.z;

		if (sf <= 1/8)
		{
			return 0.005f;
		}
		if (sf >= 16)
		{
			return 0.04f;
		}

		return (Mathf.Log(sf, 2) + 4) * 0.005f;
	}
}