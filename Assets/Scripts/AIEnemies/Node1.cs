using UnityEngine;

public class Node1 
{
    public Transform data;
    public Node1 next;

    public Node1(Transform data)
    {
        this.data = data;
        this.next = null;
    }
}
