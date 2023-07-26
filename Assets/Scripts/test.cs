using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] GameObject node;
    [SerializeField] float width, height;
    void Start()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Instantiate(node, new Vector3(i*0.5f, 0, j*0.5f), Quaternion.identity);
            }
        }
    }
}
