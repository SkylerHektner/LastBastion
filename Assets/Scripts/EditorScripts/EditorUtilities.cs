using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public static class CustomEditorUtilities
{
    public static void AutoDirtyLabeledInt( ref int original_val, string label, UnityEngine.Object target )
    {
        int new_value = EditorGUILayout.IntField( label, original_val );
        if( new_value != original_val )
        {
            original_val = new_value;
            EditorUtility.SetDirty( target );
        }
    }

    public static void AutoDirtyLabeledBool( ref bool original_val, string label, UnityEngine.Object target )
    {
        bool new_value = EditorGUILayout.Toggle( label, original_val );
        if( new_value != original_val )
        {
            original_val = new_value;
            EditorUtility.SetDirty( target );
        }
    }

    public static void AutoDirtyLabeledFloat( ref float original_val, string label, UnityEngine.Object target )
    {
        float new_value = EditorGUILayout.FloatField( label, original_val );
        if( new_value != original_val )
        {
            original_val = new_value;
            EditorUtility.SetDirty( target );
        }
    }

    public static void AutoDirtyLabeledString( ref string original_val, string label, bool use_large_entry, UnityEngine.Object target )
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField( label );
        string new_value = use_large_entry ? EditorGUILayout.TextField( original_val ) : EditorGUILayout.TextArea( original_val );
        if( new_value != original_val )
        {
            original_val = new_value;
            EditorUtility.SetDirty( target );
        }
        EditorGUILayout.EndHorizontal();
    }

    public static void AutoDirtyFoldoutHeaderGroup( ref bool original_val, string label, UnityEngine.Object target )
    {
        Debug.Assert( label != null );

        bool new_value = EditorGUILayout.BeginFoldoutHeaderGroup( original_val, label );

        if( new_value != original_val )
        {
            original_val = new_value;
            EditorUtility.SetDirty( target );
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    public static UnityEngine.Object AutoDirtyUnityObject( UnityEngine.Object original_val, System.Type type, string label, UnityEngine.Object target )
    {
        UnityEngine.Object new_value = EditorGUILayout.ObjectField( label, original_val, type, false );
        if( new_value != original_val )
        {
            EditorUtility.SetDirty( target );
            return new_value;
        }
        return original_val;
    }

    public static void ListItemControlButtonsUnsafe<T>( List<T> list, ref int index, UnityEngine.Object target )
    {
        EditorGUILayout.BeginHorizontal();

        if( GUILayout.Button( "Delete" ) )
        {
            list.RemoveAt( index );
            index--;
            EditorUtility.SetDirty( target );
        }
        if( list.Count > 1 && index > 0 && GUILayout.Button( "Move Up" ) )
        {
            T original = list[index];
            list[index] = list[index - 1];
            list[index - 1] = original;
            EditorUtility.SetDirty( target );
        }
        if( list.Count > 1 && index < list.Count - 1 && GUILayout.Button( "Move Down" ) )
        {
            T original = list[index];
            list[index] = list[index + 1];
            list[index + 1] = original;
            EditorUtility.SetDirty( target );
        }

        EditorGUILayout.EndHorizontal();
    }

    public static void ListItemControlButtons<T>( List<T> list, ref int index, ref bool confirm_delete_bool, UnityEngine.Object target )
    {
        EditorGUILayout.BeginHorizontal();
        if( !confirm_delete_bool )
        {
            bool new_confirm_delete_bool = GUILayout.Button( "Delete" );
            if( new_confirm_delete_bool != confirm_delete_bool )
            {
                confirm_delete_bool = new_confirm_delete_bool;
            }
        }
        else
        {
            EditorGUILayout.LabelField( "Are you sure?" );
            if( GUILayout.Button( "Yes" ) )
            {
                list.RemoveAt( index );
                index--;
                EditorUtility.SetDirty( target );
            }
            else if( GUILayout.Button( "No" ) )
            {
                confirm_delete_bool = false;
            }
        }

        if( list.Count > 1 && index > 0 && GUILayout.Button( "Move Up" ) )
        {
            T original = list[index];
            list[index] = list[index - 1];
            list[index - 1] = original;
            EditorUtility.SetDirty( target );
        }
        if( list.Count > 1 && index < list.Count - 1 && GUILayout.Button( "Move Down" ) )
        {
            T original = list[index];
            list[index] = list[index + 1];
            list[index + 1] = original;
            EditorUtility.SetDirty( target );
        }

        EditorGUILayout.EndHorizontal();
    }

}
#endif

public class ReadOnlyAttribute : PropertyAttribute
{

}

#if UNITY_EDITOR
[CustomPropertyDrawer( typeof( ReadOnlyAttribute ) )]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight( SerializedProperty property,
                                            GUIContent label )
    {
        return EditorGUI.GetPropertyHeight( property, label, true );
    }

    public override void OnGUI( Rect position,
                               SerializedProperty property,
                               GUIContent label )
    {
        GUI.enabled = false;
        EditorGUI.PropertyField( position, property, label, true );
        GUI.enabled = true;
    }
}
#endif