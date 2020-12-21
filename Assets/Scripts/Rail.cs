using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Rail : MonoBehaviour
{
    public static Rail LeftRail;
    public static Rail RightRail;

    [SerializeField] bool IsLeftRail = false;
    [SerializeField] [Range(0.01f, 0.5f)] float vfxUpdateInterval = 0.1f;
    [SerializeField] [Range( 0.01f, 1.0f )] float vfxLightningDensity = 0.1f;
    [SerializeField] [Range( 0.01f, 1.0f )] float vfxLightningRange = 0.1f;
    [SerializeField] GameObject topGameObject;
    [SerializeField] GameObject bottomGameObject;
    LineRenderer lineRenderer;

    public float Top { get { return topGameObject.transform.position.y; } }
    public float Bottom { get { return bottomGameObject.transform.position.y; } }
    public float RailCenter { get { return transform.position.x; } }

    public void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        StartCoroutine( updateLine() );
        if( IsLeftRail ) LeftRail = this;
        else RightRail = this;
    }

    IEnumerator updateLine()
    {
        while( true )
        {
            updateLineRenderer();
            yield return new WaitForSeconds( vfxUpdateInterval );
        }
    }

    private void updateLineRenderer()
    {
        var points = addDeviance( topGameObject.transform.position, bottomGameObject.transform.position );
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions( points );
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
