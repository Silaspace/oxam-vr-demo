using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using andywiecko.BurstTriangulator;
using Unity.Collections;
using Unity.Mathematics;
public class MeshGenerator : MonoBehaviour, GraphRenderer
{
    // Private properties
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private Color[] colors;
    private ParticleSystem.Particle[] cloud;

    public void update(Graph graphData)
	{
        // Make a new mesh
        Debug.Log("MeshGenerator.cs :: Initialise Mesh");
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Update vertices
        vertices = graphData.getVectorList().ToArray();

        // Double the color array
        Debug.Log("MeshGenerator.cs :: Double color array");
        var colorArray = graphData.getColors().ToArray();
        colors = new Color[colorArray.Length * 2];
        colorArray.CopyTo(colors, 0);
        colorArray.CopyTo(colors, colorArray.Length);

        // Set up dots
        Debug.Log("MeshGenerator.cs :: Render datapoints");
        cloud = new ParticleSystem.Particle[vertices.Length];

        for (int i = 0; i < vertices.Length; ++i)
        {
            cloud[i].position = vertices[i];
            cloud[i].startSize = 0.01f;
            cloud[i].startColor = colors[i];
        }

        GetComponent<ParticleSystem>().SetParticles(cloud, cloud.Length);

        // Triangulate Mesh
        Debug.Log("MeshGenerator.cs :: Triangulate mesh");
        triangulation();
        
        // Update the mesh
        Debug.Log("MeshGenerator.cs :: Update mesh variables");
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
	}

    private void triangulation()
    {
        // Turn vertices into float2s for triangulation
        float2[] points = new float2[vertices.Length];
        for (int p = 0; p < vertices.Length; p++)
        {
            points[p] = new(vertices[p].x, vertices[p].z);
        }

        // Run Triangulation
        using var positions = new NativeArray<float2>(points, Allocator.Persistent);
        using var triangulator = new Triangulator(capacity: 1024, Allocator.Persistent)
        {
            Input = { Positions = positions },
            //Settings = {
            //    RefineMesh = true,
            //    RefinementThresholds = {
            //        Area = 1f,
            //        Angle = math.radians(20f)
            //    },
            //}
        };

        triangulator.Run();


        // Process output of the trianglator
        NativeArray<int> nativeOutputTriangles = triangulator.Output.Triangles.AsArray();
        int triLength = nativeOutputTriangles.Length;
        int[] outputTriangles = new int[triLength];
        nativeOutputTriangles.CopyTo(outputTriangles);

        // Create new vertices slightly below
        int vertLength = vertices.Length;
        Vector3[] tempVertices = vertices;
        vertices = new Vector3[vertLength * 2];
        Array.Copy(tempVertices, 0, vertices, 0, vertLength);

        for (int v = 0; v < vertLength; v +=1)
        {
            vertices[vertLength + v] = new Vector3(vertices[v].x, vertices[v].y - 0.01f, vertices[v].z);
        }

        // Add reversed triangles for the second side
        triangles = new int[triLength*2];
        Array.Copy(outputTriangles, 0, triangles, 0, triLength);

        for (int t = 0; t < triLength; t += 3)
        {
            // Reverse vertex order
            triangles[triLength + t] = outputTriangles[t + 2] + vertLength; 
            triangles[triLength + t + 1] = outputTriangles[t + 1] + vertLength;
            triangles[triLength + t + 2] = outputTriangles[t] + vertLength;
        }
    }
}
