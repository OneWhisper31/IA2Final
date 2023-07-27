using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Node : MonoBehaviour
{
    public int heuristic;
    public Node[] neighbours;
    void Start()
    {
        neighbours = Physics.OverlapSphere(transform.position, 0.6f).Where(x => x.GetComponent<Node>() != null).Select(x => x.GetComponent<Node>())
                                                                 .Where(x => x != this).ToArray();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }

}
