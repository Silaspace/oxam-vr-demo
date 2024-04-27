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

        // Get mesh colors
        Debug.Log("MeshGenerator.cs :: Double color array");
        colors = graphData.getColors().ToArray();

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

        //triangulate for each pair of indices
        List<List<int>> triangleList = new List<List<int>>();
        var indices = graphData.getIndicesList().ToArray();
        foreach((int, int) (left, right) in indices)
        {
            //triangulate the points [left, right)
            int size = right - left;
            int offset = left;
            Vector3[] someVertices = new int[size];
            Array.Copy(vertices, left, someVertices, 0, size);
            triangleList.Add(triangulate(someVertices, offset));
        }

        triangles = triangleList.SelectMany(x => x).ToList();
        
        // Update the mesh
        Debug.Log("MeshGenerator.cs :: Update mesh variables");
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
	}

    private int[] triangulate(Vector3[] vertices, int offset)
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
        };

        triangulator.Run();

        // Process output of the trianglator
        NativeArray<int> nativeOutputTriangles = triangulator.Output.Triangles.AsArray();
        int triLength = nativeOutputTriangles.Length;
        int[] outputTriangles = new int[triLength];
        nativeOutputTriangles.CopyTo(outputTriangles);
        return outputTriangles;
    }
}
