using UnityEngine;
using System.Collections;

public class ScatterPlot2 : MonoBehaviour
{

    // Name of the input file, no extension
    public string inputfile;

    // Particle system
	ParticleSystem.Particle[] cloud;
	bool bPointsUpdated = false;
	
	void Start ()
	{
	}
	
	void Update () 
	{
		if (bPointsUpdated)
		{
            Debug.Log("ScatterPlot2.cs :: Update");
			GetComponent<ParticleSystem>().SetParticles(cloud, cloud.Length);
			bPointsUpdated = false;
            Debug.Log("ScatterPlot2.cs :: Done");
		}
	}
	
	public void SetPoints()
	{
        // Get vectorList to plot
        Debug.Log("ScatterPlot2.cs :: Fetch Data");
        var positions = GetData.fetch(inputfile);

        // Particle system built
        Debug.Log("ScatterPlot2.cs :: Set Particles");
		cloud = new ParticleSystem.Particle[positions.Count];
		
		for (int i = 0; i < positions.Count; ++i)
		{
            // Scale data to room size
            var pos = (positions[i] * 15);

			cloud[i].position = pos;			
			cloud[i].startColor = Color.red;
			cloud[i].startSize = 2f;
		}

		bPointsUpdated = true;
	}
}