using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Map
{
    public bool[,] mapArray;
    public int numrow { get; private set; }
    public int numcol { get; private set; }
    public float size { get; private set; }
    public float startX { get; private set; }
    public float startY { get; private set; }
    public Map(int numrow, int numcol, float startX = 0, float startY = 0, float size = 1)
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
            Debug.LogError(string.Format("Out of Array: row:{0}-numrow:{1}, col{2}-numcol{3}", row, numrow, col, numcol));
            return false;
        }
        return true;
    }
    public List<Vector2> GetblankAreaList()
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
}
