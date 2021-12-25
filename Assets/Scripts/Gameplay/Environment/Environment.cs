using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Environment : MonoBehaviour
{
    [Header( "---Spawn Area Information---" )]
    public Vector3 SpawnableAreaTopRight;
    public Vector3 SpawnableAreaBottomLeft;
    public Vector3 PlayableAreaTopRight;
    public Vector3 PlayableAreaBottomLeft;
    public List<Vector3> DoorSpawnPoints = new List<Vector3>();
    public Vector3 BossSpawnPoint = Vector3.zero;
    public string EnvironmentID;

    [Header( "---Animation Trigger System---" )]
    public List<Animator> AnimationTriggerListeners = new List<Animator>();
}

// EDITOR
#if UNITY_EDITOR
[CustomEditor( typeof( Environment ) )]
public class EnvironmentEditor : Editor
{
    private void OnSceneGUI()
    {
        Environment spawn_manager = (Environment)target;

        {
            Vector3 top_right = spawn_manager.SpawnableAreaTopRight;
            Vector3 bottom_left = spawn_manager.SpawnableAreaBottomLeft;
            Vector3 top_left = new Vector3( bottom_left.x, top_right.y, ( bottom_left.z + top_right.z ) / 2.0f );
            Vector3 bottom_right = new Vector3( top_right.x, bottom_left.y, ( bottom_left.z + top_right.z ) / 2.0f );
            Handles.color = Color.cyan;
            Handles.DrawLine( top_left, top_right );
            Handles.DrawLine( top_right, bottom_right );
            Handles.DrawLine( bottom_right, bottom_left );
            Handles.DrawLine( bottom_left, top_left );
        }

        {
            Vector3 top_right = spawn_manager.PlayableAreaTopRight;
            Vector3 bottom_left = spawn_manager.PlayableAreaBottomLeft;
            Vector3 top_left = new Vector3( bottom_left.x, top_right.y, ( bottom_left.z + top_right.z ) / 2.0f );
            Vector3 bottom_right = new Vector3( top_right.x, bottom_left.y, ( bottom_left.z + top_right.z ) / 2.0f );
            Handles.color = Color.green;
            Handles.DrawLine( top_left, top_right );
            Handles.DrawLine( top_right, bottom_right );
            Handles.DrawLine( bottom_right, bottom_left );
            Handles.DrawLine( bottom_left, top_left );
        }

        {
            foreach( Vector3 door_spawn_point in spawn_manager.DoorSpawnPoints )
            {
                Handles.color = Color.red;
                Handles.DrawWireDisc( door_spawn_point, Vector3.forward, 0.2f );
            }
        }

        {
            Handles.color = Color.magenta;
            Handles.DrawWireDisc( spawn_manager.BossSpawnPoint, Vector3.forward, 0.2f );
        }

    }
}
#endif
