using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Physics2DExtension
{
    public static bool InFieldOfView(this Vector3 start, Vector3 end, float viewRadius, float viewAngle, LayerMask wallLayer)
    {
        Vector2 dir = end - start;
        if (dir.magnitude > viewRadius) return false;
        if (!InLineOfSight(end, start, wallLayer)) return false;
        return Vector3.Angle(end, dir) <= viewAngle / 2;
    }
    
    public static bool InLineOfSight(this Vector3 start, Vector3 end, LayerMask wallLayer)
    {
        //origen, direccion, distancia maxima, layer mask
        Vector2 dir = end - start;
        return !Physics2D.Raycast(start, dir,dir.magnitude, wallLayer);
    }
    public static bool InLineOfSight(this Vector3 start, Vector3 end, float distance, LayerMask wallLayer)
    {
        //origen, direccion, distancia maxima, layer mask
        Vector2 dir = end - start;
        return !Physics2D.Raycast(start, dir, distance, wallLayer);
    }
    public static bool InLineOfSight(this Vector3 start, Vector3 end)
    {
        //origen, direccion, distancia maxima, layer mask
        Vector3 dir = end - start;
        return !Physics.Raycast(start, dir,dir.magnitude);
    }
    public static bool CanPassThrough(this Vector3 start, Vector3 end,float width, LayerMask wallLayer)
    {
        Vector2 dir = end - start;
        Vector2 dirRight = dir + Vector2.right* width;
        Vector2 dirLeft = dir - Vector2.right * width;
        Vector2 dirUp = dir + Vector2.up * width;
        Vector2 dirDown = dir - Vector2.up * width;

        if (!Physics2D.Raycast(start, dir, dir.magnitude, wallLayer))
            if (!Physics2D.Raycast(start, dirRight, dirRight.magnitude, wallLayer))
                if (!Physics2D.Raycast(start, dirLeft, dirLeft.magnitude, wallLayer))
                    if (!Physics2D.Raycast(start, dirUp, dirUp.magnitude, wallLayer))
                        return !Physics2D.Raycast(start, dirDown, dirDown.magnitude, wallLayer);

        return false;
    }
}
