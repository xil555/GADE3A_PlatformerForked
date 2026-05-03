using UnityEngine;

public class LinkedListADT1 : MonoBehaviour
{
    private Node1 head;

    // Add waypoint to list
    public void Add(Transform data)
    {
        Node1 newNode = new Node1(data);

        if (head == null)
        {
            head = newNode;
            return;
        }

        Node1 current = head;

        while (current.next != null)
        {
            current = current.next;
        }

        current.next = newNode;
    }

    // Get first node
    public Node1 GetHead()
    {
        return head;
    }
}
