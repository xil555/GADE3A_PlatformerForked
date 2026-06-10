using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GraphNode
{
    public Transform waypoint;
    public List<int> neighbourIndices = new List<int>();
}
