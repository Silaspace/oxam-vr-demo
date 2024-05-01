using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using andywiecko.BurstTriangulator;
using Unity.Collections;
using Unity.Mathematics;
using System.Linq;

public class MeshGenerator : MonoBehaviour, GraphRenderer
{
    // Private properties
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private Color[] colors;
    private ScatterPlot scatter;

    void Start () 
	{
        var meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;

        scatter = GetComponent<ScatterPlot>();
    }

    public void update(Graph graphData)
	{
        // Invoke scatter graph
        scatter.update(graphData);

        // Clear mesh and particle system
        Debug.Log("MeshGenerator.cs :: Clear Mesh");
        mesh.Clear();

        // Don't render if hidden
        if (!graphData.getVisibility())
        {
            return;
        }

        // Get appropriate data from graph object
        Debug.Log("MeshGenerator.cs :: Get graph data");
        vertices = graphData.getVectorList().ToArray();
        colors = graphData.getColors().ToArray();

        // Triangulate Mesh
        Debug.Log("MeshGenerator.cs :: Triangulate mesh");

        //triangulate for each pair of indices
        
        List<List<int>> triangleList = new List<List<int>>();
        List<(int, int)> indices = graphData.getIndicesList();
        foreach((int, int) tuple in indices)
        {
            int left = tuple.Item1;
            int right = tuple.Item2;
            Debug.Log("MeshGenerator.cs :: Triangulate indices " + left + " to " + right);
            //triangulate the points [left, right)
            int size = right - left;
            int offset = left;
            Vector3[] someVertices = new Vector3[size];
            Array.Copy(vertices, left, someVertices, 0, size);
            triangleList.Add(triangulate(someVertices.ToList(), offset));
        }

        triangles = triangleList.SelectMany(x => x).ToArray();
        //triangles = triangulate(vertices.ToList(), 0).ToArray();
        
        //  Update
        Debug.Log("MeshGenerator.cs :: Update");
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
	}

    private List<int> triangulate(List<Vector3> someVertices, int offset)
    {
        // Turn vertices into float2s for triangulation
        float2[] points = new float2[someVertices.Count];
        for (int p = 0; p < someVertices.Count; p++)
        {
            points[p] = new(someVertices[p].x, someVertices[p].z);
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

        return outputTriangles.Select(x => x + offset).ToList();
    }
}
