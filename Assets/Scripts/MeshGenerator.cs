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
    private Color[] colours;
    private float yMin;
    private float yMax;
    private float xMax;
    private float xMin;
    private float zMax;
    private float zMin;

    ParticleSystem.Particle[] cloud;
    bool pointsUpdated = false;

    public void update(List<Vector3> positions, List<string> labels)
	{
        vertices = positions.ToArray();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        for (int i = 0; i < vertices.Length; i++){
            var y = vertices[i].y;
            yMax = Math.Max(yMax, y);
            yMin = Math.Min(yMin, y);
        }

        Debug.Log("Setting points...");

        cloud = new ParticleSystem.Particle[positions.Count];

        for (int i = 0; i < positions.Count; ++i)
        {
            Vector3 pos = positions[i];
            cloud[i].position = new Vector3(pos.x, pos.y + 0.005f, pos.z);
            cloud[i].startSize = 0.01f;
            cloud[i].startColor = Color.white;
        }

        pointsUpdated = true;

        triangulation();
        updateMesh();
        updatePoints();
	}

    private void updateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colours;
        mesh.RecalculateNormals();
    }

    private void updatePoints()
    {
        if (pointsUpdated) {
            Debug.Log("updating points...");
            GetComponent<ParticleSystem>().SetParticles(cloud, cloud.Length);
            pointsUpdated = false;
        }       
    }

    private void triangulation()
    {
        //turn vertices into float2s for triangulation
        float2[] points = new float2[vertices.Length];
        for (int p = 0; p < vertices.Length; p++)
        {
            points[p] = new(vertices[p].x, vertices[p].z);
        }

        //run triangulation, currently no refinement mesh
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

        // Render the mesh

        NativeArray<int> nativeOutputTriangles = triangulator.Output.Triangles.AsArray(); //create the mesh
        int triLength = nativeOutputTriangles.Length;
        int[] outputTriangles = new int[triLength]; //correct type of array for triangles
        nativeOutputTriangles.CopyTo(outputTriangles); //get the correct type for the triangles

        triangles = new int[triLength*2]; //this is used to render the mesh
        Array.Copy(outputTriangles, 0, triangles, 0, triLength); // copy original triangles to front of final array

        //create new vertices that are slightly below the old ones to avoid shader bug
        int vertLength = vertices.Length;
        Vector3[] tempVertices = vertices;
        vertices = new Vector3[vertLength * 2];
        Array.Copy(tempVertices, 0, vertices, 0, vertLength);

        //also, add the colours to each vertex at the same time
        //create colours for gradient on mesh
        colours = new Color[vertLength*2];

        Debug.Log("MeshGenerator.cs :: yMax: " + yMax + ", yMin: " + yMin);

        for (int v = 0; v < vertLength; v +=1)
        {
            //create new vertex
            vertices[vertLength + v] = new Vector3(vertices[v].x, vertices[v].y - 0.01f, vertices[v].z);
            //assign colours to both vertices
            colours[v] = CustomGradient.GetColor(vertices[v].y, yMin, yMax, "magma");
            colours[vertLength + v] = colours[v];
        }

        // Add reversed triangles for the second side
        for (int t = 0; t < triLength; t += 3)
        {
            // reverse vertex order, offset to be vertices on underside of mesh
            triangles[triLength + t] = outputTriangles[t + 2] + vertLength; 
            triangles[triLength + t + 1] = outputTriangles[t + 1] + vertLength;
            triangles[triLength + t + 2] = outputTriangles[t] + vertLength;
        }
    }
}
