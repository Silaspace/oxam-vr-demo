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

        NativeArray<int> nativeOutputTriangles = triangulator.Output.Triangles.AsArray();
        int[] outputTriangles = new int[nativeOutputTriangles.Length];
        nativeOutputTriangles.CopyTo(outputTriangles);
        triangles = outputTriangles;

        // Render second side of the mesh
        
        int triLength = outputTriangles.Length;
        triangles = new int[triLength*2];
        Array.Copy(outputTriangles, 0, triangles, 0, triLength); // copy original triangles

        //create new vertices that are slightly below the old ones to avoid shader bug
        int vertLength = vertices.Length;
        Vector3[] tempVertices = vertices;
        vertices = new Vector3[vertLength * 2];
        Array.Copy(tempVertices, 0, vertices, 0, vertLength);
        for (int v = 0; v < vertLength; v +=1)
        {
            vertices[vertLength + v] = new Vector3(vertices[v].x, vertices[v].y - 0.0001f, vertices[v].z);
        }

        // Add reversed triangles for the second side
        for (int t = 0; t < triLength; t += 3)
        {
            // reverse vertex order, offset to be vertices on underside of mesh
            triangles[triLength + t] = outputTriangles[t + 2] + vertLength; 
            triangles[triLength + t + 1] = outputTriangles[t + 1] + vertLength;
            triangles[triLength + t + 2] = outputTriangles[t] + vertLength;
        }
        

        //create colours for gradient on mesh
        colours = new Color[vertices.Length];

        Debug.Log("MeshGenerator.cs :: yMax: " + yMax + ", yMin: " + yMin);

        for (int i = 0; i < vertices.Length; i++)
        {
	        colours[i] = CustomGradient.GetColor(vertices[i].y, yMin, yMax, "magma");
        }
    }
    
    /*
    void loadData(string inputfile) {

        (List<Vector3> vertexData, float xMin, float xMax, float yMin, float yMax, float zMin, float zMax) 
            = GetOrderbookData.ReadOrderbook();
        
        vertices = new Vector3[vertexData.Count];

        for (int i = 0; i < vertexData.Count; i++){
            float x = GetSharePriceData.MapRange(vertexData[i][0], xMin, xMax, 0, xSize);
            float y = GetSharePriceData.MapRange(vertexData[i][1], yMin, yMax, 0, ySize);
            float z = GetSharePriceData.MapRange(vertexData[i][2], zMin, zMax, 0, zSize);

            vertices[i] = new Vector3(x, y, z);

            yMax = Math.Max(yMax, y);
            yMin = Math.Min(yMin, y);
        } 
    }
    */
}
