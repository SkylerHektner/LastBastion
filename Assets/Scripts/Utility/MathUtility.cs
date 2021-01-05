using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtility
{
    // all rotations are done counter clockwise. use negative radians to go clockwise
    public static Vector3 RotateVector2D(Vector3 vec, float radians)
    {
        float cos_rad = Mathf.Cos( radians );
        float sin_rad = Mathf.Sin( radians );
        return new Vector3( cos_rad * vec.x - sin_rad * vec.y,
            sin_rad * vec.x + cos_rad * vec.y );
    }

    // all rotations are done counter clockwise. use negative radians to go clockwise
    public static Vector2 RotateVector2D( Vector2 vec, float radians )
    {
        float cos_rad = Mathf.Cos( radians );
        float sin_rad = Mathf.Sin( radians );
        return new Vector2( cos_rad * vec.x - sin_rad * vec.y,
            sin_rad * vec.x + cos_rad * vec.y );
    }
}
