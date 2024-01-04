using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
//SOME ERRORS CAUSE BY THIS CODE WHICH IS NO USE FOR THE MOMENT
public class UIGridRenderer : Graphic
{
    public Vector2Int gridSize = new Vector2Int(10, 10);
    public float thickness = 10f;
    private float cellWidth;
    private float cellHeight;

    private float width;
    private float height;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        cellWidth = width / (float)gridSize.x;
        cellHeight = height / (float)gridSize.y;

        int count = 0;

        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                DrawCell(x, y, count, vh);
                count++;
            }
        }
    }

    private void DrawCell(int x, int y, int index, VertexHelper vh)
    {
        float xPos = x * cellWidth;
        float yPos = y * cellHeight;

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = new Vector3(xPos, yPos);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos, yPos + cellHeight);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + cellWidth, yPos + cellHeight);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + cellWidth, yPos);
        vh.AddVert(vertex);

        float grid_distance = Mathf.Sqrt(cellWidth * cellWidth + cellHeight * cellHeight);

        vertex.position = new Vector3(xPos + grid_distance, yPos + grid_distance);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + grid_distance, yPos + cellHeight - grid_distance);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + cellWidth - grid_distance, yPos + cellHeight - grid_distance);
        vh.AddVert(vertex);

        vertex.position = new Vector3(xPos + cellWidth - grid_distance, yPos + grid_distance);
        vh.AddVert(vertex);

        int offset = index * 8;

        //Left Edge
        vh.AddTriangle(0 + offset, 1 + offset, 5 + offset);
        vh.AddTriangle(5 + offset, 4 + offset, 0 + offset);

        //Top Edge
        vh.AddTriangle(1 + offset, 2 + offset, 6 + offset);
        vh.AddTriangle(6 + offset, 5 + offset, 1 + offset);

        //Right Edge
        vh.AddTriangle(2 + offset, 3 + offset, 7 + offset);
        vh.AddTriangle(7 + offset, 6 + offset, 2 + offset);

        //Bottom Edge
        vh.AddTriangle(3 + offset, 0 + offset, 4 + offset);
        vh.AddTriangle(4 + offset, 7 + offset, 3 + offset);
        
        float widthSqr = thickness * thickness;
        float grid_distanceSqr = widthSqr/2f;
        float grid_distance = Mathf.Sqrt(grid_distanceSqr);

        vertex.position = new Vector3(grid_distance, grid_distance);
        vh.AddVert(vertex);

        vertex.position = new Vector3(grid_distance, height - grid_distance);
        vh.AddVert(vertex);

        vertex.position = new Vector3(width - grid_distance, height - grid_distance);
        vh.AddVert(vertex);

        vertex.position = new Vector3(width - grid_distance, grid_distance);
        vh.AddVert(vertex);

        //Left Edge
        vh.AddTriangle(0, 1, 5);
        vh.AddTriangle(5, 4, 0);

        //Top Edge
        vh.AddTriangle(1, 2, 6);
        vh.AddTriangle(6, 5, 1);

        //Right Edge
        vh.AddTriangle(2, 3, 7);
        vh.AddTriangle(7, 6, 2);

        //Bottom Edge
        vh.AddTriangle(3, 0, 4);
        vh.AddTriangle(4, 7, 3);
    }
}
*/