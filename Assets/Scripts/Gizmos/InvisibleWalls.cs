using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InvisibleWalls : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        var colliders = GetComponentsInChildren<Collider>().OfType<BoxCollider>();//IA2-LINQ
        foreach (var collider in colliders)
        {

            Matrix4x4 cubeTransform = Matrix4x4.TRS(
                collider.transform.position,
                collider.transform.rotation,
                collider.transform.lossyScale
            );

            Gizmos.matrix = cubeTransform;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
    }
}
