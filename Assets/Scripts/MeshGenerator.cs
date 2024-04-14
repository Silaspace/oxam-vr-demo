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

    public int xSize = 20;
    public int ySize = 10;
    public int zSize = 20;

    // Try with "Data/RadcliffeTemperature/temp" or "Data/RoyalMailSharePrice/price"
    public string dataset = "Data/RoyalMailSharePrice/price";

    // Use every `takeEvery` value in the dataset, e.g. if 1 uses every value, if 10 uses every 10th value
    public int takeEvery = 10;

    public Gradient gradient;

    public bool renderBothSides;

    float yMin;
    float yMax;
    float xMax;
    float xMin;
    float zMax;
    float zMin;

    void Start()
    {
        yMax = Int32.MinValue;
        yMin = Int32.MaxValue;

        LoadData(dataset);

        //make mesh
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        //CreateMesh();
        //UpdateMesh();

        Triangulation();
    }

    //old code for even meshes
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

    }

    void UpdateMesh()
    {
        //update mesh
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colours;

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
        // int len = 100;
        // float yMax = Int32.MinValue;
        // float yMin = Int32.MaxValue;
        // vertices = new Vector3[len];
        // System.Random rnd = new System.Random();
        // for (int i = 0; i < len; i++){
            // float x = (float)rnd.NextDouble()*20;
            // float z = (float)rnd.NextDouble()*20;
            // float y = Mathf.PerlinNoise(x * .3f, z * .3f) * 2f; //change this to be heights
            // vertices[i] = new Vector3(x,y,z);

            // yMax = Math.Max(yMax, y);
            // yMin = Math.Min(yMin, y);
        // }
    
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

        //if we want to render both sides of the mesh
        if (renderBothSides)
        {
            int triLength = outputTriangles.Length;
            triangles = new int[triLength*2];
            for (int t = 0; t < triLength / 3; t++)
            {
                triangles[t] = outputTriangles[t];
                triangles[t+1] = outputTriangles[t+1];
                triangles[t+2] = outputTriangles[t+2];
                triangles[triLength+t] = outputTriangles[t+2];
                triangles[triLength+t+1] = outputTriangles[t+1];
                triangles[triLength+t+2] = outputTriangles[t];
            }
        } else 
        {
            triangles = new int[outputTriangles.Length];
            for (int t = 0; t < triangles.Length; t++)
            {
                triangles[t] = outputTriangles[t];
            }
        }



        //create colours for gradient on mesh
        //TODO currently doesn't work because no rendering pipeline
        colours = new Color[vertices.Length];

        Debug.Log("MeshGenerator.cs :: yMax: " + yMax + ", yMin: " + yMin);

        for (int i = 0; i < vertices.Length; i++)
        {
            float height = Mathf.InverseLerp(yMin, yMax, vertices[i].y);
            colours[i] = gradient.Evaluate(height);
        }

        UpdateMesh();
    }

    void LoadData(string inputfile) {
        float repeatPeriod = (float)(60.0 * 60.0 * 24.0 * 365.25);

        (List<Vector3> vertexData, float timeShortMin, float timeShortMax, float timeLongMin, float timeLongMax, float priceMin, float priceMax) 
            = GetSharePriceData.fetch(inputfile, repeatPeriod, takeEvery);
        
        vertices = new Vector3[vertexData.Count];

        for (int i = 0; i < vertexData.Count; i++){
            // vertexData is (TimeShort, TimeLong, Price)
            // But need to plot points as
            // x: TimeShort (i.e. a one year span)
            // y: Price
            // z: TimeLong (i.e. each year)

            float x = GetSharePriceData.MapRange(vertexData[i][0], timeShortMin, timeShortMax, 0, xSize);
            float y = GetSharePriceData.MapRange(vertexData[i][2], priceMin, priceMax, 0, ySize);
            float z = GetSharePriceData.MapRange(vertexData[i][1], timeLongMin, timeLongMax, 0, zSize);

            vertices[i] = new Vector3(x, y, z);

            yMax = Math.Max(yMax, y);
            yMin = Math.Min(yMin, y);
        }
    }
}
