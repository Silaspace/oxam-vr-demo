using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using andywiecko.BurstTriangulator;
using Unity.Collections;
using Unity.Mathematics;
public class MeshGenerator : MonoBehaviour
{

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    Color[] colours;

    public int xSize = 2;
    public int ySize = 2;
    public int zSize = 2;

    // Try with "Data/RadcliffeTemperature/temp" or "Data/RoyalMailSharePrice/price"
    public string dataset = "Data/RoyalMailSharePrice/price";

    // Use every `takeEvery` value in the dataset, e.g. if 1 uses every value, if 10 uses every 10th value
    public int takeEvery = 10;

    // Toggle to instead render Apple order books data
    // TODO: the option to pick a different visualisation with the mesh should probably be somewhere else but this works for now
    public bool renderOrderbookData = false;

    public Gradient gradient;

    //beware, if both sides are rendered, we basically render the entire mesh twice
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

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        Triangulation();
    }

    void UpdateMesh()
    {
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

        //if we want to render both sides of the mesh
        if (renderBothSides)
        {
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
                vertices[vertLength + v] = new Vector3(vertices[v].x, vertices[v].y - 0.01f, vertices[v].z);
            }

            // Add reversed triangles for the second side
            for (int t = 0; t < triLength; t += 3)
            {
                // reverse vertex order, offset to be vertices on underside of mesh
                triangles[triLength + t] = outputTriangles[t + 2] + vertLength; 
                triangles[triLength + t + 1] = outputTriangles[t + 1] + vertLength;
                triangles[triLength + t + 2] = outputTriangles[t] + vertLength;
            }
        } else 
        {
            triangles = outputTriangles;
        }

        //create colours for gradient on mesh
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

        if (!renderOrderbookData)
        {
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
        } else {

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
    }
}
