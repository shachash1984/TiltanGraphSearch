using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AStarUtil 
{
    static public SortedQueue<SNode> openList;
    static public HashSet<SNode> closedList;

    private static List<SNode> CalculatePath(SNode sn)
    {
        List<SNode> list = new List<SNode>();
        while (sn != null)
        {
            list.Add(sn);
            sn = (SNode)sn.parent;
        }
        list.Reverse();
        return list;
    }

    private static float GetHeuristicEstimateCost(SNode curNode, SNode goalNode)
    {
        Vector3 vecCost = curNode.position - goalNode.position;
        return vecCost.magnitude;
    }

    public static List<SNode> FindPath(SNode start, SNode goal)
    {
        openList = new SortedQueue<SNode>();
        openList.Push(start);
        start.initialCost = 0.0f;
        start.estimatedCost = GetHeuristicEstimateCost(start, goal);
        start.nodeTotalCost = start.initialCost + start.estimatedCost;

        closedList = new HashSet<SNode>();
        SNode sNode = null;

        while (openList.Length != 0)
        {
            sNode = openList.First;
            if (sNode.position == goal.position)
                return CalculatePath(sNode);

            List<SNode> neighbors = new List<SNode>();
            GridHandler.S.GetNeighbors(sNode, neighbors);

            for (int i = 0; i < neighbors.Count; i++)
            {
                SNode neighborNode = neighbors[i];
                if(!closedList.Contains(neighborNode))
                {
                    neighborNode.estimatedCost = GetHeuristicEstimateCost(neighborNode, goal);
                    neighborNode.initialCost = sNode.initialCost + 1;
                    neighborNode.nodeTotalCost = neighborNode.estimatedCost + neighborNode.initialCost;

                    neighborNode.parent = sNode;
                    if(!openList.Contains(neighborNode))
                    {
                        openList.Push(neighborNode);
                    }
                }
            }

            closedList.Add(sNode);
            openList.Remove(sNode);
        }

        //If finished looping and cannot find the goal then return null
        if (sNode.position != goal.position)
        {
            Debug.LogError("Goal Not Found");
            return null;
        }

        //Calculate the path based on the final node
        return CalculatePath(sNode);
    }
}
