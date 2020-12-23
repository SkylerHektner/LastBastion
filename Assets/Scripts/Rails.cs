using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Rails : MonoBehaviour
{
    public static Rails Instance;

    [SerializeField] [Range(0.01f, 0.5f)] float vfxUpdateInterval = 0.1f;
    [SerializeField] [Range( 0.01f, 1.0f )] float vfxLightningDensity = 0.1f;
    [SerializeField] [Range( 0.01f, 1.0f )] float vfxLightningRange = 0.1f;
    public Vector3 RailAreaTopRight;
    public Vector3 RailAreaBottomLeft;
    [SerializeField] LineRenderer leftLineRenderer;
    [SerializeField] LineRenderer rightLineRenderer;
    private Vector3 RailAreaTopLeft;
    private Vector3 RailAreaBottomRight;

    public float Top { get { return RailAreaTopRight.y; } }
    public float Bottom { get { return RailAreaBottomLeft.y; } }
    public float Right { get { return RailAreaTopRight.x; } }
    public float Left { get { return RailAreaBottomLeft.x; } }

    public void Start()
    {
        Instance = this;
        RailAreaTopLeft = new Vector3( RailAreaBottomLeft.x, RailAreaTopRight.y );
        RailAreaBottomRight = new Vector3( RailAreaTopRight.x, RailAreaBottomLeft.y );
        StartCoroutine( updateLines() );
    }

    IEnumerator updateLines()
    {
        while( true )
        {
            updateLineRenderer();
            yield return new WaitForSeconds( vfxUpdateInterval );
        }
    }

    private void updateLineRenderer()
    {
        {
            var points = addDeviance( RailAreaTopLeft, RailAreaBottomLeft );
            leftLineRenderer.positionCount = points.Length;
            leftLineRenderer.SetPositions( points );
        }
        {
            var points = addDeviance( RailAreaTopRight, RailAreaBottomRight);;
            rightLineRenderer.positionCount = points.Length;
            rightLineRenderer.SetPositions( points );
        }
    }

    // creates a list of points with the appropriate deviance. Includes the start and end points
    private UnityEngine.Vector3[] addDeviance( Vector3 start, Vector3 end )
    {
        List<Vector3> result = new List<Vector3>();
        result.Add( start );
        Vector3 path = end - start;
        for( int i = 0; i * vfxLightningDensity < path.magnitude; i++ )
        {
            Vector3 point = new Vector3();
            point.x = path.normalized.x * i * vfxLightningDensity + UnityEngine.Random.Range( -vfxLightningRange, vfxLightningRange );
            point.y = path.normalized.y * i * vfxLightningDensity + UnityEngine.Random.Range( -vfxLightningRange, vfxLightningRange );
            point.x += start.x;
            point.y += start.y;
            point.z = -1.0f;
            result.Add( point );
        }
        result.Add( end );

        return result.ToArray();
    }
}

// EDITOR
[CustomEditor( typeof( Rails ) )]
public class RailsEdtior : Editor
{
    private void OnSceneGUI()
    {
        Rails rails = (Rails)target;

        Vector3 top_right = rails.RailAreaTopRight;
        Vector3 bottom_left = rails.RailAreaBottomLeft;
        Vector3 top_left = new Vector3( bottom_left.x, top_right.y, ( bottom_left.z + top_right.z ) / 2.0f );
        Vector3 bottom_right = new Vector3( top_right.x, bottom_left.y, ( bottom_left.z + top_right.z ) / 2.0f );
        Handles.color = Color.red;
        Handles.DrawLine( top_left, top_right );
        Handles.DrawLine( top_right, bottom_right );
        Handles.DrawLine( bottom_right, bottom_left );
        Handles.DrawLine( bottom_left, top_left );

    }
}
