using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

[Serializable]
public class Map : MonoBehaviour
{
    [SerializeField]
    MeshFilter meshFilter;

    public bool[,] mapArray;
    public int numrow { get; private set; }
    public int numcol { get; private set; }
    public float size { get; private set; }
    public float startX { get; private set; }
    public float startY { get; private set; }
    public void InitMap(int numrow, int numcol, float startX = 0, float startY = 0, float size = 1)
    {
        this.numrow = numrow;
        this.numcol = numcol;
        this.startX = startX;
        this.startY = startY;
        this.size = size;
        mapArray = new bool[numrow, numcol];
        for (int row = 0; row < numrow; row++)
        {
            for (int col = 0; col < numcol; col++)
            {
                mapArray[row, col] = false;
            }
        }
        DrawMap();
    }
    public void ResetMap()
    {
        for (int row = 0; row < numrow; row++)
        {
            for (int col = 0; col < numcol; col++)
            {
                SetValue(row, col, false);
            }
        }
    }
    public Vector3 GetPosition(int row, int col)
    {
        return new Vector3(startX + row * size, startY + col * size);
    }
    public void SetValue(Vector3 position, bool value)
    {
        int row = (int)((position.x - startX) / size);
        int col = (int)((position.y - startY) / size);
        SetValue(row, col, value);
    }
    public void SetValue(int row, int col, bool value)
    {
        if (!IsInArray(row, col)) return;
        mapArray[row, col] = value;
        SetMeshColor(row, col, value ? shadowColor : blankColor);
    }
    public bool GetValue(int row, int col)
    {
        if (!IsInArray(row, col)) return false;
        return mapArray[row, col];
    }
    public bool IsInArray(int row, int col)
    {
        if (row < 0 || row >= numrow || col < 0 || col >= numcol)
        {
            //Debug.LogError(string.Format("Out of Array: row:{0}-numrow:{1}, col{2}-numcol{3}", row, numrow, col, numcol));
            return false;
        }
        return true;
    }
    public List<Vector2> GetBlankAreaList()
    {
        List<Vector2> blankAreaList = new List<Vector2>();
        for (int row = 0; row < numrow; row++)
        {
            for (int col = 0; col < numcol; col++)
            {
                if (mapArray[row, col] == false)
                    blankAreaList.Add(new Vector2(row, col));
            }
        }
        return blankAreaList;
    }
    public void DebugMap()
    {
        for (int row = 0; row < numrow; row++)
        {
            for (int col = 0; col < numcol; col++)
            {
                if (GetValue(row, col))
                    DebugRectagle(startX + row * size, startY + col * size, size, Color.white);
                else
                    DebugRectagle(startX + row * size, startY + col * size, size, Color.red);
            }
        }
    }
    void DebugRectagle(float x, float y, float size, Color color)
    {
        float halfSize = size / 2;
        Vector3 TL = new Vector3(x - halfSize, y + halfSize);
        Vector3 TR = new Vector3(x + halfSize, y + halfSize);
        Vector3 BL = new Vector3(x - halfSize, y - halfSize);
        Vector3 BR = new Vector3(x + halfSize, y - halfSize);
        Debug.DrawLine(TL, TR, color);
        Debug.DrawLine(BL, BR, color);
        Debug.DrawLine(TL, BL, color);
        Debug.DrawLine(TR, BR, color);
    }
    
    Color[] meshColors;
    [SerializeField]
    Color shadowColor = new Color(0, 0, 0, 0.2f);
    [SerializeField]
    Color blankColor = new Color(0, 0, 0, 0);
    //Mesh mesh;
    void DrawMap()
    {
        Vector3[] vertices;
        Vector2[] uv;
        int[] triangles;

        Mesh mesh = new Mesh();
        vertices = new Vector3[numcol * numrow * 4];
        uv = new Vector2[vertices.Length];
        meshColors = new Color[vertices.Length];
        triangles = new int[numcol * numrow * 6];
        int ivert = 0;
        int itri = 0;
        for (int row = 0; row < numrow; row++)
            for (int col = 0; col < numcol; col++)
            {
                Vector3[] fourCorner = GetFourCorner(startX + col * size, startY + row * size, size);

                vertices[ivert] = fourCorner[0];
                vertices[ivert + 1] = fourCorner[1];
                vertices[ivert + 2] = fourCorner[2];
                vertices[ivert + 3] = fourCorner[3];

                uv[ivert] = new Vector2(0, 0);
                uv[ivert + 1] = new Vector2(1, 0);
                uv[ivert + 2] = new Vector2(0, 1);
                uv[ivert + 3] = new Vector2(1, 1);

                meshColors[ivert] = blankColor;
                meshColors[ivert + 1] = blankColor;
                meshColors[ivert + 2] = blankColor;
                meshColors[ivert + 3] = blankColor;

                triangles[itri] = ivert;
                triangles[itri + 1] = ivert + 1;
                triangles[itri + 2] = ivert + 2;

                triangles[itri + 3] = ivert + 1;
                triangles[itri + 4] = ivert + 3;
                triangles[itri + 5] = ivert + 2;

                ivert += 4;
                itri += 6;
            }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.colors = meshColors;
        meshFilter.mesh = mesh;
    }
    public void UpdateMeshColor()
    {
        meshFilter.mesh.colors = meshColors;
    }
    void SetMeshColor(int row, int col, Color color)
    {
        int i = (col * numrow + row) * 4;
        if (i > 0 && i + 3 < meshColors.Length)
        {
            meshColors[i] = meshColors[i + 1] = meshColors[i + 2] = meshColors[i + 3] = color;
        }

    }
    Vector3[] GetFourCorner(float x, float y, float size)
    {
        float halfSize = size / 2;
        return new Vector3[4]
        {
            new Vector3(x - halfSize, y + halfSize, transform.position.z),//TL
            new Vector3(x + halfSize, y + halfSize, transform.position.z),//TR
            new Vector3(x - halfSize, y - halfSize, transform.position.z),//BL
            new Vector3(x + halfSize, y - halfSize, transform.position.z)//BR
        };
    }
}
