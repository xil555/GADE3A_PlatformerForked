using UnityEngine;

public class LinkedListADT1 : MonoBehaviour
{
    private Node1 head;
    private Node1 tail;

    public void Add(Transform data)
    {
        if (data == null) return;

        Node1 newNode = new Node1(data);

        if (head == null)
        {
            head = newNode;
            tail = newNode;
            return;
        }

        tail.next = newNode;
        tail = newNode;
    }

    public Node1 GetHead()
    {
        return head;
    }
    public void Clear()
    {
        head = null;
        tail = null;
    }
    public Node1 GetNext(Node1 current)
    {
        if (current == null) return head;

        if (current.next == null)
            return head;

        return current.next;
    }
}