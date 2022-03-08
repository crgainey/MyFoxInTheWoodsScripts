using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Makes a grid of Nodes. 
public class PathGrid : MonoBehaviour
{
	public bool displayGizmos;
	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;

	Node[,] grid;
	float nodeDiameter;
	int gridSizeX, gridSizeY;

	void Awake()
	{
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

		CreateGrid();
	}

	public int MaxSize
	{
		get
		{
			return gridSizeX * gridSizeY;
		}
	}
	void CreateGrid()
	{
		grid = new Node[gridSizeX, gridSizeY];
		Vector2 worldBottomLeft = (Vector2)transform.position - Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;

		for (int x = 0; x < gridSizeX; x++)
		{
			for (int y = 0; y < gridSizeY; y++)
			{
				Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
				bool walkable = (Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableMask) == null); // if no collider node is walkable

				grid[x, y] = new Node(walkable, worldPoint, x, y);
			}
		}
	}

	public List<Node> GetNeighbors(Node node)
	{
		List<Node> neighbors = new List<Node>();

		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				if (x == 0 && y == 0)
				{
					continue;
				}

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
				{
					neighbors.Add(grid[checkX, checkY]);
				}
			}

		}

		return neighbors;
	}

	//convert world pos into grid pos.
	public Node GetNodeFromWorldPoint(Vector2 worldPosition)
	{
		//Convert to a precentage for x,y how far on the grid
		float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
		float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;
		//prevents information from outside our grid
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		//-1 makes sure we are not outside the array
		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
		return grid[x, y];

	}


	//Vizualizes the grid
	void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(transform.position, new Vector2(gridWorldSize.x, gridWorldSize.y));

		if (grid != null && displayGizmos)
		{
			foreach (Node n in grid)
			{
				Gizmos.color = Color.red;
				if (n.walkable)
				{
					Gizmos.color = Color.white;
				}
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
			}
		}

	}
}
