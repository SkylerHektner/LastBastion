using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CreateAssetMenu( fileName = "SpawnGroup", menuName = "ScriptableObjects/SpawnGroup", order = 1 )]
[System.Serializable]
public class SpawnGroup : ScriptableObject
{
    public static readonly Vector2 DEFAULT_CUSTOM_SPAWN_LAYOUT_POSITION = new Vector2( 0.5f, 0.5f );

    [SerializeField] public SpawnDictionary SpawnMap;
    [SerializeField] public Layout layout;
    [SerializeField] public List<Vector2> custom_layout_positions = new List<Vector2>(); // only set if a custom layout is used
    [SerializeField] public float ClusterDensity = 2.0f; // per meter squared
    [SerializeField] public float SpawnStaggerMinTime = 0.02f;
    [SerializeField] public float SpawnStaggerMaxTime = 0.07f;

    // editor ui only
    [System.NonSerialized] public bool confirm_clear = false;
    [SerializeField] public Vector2Int custom_layout_grid_size = new Vector2Int( 10, 10 );

    [System.Serializable]
    public enum Layout
    {
        Cluster,
        Spread,
        Door,
        Boss,
        Custom,
        PointCluster,
    }
}

#if UNITY_EDITOR
[CustomEditor( typeof( SpawnGroup ) )]
public class SpawnGroupEdtior : Editor
{
    public override void OnInspectorGUI()
    {
        SpawnGroup spawn_group = (SpawnGroup)target;
        var enemy_enum_values = Enum.GetValues( typeof( EnemyEnum ) ).Cast<EnemyEnum>();

        // no spawnmap exists
        if( spawn_group.SpawnMap == null )
        {
            spawn_group.SpawnMap = new SpawnDictionary();
            EditorUtility.SetDirty( target );
        }
        // new enemy types have been added
        else if( spawn_group.SpawnMap.Count < enemy_enum_values.Count() )
        {
            foreach( var e in enemy_enum_values )
                if( !spawn_group.SpawnMap.ContainsKey( e ) )
                    spawn_group.SpawnMap[e] = 0;
            EditorUtility.SetDirty( target );
        }
        // enemy types have been removed
        else if( spawn_group.SpawnMap.Count > enemy_enum_values.Count() )
        {
            var old_spawn_map = spawn_group.SpawnMap;
            spawn_group.SpawnMap = new SpawnDictionary();
            foreach( var e in enemy_enum_values )
            {
                spawn_group.SpawnMap[e] = old_spawn_map[e];
            }
            EditorUtility.SetDirty( target );
        }
        // fixup custom locations
        if( spawn_group.custom_layout_positions == null ||
            spawn_group.custom_layout_positions.Count != spawn_group.SpawnMap.Sum( kvp => kvp.Value ) )
        {
            List<Vector2> old_custom_layout = spawn_group.custom_layout_positions;
            spawn_group.custom_layout_positions = new List<Vector2>();
            int total_spawns = spawn_group.SpawnMap.Sum( kvp => kvp.Value );
            for( int x = 0; x < total_spawns; ++x )
            {
                spawn_group.custom_layout_positions.Add(
                    ( old_custom_layout != null && x < old_custom_layout.Count )
                        ? old_custom_layout[x] : SpawnGroup.DEFAULT_CUSTOM_SPAWN_LAYOUT_POSITION );
            }
            EditorUtility.SetDirty( target );
        }

        int spawn_index_tracker = 0;
        foreach( var e in Enum.GetValues( typeof( EnemyEnum ) ) )
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField( e.ToString() + ": " + spawn_group.SpawnMap[(EnemyEnum)e].ToString() );
            spawn_index_tracker += spawn_group.SpawnMap[(EnemyEnum)e];
            if( GUILayout.Button( "+", GUILayout.Width( 25 ) ) )
            {
                spawn_group.SpawnMap[(EnemyEnum)e]++;
                spawn_group.custom_layout_positions.Insert( spawn_index_tracker, SpawnGroup.DEFAULT_CUSTOM_SPAWN_LAYOUT_POSITION );
                EditorUtility.SetDirty( target );
            }
            else if( GUILayout.Button( "-", GUILayout.Width( 25 ) ) && spawn_group.SpawnMap[(EnemyEnum)e] >= 1 )
            {
                spawn_group.SpawnMap[(EnemyEnum)e]--;
                spawn_group.custom_layout_positions.RemoveAt( spawn_index_tracker - 1 );
                EditorUtility.SetDirty( target );
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        SpawnGroup.Layout layout = (SpawnGroup.Layout)EditorGUILayout.EnumPopup( "Layout: ", spawn_group.layout );
        if( layout != spawn_group.layout )
        {
            spawn_group.layout = layout;
            EditorUtility.SetDirty( target );
        }

        if( layout == SpawnGroup.Layout.Cluster || layout == SpawnGroup.Layout.PointCluster )
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField( "Monster Per Meter^2" );
            float cluster_density = EditorGUILayout.FloatField( spawn_group.ClusterDensity );
            if( cluster_density != spawn_group.ClusterDensity )
            {
                if( cluster_density <= 0.0f )
                    cluster_density = 0.1f;
                spawn_group.ClusterDensity = cluster_density;
                EditorUtility.SetDirty( target );
            }
            EditorGUILayout.EndHorizontal();
        }
        else if( layout == SpawnGroup.Layout.Custom )
        {
            if( GUILayout.Button( "Edit Custom Layout" ) )
            {
                CustomSpawnLayoutEditor.OpenWindow( spawn_group );
            }
            if( spawn_group.confirm_clear )
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField( "Definitely clear the custom layout?" );
                if( GUILayout.Button( "Yes" ) )
                {
                    for( int x = 0; x < spawn_group.custom_layout_positions.Count; ++x )
                    {
                        spawn_group.custom_layout_positions[x] = SpawnGroup.DEFAULT_CUSTOM_SPAWN_LAYOUT_POSITION;
                    }
                    EditorUtility.SetDirty( target );
                    spawn_group.confirm_clear = false;
                }
                else if( GUILayout.Button( "No" ) )
                {
                    spawn_group.confirm_clear = false;
                }

                EditorGUILayout.EndHorizontal();
            }
            else if( GUILayout.Button( "CLEAR CUSTOM LAYOUT" ) )
            {
                spawn_group.confirm_clear = true;
            }

            Vector2Int new_grid = EditorGUILayout.Vector2IntField( "Custom Layout Grid Dimensions", spawn_group.custom_layout_grid_size );
            if( new_grid != spawn_group.custom_layout_grid_size )
            {
                spawn_group.custom_layout_grid_size = new_grid;
                EditorUtility.SetDirty( target );
            }
        }

        // spawn stagger times
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField( "Spawn Stagger Min Time" );
            float spawn_stagger_min_time = EditorGUILayout.FloatField( spawn_group.SpawnStaggerMinTime );
            if( spawn_stagger_min_time != spawn_group.SpawnStaggerMinTime )
            {
                if( spawn_stagger_min_time <= 0.0f )
                    spawn_stagger_min_time = 0.0f;
                spawn_group.SpawnStaggerMinTime = spawn_stagger_min_time;
                EditorUtility.SetDirty( target );
            }
            EditorGUILayout.EndHorizontal();
        }
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField( "Spawn Stagger Max Time" );
            float spawn_stagger_max_time = EditorGUILayout.FloatField( spawn_group.SpawnStaggerMaxTime );
            if( spawn_stagger_max_time != spawn_group.SpawnStaggerMaxTime )
            {
                if( spawn_stagger_max_time <= 0.0f )
                    spawn_stagger_max_time = 0.0f;
                spawn_group.SpawnStaggerMaxTime = spawn_stagger_max_time;
                EditorUtility.SetDirty( target );
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif

[Serializable] public class SpawnDictionary : SerializableDictionary<EnemyEnum, int> { }

#if UNITY_EDITOR
public class CustomSpawnLayoutEditor : EditorWindow
{
    private static GUIStyle label_style = new GUIStyle();
    private static readonly Rect default_node_rect = new Rect( 0, 0, 30, 30 );
    private static readonly Rect default_text_rect = new Rect( 0, 0, 300, 60 );

    private SpawnGroup spawn_group = null;
    private Rect play_area_rect = new Rect();
    private Rect cached_window_size = new Rect();
    private int dragging_index = -1;
    private bool in_snap_mode = false;

    public static void OpenWindow( SpawnGroup group )
    {
        CustomSpawnLayoutEditor window = GetWindow<CustomSpawnLayoutEditor>();
        window.spawn_group = group;
        window.titleContent = new GUIContent( $"{group.name} (CUSTOM LAYOUT)" );

        label_style.alignment = TextAnchor.UpperCenter;
        label_style.fontStyle = FontStyle.Bold;
        label_style.fontSize = 16;
        label_style.normal.textColor = Color.red;

        window.RefreshPlayAreaRect();
    }

    private void RefreshPlayAreaRect()
    {
        cached_window_size = position;
        float padding = 30.0f;
        float unit_length = Mathf.Min( position.width, position.height ) - padding;

        // roughly match the shape of a play area
        float height = unit_length;
        float width = unit_length * 0.66f;

        play_area_rect.position = new Vector2( ( position.width - width ) * 0.5f + ( padding / 2.0f ), ( padding / 2.0f ) );
        play_area_rect.width = width;
        play_area_rect.height = height;
    }

    public Vector2 ConvertNormalizedCoordinatesToWindowSpace( Vector2 normalized_pos )
    {
        Vector2 ret = new Vector2();
        ret.x = play_area_rect.x + play_area_rect.width * normalized_pos.x;
        ret.y = play_area_rect.y + play_area_rect.height * normalized_pos.y;
        return ret;
    }

    public Vector2 ConvertWindowSpacePositionToNormalized( Vector2 world_space_pos )
    {
        Vector2 ret = new Vector2();
        ret.x = ( world_space_pos.x - play_area_rect.x ) / play_area_rect.width;
        ret.y = ( world_space_pos.y - play_area_rect.y ) / play_area_rect.height;
        return ret;
    }

    public Vector2 ClampWindowSpacePositionToPlayArea( Vector2 position )
    {
        position.x = Mathf.Clamp( position.x, play_area_rect.x, play_area_rect.x + play_area_rect.width );
        position.y = Mathf.Clamp( position.y, play_area_rect.y, play_area_rect.y + play_area_rect.height );
        return position;
    }

    private void OnGUI()
    {
        DrawPlayArea();
        DrawNodes();

        ProcessEvents( Event.current );

        if( GUI.changed )
            Repaint();
    }

    private void DrawPlayArea()
    {
        float thickness = 0.5f;
        for( int x = 0; x < spawn_group.custom_layout_grid_size.x + 1; ++x )
        {
            float mul_factor = (float)x / (float)spawn_group.custom_layout_grid_size.x;

            // draw vertical lines
            Handles.DrawLine(
                new Vector2( play_area_rect.x + play_area_rect.width * mul_factor, play_area_rect.y ),
                new Vector2( play_area_rect.x + play_area_rect.width * mul_factor, play_area_rect.y + play_area_rect.height ),
                thickness );
        }

        for( int x = 0; x < spawn_group.custom_layout_grid_size.y + 1; ++x )
        {
            float mul_factor = (float)x / (float)spawn_group.custom_layout_grid_size.y;

            // draw horizontal lines
            Handles.DrawLine(
                new Vector2( play_area_rect.x, play_area_rect.y + play_area_rect.height * mul_factor ),
                new Vector2( play_area_rect.x + play_area_rect.width, play_area_rect.y + play_area_rect.height * mul_factor ),
                thickness );
        }
    }

    private void DrawNodes()
    {
        int index = 0;
        foreach( var kvp in spawn_group.SpawnMap )
        {
            for( int x = 0; x < kvp.Value; ++x )
            {
                Handles.DrawSolidDisc( ConvertNormalizedCoordinatesToWindowSpace( spawn_group.custom_layout_positions[index] ), Vector3.forward, 10.0f );
                ++index;
            }
        }

        index = 0;
        foreach( var kvp in spawn_group.SpawnMap )
        {
            for( int x = 0; x < kvp.Value; ++x )
            {
                Rect draw_rect = default_text_rect;
                draw_rect.center = ConvertNormalizedCoordinatesToWindowSpace( spawn_group.custom_layout_positions[index] );

                GUI.Label( draw_rect,
                kvp.Key.ToString() + x.ToString(),
                label_style );

                ++index;
            }
        }
    }

    private void ProcessEvents( Event e )
    {
        if( e.type == EventType.KeyDown && e.keyCode == KeyCode.LeftControl )
        {
            in_snap_mode = true;
        }
        else if( e.type == EventType.KeyUp && e.keyCode == KeyCode.LeftControl )
        {
            in_snap_mode = false;
        }

        for( int x = 0; x < spawn_group.custom_layout_positions.Count; ++x )
        {
            bool gui_changed = ProcessEventOnSpawnPosition( e, x );
            GUI.changed = GUI.changed || gui_changed;
        }
    }

    private bool ProcessEventOnSpawnPosition( Event e, int index )
    {
        Rect rect = default_node_rect;
        rect.center
            = ConvertNormalizedCoordinatesToWindowSpace( spawn_group.custom_layout_positions[index] );

        switch( e.type )
        {
            case EventType.MouseDown:
            {
                if( e.button == 0
                    && rect.Contains( e.mousePosition ) )
                {
                    dragging_index = index;
                    return true;
                }
                break;
            }
            case EventType.MouseUp:
            {
                if( dragging_index == index )
                    dragging_index = -1;
                break;
            }
            case EventType.MouseDrag:
            {
                if( e.button == 0
                    && dragging_index == index )
                {
                    rect.center = ClampWindowSpacePositionToPlayArea( e.mousePosition );
                    spawn_group.custom_layout_positions[index] =
                        ConvertWindowSpacePositionToNormalized( rect.center );
                    EditorUtility.SetDirty( spawn_group );

                    // allow for snap to grid
                    if( in_snap_mode )
                    {
                        Vector2 temp_position = spawn_group.custom_layout_positions[index];
                        temp_position.x *= spawn_group.custom_layout_grid_size.x;
                        temp_position.x = Mathf.Round( temp_position.x );
                        temp_position.x /= spawn_group.custom_layout_grid_size.x;
                        temp_position.y *= spawn_group.custom_layout_grid_size.y;
                        temp_position.y = Mathf.Round( temp_position.y );
                        temp_position.y /= spawn_group.custom_layout_grid_size.y;
                        spawn_group.custom_layout_positions[index] = temp_position;
                    }

                    e.Use();
                    return true;
                }
                break;
            }
        }

        return false;
    }

    private void Update()
    {
        // I hate that there is no resize event and I have to do this
        if( position != cached_window_size )
        {
            RefreshPlayAreaRect();
        }
    }
}
#endif