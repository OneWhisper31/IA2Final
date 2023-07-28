using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class test : MonoBehaviour
{
    //[SerializeField] GameObject node;
    //[SerializeField] float width, height;
    /*void Start()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Instantiate(node, new Vector3(i*0.5f, 0, j*0.5f), Quaternion.identity);
            }
        }
    }*/
    [SerializeField] Node start;
    [SerializeField] Node end;

    private void Start()
    {
        UseAStar();
    }
    Node[] path;
    public void UseAStar()
    {
        

        StartCoroutine(AStar.CalculatePath(start, (x) => x == end,
                (x) => x.neighbours.Select(x => new WeightedNode<Node>(x, 1)),
                x => x.heuristic,
                x => path = x.ToArray()));
    }
    IEnumerator Path()
    {
        yield return new WaitForSecondsRealtime(2);
        foreach (var item in path)
        {
            Debug.Log(item.name);
        }
    }
}
