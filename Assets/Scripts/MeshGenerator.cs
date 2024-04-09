using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{

    // code from Brackeys video https://www.youtube.com/watch?v=64NblGkAabk&ab_channel=Brackeys

    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    Color[] colours;

    public int xSize = 20;
    public int zSize = 20;

    public Gradient gradient;

    float minHeight;
    float maxHeight;

    void Start()
    {
        //make mesh

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateMesh();
        UpdateMesh();
    }


    void CreateMesh()
    {
        vertices = new Vector3[(xSize+1) * (zSize+1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = 0; //change this to be heights
                vertices[i] = new Vector3(x, y, z);

                maxHeight = Math.Max(maxHeight, y);
                minHeight = Math.Min(minHeight, y);

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
                float height = Mathf.InverseLerp(minHeight, maxHeight, vertices[i].y);
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
        mesh.colors = colours;

        mesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        if (vertices == null)
            return;
        
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], .1f);
        }
    }

}
