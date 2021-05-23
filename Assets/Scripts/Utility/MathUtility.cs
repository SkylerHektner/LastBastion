using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtility
{
    // reflects a vector along a line 
    // https://math.stackexchange.com/questions/13261/how-to-get-a-reflection-vector
    public static Vector2 ReflectVector(Vector2 input_vec, Vector2 reflection_vec)
    {
        reflection_vec.Normalize();
        return input_vec - 2 * ( Vector2.Dot( input_vec, reflection_vec ) * reflection_vec );
    }

    // all rotations are done counter clockwise. use negative radians to go clockwise
    public static Vector3 RotateVector2D( Vector3 vec, float radians )
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
    public static Vector2 PerpendicularClockwise( Vector2 vector2 )
    {
        return new Vector2( vector2.y, -vector2.x );
    }

    public static Vector2 PerpendicularCounterClockwise( Vector2 vector2 )
    {
        return new Vector2( -vector2.y, vector2.x );
    }

    // only works in 2d space
    public static Vector3 PerpendicularClockwise( Vector3 vector2 )
    {
        return new Vector2( vector2.y, -vector2.x );
    }

    // only works in 2d space
    public static Vector3 PerpendicularCounterClockwise( Vector3 vector2 )
    {
        return new Vector2( -vector2.y, vector2.x );
    }
}
