using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleQuery : MonoBehaviour, IQuery
{

    public SpatialGrid targetGrid;
    public float radious = 15f;
    public IEnumerable<IGridEntity> selected = new List<IGridEntity>();

    public IEnumerable<IGridEntity> Query()
    {
        var h = radious;
        var w = radious;
        //posicion inicial --> esquina superior izquierda de la "caja"
        //posición final --> esquina inferior derecha de la "caja"
        //como funcion para filtrar le damos una que siempre devuelve true, para que no filtre nada.
        return targetGrid.Query(
                                transform.position + new Vector3(-w, 0, -h),
                                transform.position + new Vector3(w, 0, h),
                                x => Vector3.Distance(x, transform.position) <= radious);
    }

    void OnDrawGizmos()
    {
        if (targetGrid == null) return;

        //Flatten the sphere we're going to draw
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radious);
    }
}
