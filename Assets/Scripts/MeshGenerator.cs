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
        triangles = outputTriangles;
        
        //  Update
        Debug.Log("MeshGenerator.cs :: Update");
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
	}
}
