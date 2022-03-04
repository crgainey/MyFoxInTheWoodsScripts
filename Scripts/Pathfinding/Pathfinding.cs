using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

//Algoathum finding the paths
public class Pathfinding : MonoBehaviour
{
    PathGrid grid;
    static Pathfinding instance;


    void Awake()
    {
        grid = GetComponent<PathGrid>(); //needs to be on the same gameobject for it to read
        instance = this;
    }

    public static Vector2[] RequestPath(Vector2 startPos, Vector2 endPos)
    {
        return instance.FindPath(startPos, endPos);
    }

    Vector2[] FindPath(Vector2 startPos, Vector2 endPos) //CHECK 
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector2[] waypoints = new Vector2[0];
        bool pathSuccess = false;

        Node startNode = grid.GetNodeFromWorldPoint(startPos);
        Node endNode = grid.GetNodeFromWorldPoint(endPos);

        if (startNode.walkable && endNode.walkable)
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> cloasedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                cloasedSet.Add(currentNode);

                if (currentNode == endNode)
                {
                    sw.Stop();
                    //print("Path found: " + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbor in grid.GetNeighbors(currentNode))
                {
                    if (!neighbor.walkable || cloasedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    int newMovementCostToNeightbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                    if (newMovementCostToNeightbor < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = newMovementCostToNeightbor;
                        neighbor.hCost = GetDistance(neighbor, endNode);
                        neighbor.parent = currentNode;

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                        else
                        {
                            openSet.UpdateItem(neighbor);
                        }
                    }
                }
            }
        }


        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, endNode);
        }
        return waypoints;
    }

    //Going from end of path backwards 
    Vector2[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector2[] waypoints = SimplfyPath(path);
        Array.Reverse(waypoints); //will start from beginning
        return waypoints;
    }

    Vector2[] SimplfyPath(List<Node> path)
    {
        List<Vector2> waypoints = new List<Vector2>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }

        return waypoints.ToArray();
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }

        return 14 * distX + 10 * (distY - distX);
    }
}

