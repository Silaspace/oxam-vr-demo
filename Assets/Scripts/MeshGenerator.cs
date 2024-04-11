using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using andywiecko.BurstTriangulator;
using Unity.Collections;
using Unity.Mathematics;
public class MeshGenerator : MonoBehaviour
{

    // code from Brackeys video https://www.youtube.com/watch?v=64NblGkAabk&ab_channel=Brackeys

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    Color[] colours;

    public int xSize = 2;
    public int zSize = 2;

    public Gradient gradient;

    float yMin;
    float yMax;
    float xMax;
    float xMin;
    float zMax;
    float zMin;

    void Start()
    {
        //make mesh
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        //CreateMesh();
        //UpdateMesh();

        Triangulation();
    }


    void CreateMesh()
    {
        vertices = new Vector3[(xSize+1) * (zSize+1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * .3f, z * .3f) * 2f; //change this to be heights
                vertices[i] = new Vector3(x, y, z);

                xMax = Math.Max(xMax, x);
                xMin = Math.Min(xMin, x);
                yMax = Math.Max(yMax, y);
                yMin = Math.Min(yMin, y);
                zMax = Math.Max(zMax, z);
                zMin = Math.Min(zMin, y);

                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];
        // must make points in clockwise order

        int vert = 0;
        int tris = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris +=6;
            }
            vert++;
        }

        //create colours for gradient on mesh
        //TODO currently doesn't work because no rendering pipeline
        colours = new Color[vertices.Length];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x<= xSize; x++)
            {
                float height = Mathf.InverseLerp(yMin, yMax, vertices[i].y);
                colours[i] = gradient.Evaluate(height);
                i++;
            }
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        //mesh.colors = colours;

        mesh.RecalculateNormals();
    }

    //remove this function to remove dots from mesh
    private void OnDrawGizmos()
    {
        if (vertices == null)
            return;
        
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], .1f);
        }
    }

    void Triangulation()
    {
        //vertices are normally global variable, assigned here to random points for testing
        int len = 100;
        vertices = new Vector3[len];
        System.Random rnd = new System.Random();
        for (int i = 0; i < len; i++){
            float x = (float)rnd.NextDouble()*2;
            float z = (float)rnd.NextDouble()*2;
            float y = Mathf.PerlinNoise(x * .3f, z * .3f); //change this to be heights
            vertices[i] = new Vector3(x,y,z);
        }
    
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

        //convert to correct type for output
        var outputTriangles = triangulator.Output.Triangles;
        int[] finalTriangles = new int[outputTriangles.Length];
        for (int t = 0; t<finalTriangles.Length; t++)
        {
            finalTriangles[t] = outputTriangles[t];
        }

        //update mesh
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = finalTriangles;

        mesh.RecalculateNormals();
    }
}