using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

#if UNITY_EDITOR
public class ExtendedEditor<T> : Editor where T : UnityEngine.Object
{
    protected T GetTarget()
    {
        return (T)target;
    }

    // generic
    protected void UnityObjectField<V>( ref V value, bool allowSceneObjects, string label = null )
        where V : UnityEngine.Object
    {
        UnityEngine.Object new_value;
        if( label == null )
            new_value = EditorGUILayout.ObjectField( value, typeof( V ), allowSceneObjects );
        else
            new_value = EditorGUILayout.ObjectField( label, value, typeof( V ), allowSceneObjects );
        if( new_value != value )
        {
            value = (V)new_value;
            EditorUtility.SetDirty( target );
        }
    }

    // primitives
    protected void IntField( ref int value, string label = null )
    {
        var new_value = value;
        if( label == null )
            new_value = EditorGUILayout.IntField( value );
        else
            new_value = EditorGUILayout.IntField( label, value );
        if( new_value != value )
        {
            value = new_value;
            EditorUtility.SetDirty( target );
        }
    }
    protected void IntSliderField( ref int value, int min, int max, string label = null )
    {
        var new_value = value;
        if( label == null )
            new_value = EditorGUILayout.IntSlider( value, min, max );
        else
            new_value = EditorGUILayout.IntSlider( label, value, min, max );
        if( new_value != value )
        {
            value = new_value;
            EditorUtility.SetDirty( target );
        }
    }
    protected void LongField( ref long value, string label = null )
    {
        var new_value = value;
        if( label == null )
            new_value = EditorGUILayout.LongField( value );
        else
            new_value = EditorGUILayout.LongField( label, value );
        if( new_value != value )
        {
            value = new_value;
            EditorUtility.SetDirty( target );
        }
    }
    protected void FloatField( ref float value, string label = null )
    {
        var new_value = value;
        if( label == null )
            new_value = EditorGUILayout.FloatField( value );
        else
            new_value = EditorGUILayout.FloatField( label, value );
        if( new_value != value )
        {
            value = new_value;
            EditorUtility.SetDirty( target );
        }
    }
    protected void FloatSliderField( ref float value, float min, float max, string label = null )
    {
        var new_value = value;
        if( label == null )
            new_value = EditorGUILayout.Slider( value, min, max );
        else
            new_value = EditorGUILayout.Slider( label, value, min, max );
        if( new_value != value )
        {
            value = new_value;
            EditorUtility.SetDirty( target );
        }
    }
    protected void DoubleField( ref double value, string label = null )
    {
        var new_value = value;
        if( label == null )
            new_value = EditorGUILayout.DoubleField( value );
        else
            new_value = EditorGUILayout.DoubleField( label, value );
        if( new_value != value )
        {
            value = new_value;
            EditorUtility.SetDirty( target );
        }
    }
    protected void StringField( ref string value, string label = null )
    {
        var new_value = value;
        if( label == null )
            new_value = EditorGUILayout.TextField( value );
        else
            new_value = EditorGUILayout.TextField( label, value );
        if( new_value != value )
        {
            value = new_value;
            EditorUtility.SetDirty( target );
        }
    }
    protected void LargeStringField( ref string value, string label = null )
    {
        GUILayout.BeginHorizontal();
        if( label != null )
            GUILayout.Label( label );
        var style = GUI.skin.textArea;
        style.wordWrap = true;
        var new_value = EditorGUILayout.TextArea( value, style );
        if( new_value != value )
        {
            value = new_value;
            EditorUtility.SetDirty( target );
        }
        GUILayout.EndHorizontal();
    }
    protected void ToggleField( ref bool value, string label = null )
    {
        var new_value = value;
        if( label == null )
            new_value = EditorGUILayout.Toggle( value );
        else
            new_value = EditorGUILayout.Toggle( label, value );
        if( new_value != value )
        {
            value = new_value;
            EditorUtility.SetDirty( target );
        }
    }

    // enum
    protected void EnumFlagsField<E>( ref E value, string label = null )
        where E : Enum
    {
        Enum new_value;
        if( label == null )
            new_value = EditorGUILayout.EnumFlagsField( value );
        else
            new_value = EditorGUILayout.EnumFlagsField( label, value );
        if( new_value != (Enum)value )
        {
            value = (E)new_value;
            EditorUtility.SetDirty( target );
        }
    }
    protected void EnumField<E>( ref E value, string label = null )
        where E : Enum
    {
        Enum new_value;
        if( label == null )
            new_value = EditorGUILayout.EnumPopup( value );
        else
            new_value = EditorGUILayout.EnumPopup( label, value );
        if( new_value != (Enum)value )
        {
            value = (E)new_value;
            EditorUtility.SetDirty( target );
        }
    }

    // graphical
    protected void GradientField( ref Gradient value, string label = null )
    {
        var new_value = value;
        if( label == null )
            new_value = EditorGUILayout.GradientField( value );
        else
            new_value = EditorGUILayout.GradientField( label, value );
        if( new_value != value )
        {
            value = new_value;
            EditorUtility.SetDirty( target );
        }
    }
    protected void ColorField( ref Color value, string label = null )
    {
        var new_value = value;
        if( label == null )
            new_value = EditorGUILayout.ColorField( value );
        else
            new_value = EditorGUILayout.ColorField( label, value );
        if( new_value != value )
        {
            value = new_value;
            EditorUtility.SetDirty( target );
        }
    }

    // Common unity object fields
    protected void SpriteField( ref Sprite value, string label = null )
    {
        var new_value = value;
        if( label == null )
            new_value = (Sprite)EditorGUILayout.ObjectField( value, typeof( Sprite ), false );
        else
            new_value = (Sprite)EditorGUILayout.ObjectField( label, value, typeof( Sprite ), false );
        if( new_value != value )
        {
            value = new_value;
            EditorUtility.SetDirty( target );
        }
    }
    protected void AudioClipField( ref AudioClip value, string label = null )
    {
        var new_value = value;
        if( label == null )
            new_value = (AudioClip)EditorGUILayout.ObjectField( value, typeof( AudioClip ), false );
        else
            new_value = (AudioClip)EditorGUILayout.ObjectField( label, value, typeof( AudioClip ), false );
        if( new_value != value )
        {
            value = new_value;
            EditorUtility.SetDirty( target );
        }
    }

    // vector
    protected void Vector2Field( ref Vector2 value, string label )
    {
        var new_value = EditorGUILayout.Vector2Field( label, value );
        if( new_value != value )
        {
            value = new_value;
            EditorUtility.SetDirty( target );
        }
    }
    protected void Vector2IntField( ref Vector2Int value, string label )
    {
        var new_value = EditorGUILayout.Vector2IntField( label, value );
        if( new_value != value )
        {
            value = new_value;
            EditorUtility.SetDirty( target );
        }
    }
    protected void Vector3Field( ref Vector3 value, string label )
    {
        var new_value = EditorGUILayout.Vector3Field( label, value );
        if( new_value != value )
        {
            value = new_value;
            EditorUtility.SetDirty( target );
        }
    }
    protected void Vector3IntField( ref Vector3Int value, string label )
    {
        var new_value = EditorGUILayout.Vector3IntField( label, value );
        if( new_value != value )
        {
            value = new_value;
            EditorUtility.SetDirty( target );
        }
    }
    protected void Vector4Field( ref Vector4 value, string label )
    {
        var new_value = EditorGUILayout.Vector4Field( label, value );
        if( new_value != value )
        {
            value = new_value;
            EditorUtility.SetDirty( target );
        }
    }

    // exotic
    protected void CurveField( ref AnimationCurve value, string label = null )
    {
        var new_value = value;
        if( label == null )
            new_value = EditorGUILayout.CurveField( value );
        else
            new_value = EditorGUILayout.CurveField( label, value );
        if( new_value != value )
        {
            value = new_value;
            EditorUtility.SetDirty( target );
        }
    }
    protected void BoundsField( ref Bounds value, string label = null )
    {
        var new_value = value;
        if( label == null )
            new_value = EditorGUILayout.BoundsField( value );
        else
            new_value = EditorGUILayout.BoundsField( label, value );
        if( new_value != value )
        {
            value = new_value;
            EditorUtility.SetDirty( target );
        }
    }
    protected void BoundsIntField( ref BoundsInt value, string label = null )
    {
        var new_value = value;
        if( label == null )
            new_value = EditorGUILayout.BoundsIntField( value );
        else
            new_value = EditorGUILayout.BoundsIntField( label, value );
        if( new_value != value )
        {
            value = new_value;
            EditorUtility.SetDirty( target );
        }
    }
    protected void MinMaxSliderField( ref float minValue, ref float maxValue, float minLimit, float maxLimit, string label = null )
    {
        float oldMin = minValue;
        float oldMax = maxLimit;
        if( label == null )
            EditorGUILayout.MinMaxSlider( ref minValue, ref maxValue, minLimit, maxLimit );
        else
            EditorGUILayout.MinMaxSlider( label, ref minValue, ref maxValue, minLimit, maxLimit );
        if( oldMin != minValue || oldMax != maxValue )
        {
            EditorUtility.SetDirty( target );
        }
    }
    protected void RectField( ref Rect value, string label = null )
    {
        var new_value = value;
        if( label == null )
            new_value = EditorGUILayout.RectField( value );
        else
            new_value = EditorGUILayout.RectField( label, value );
        if( new_value != value )
        {
            value = new_value;
            EditorUtility.SetDirty( target );
        }
    }
    protected void RectIntField( ref RectInt value, string label = null )
    {
        var new_value = value;
        if( label == null )
            new_value = EditorGUILayout.RectIntField( value );
        else
            new_value = EditorGUILayout.RectIntField( label, value );
        if( !new_value.Equals( value ) )
        {
            value = new_value;
            EditorUtility.SetDirty( target );
        }
    }

    // physics
    protected void LayerField( ref int value, string label = null )
    {
        var new_value = value;
        if( label == null )
            new_value = EditorGUILayout.LayerField( value );
        else
            new_value = EditorGUILayout.LayerField( label, value );
        if( !new_value.Equals( value ) )
        {
            value = new_value;
            EditorUtility.SetDirty( target );
        }
    }
    protected void TagField( ref string value, string label = null )
    {
        var new_value = value;
        if( label == null )
            new_value = EditorGUILayout.TagField( value );
        else
            new_value = EditorGUILayout.TagField( label, value );
        if( !new_value.Equals( value ) )
        {
            value = new_value;
            EditorUtility.SetDirty( target );
        }
    }

    // complex
    protected void ListField<L>( List<L> value, string label, string UniqueID, Action<int> RenderItemFunction, Func<L> NewEntryFunction, Func<int, string> GetItemFoldoutLabelFunction = null /*if non null entries will be sorted under respective foldout sections*/ )
    {
        SectionHeader( label );

        // list management buttons
        GUILayout.BeginHorizontal();
        if( GUILayout.Button( "New Entry" ) )
        {
            value.Add( NewEntryFunction() );
            EditorUtility.SetDirty( target );
        }
        string ConfirmClearID = UniqueID + "Clear";
        if( GetElementState( ConfirmClearID ) )
        {
            if( GUILayout.Button( "Confirm Clear" ) )
            {
                value.Clear();
                EditorUtility.SetDirty( target );
                ElementStateTrackers[ConfirmClearID] = false;
            }
            else if( GUILayout.Button( "Cancel" ) )
            {
                ElementStateTrackers[ConfirmClearID] = false;
            }
        }
        else if( GUILayout.Button( "Clear List" ) )
        {
            ElementStateTrackers[ConfirmClearID] = true;
        }
        if( GetItemFoldoutLabelFunction != null )
        {
            if( GUILayout.Button( "+", EditorStyles.miniButtonLeft, miniButtonWidth ) )
            {
                for( int x = 0; x < value.Count; ++x )
                {
                    string CollapseEntryID = UniqueID + "CollapseEntry" + x.ToString();
                    ElementStateTrackers[CollapseEntryID] = true;
                }
            }
            if( GUILayout.Button( "-", EditorStyles.miniButtonLeft, miniButtonWidth ) )
            {
                for( int x = 0; x < value.Count; ++x )
                {
                    string CollapseEntryID = UniqueID + "CollapseEntry" + x.ToString();
                    ElementStateTrackers[CollapseEntryID] = false;
                }
            }
        }
        GUILayout.EndHorizontal();

        // render entries
        EditorGUI.indentLevel++;
        for( int x = 0; x < value.Count; ++x )
        {
            GUILayout.BeginHorizontal();

            // show foldout header if function for label not null
            if( GetItemFoldoutLabelFunction != null )
            {
                string CollapseEntryID = UniqueID + "CollapseEntry" + x.ToString();
                bool show = EditorGUILayout.BeginFoldoutHeaderGroup( GetElementState( CollapseEntryID ), GetItemFoldoutLabelFunction( x ) );
                ElementStateTrackers[CollapseEntryID] = show;
                RenderListItemControlButtons( value, x, UniqueID, NewEntryFunction );
                GUILayout.EndHorizontal();

                if( !GetElementState( CollapseEntryID ) )
                {
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    continue;
                }
            }

            // render item
            RenderItemFunction( x );

            // end header group or horizontal
            if( GetItemFoldoutLabelFunction != null )
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
            else
            {
                RenderListItemControlButtons( value, x, UniqueID, NewEntryFunction );
                GUILayout.EndHorizontal();
            }
        }
        EditorGUI.indentLevel--;
    }
    private void RenderListItemControlButtons<L>( List<L> value, int index, string UniqueID, Func<L> NewEntryFunction )
    {
        // list item management buttons
        string ConfirmDeleteEntryID = UniqueID + "DeleteEntry" + index.ToString();
        if( GUILayout.Button( "\u2191", EditorStyles.miniButtonLeft, miniButtonWidth ) && index > 0 )
        {
            var temp = value[index];
            value[index] = value[index - 1];
            value[index - 1] = temp;
            string ConfirmDeleteEntryIDPrev = UniqueID + "DeleteEntry" + index.ToString();
            ElementStateTrackers[ConfirmDeleteEntryID] = false;
            ElementStateTrackers[ConfirmDeleteEntryIDPrev] = false;
            EditorUtility.SetDirty( target );
        }
        if( GUILayout.Button( "\u2193", EditorStyles.miniButtonMid, miniButtonWidth ) && index < value.Count - 1 )
        {
            var temp = value[index];
            value[index] = value[index + 1];
            value[index + 1] = temp;
            string ConfirmDeleteEntryIDNext = UniqueID + "DeleteEntry" + index.ToString();
            ElementStateTrackers[ConfirmDeleteEntryID] = false;
            ElementStateTrackers[ConfirmDeleteEntryIDNext] = false;
            EditorUtility.SetDirty( target );
        }
        if( GUILayout.Button( "\u21b4", EditorStyles.miniButtonRight, miniButtonWidth ) )
        {
            value.Insert( index + 1, value[index] );
            EditorUtility.SetDirty( target );
        }
        if( GetElementState( ConfirmDeleteEntryID ) )
        {
            if( GUILayout.Button( "Confirm", EditorStyles.miniButton, smallButtonWidth ) )
            {
                value.RemoveAt( index );
                index--;
                EditorUtility.SetDirty( target );
                ElementStateTrackers[ConfirmDeleteEntryID] = false;
            }
            else if( GUILayout.Button( "Cancel", EditorStyles.miniButton, smallButtonWidth ) )
            {
                ElementStateTrackers[ConfirmDeleteEntryID] = false;
            }
        }
        else if( GUILayout.Button( "X", EditorStyles.miniButton, smallButtonWidth ) )
        {
            ElementStateTrackers[ConfirmDeleteEntryID] = true;
        }
    }

    // formatting
    protected void SectionHeader( string label )
    {
        Seperator();
        var labelLayout = GUI.skin.label;
        labelLayout.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label( "---------- " + label + " ----------", labelLayout );
        Seperator();
    }
    protected void Seperator()
    {
        EditorGUILayout.Separator();
    }
    protected void FoldoutArea( string label, string UniqueID, Action RenderContents )
    {
        bool show = EditorGUILayout.BeginFoldoutHeaderGroup( GetElementState( UniqueID ), label );
        if( show )
        {
            EditorGUI.indentLevel++;
            RenderContents();
            EditorGUI.indentLevel--;
        }
        ElementStateTrackers[UniqueID] = show;
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
    protected void ToggleArea( string label, string UniqueID, Action RenderContents )
    {
        bool show = EditorGUILayout.BeginToggleGroup( label, GetElementState( UniqueID ) );
        if( show )
        {
            EditorGUI.indentLevel++;
            RenderContents();
            EditorGUI.indentLevel--;
        }
        ElementStateTrackers[UniqueID] = show;
        EditorGUILayout.EndToggleGroup();
    }

    protected void LabelField(string label)
    {
        EditorGUILayout.LabelField( label );
    }

    // private stuff
    private Dictionary<string, bool> ElementStateTrackers = new Dictionary<string, bool>();
    private static GUILayoutOption miniButtonWidth = GUILayout.Width( 20f );
    private static GUILayoutOption smallButtonWidth = GUILayout.Width( 80f );
    private T old_target;
    private bool GetElementState( string UniqueID )
    {
        if( !ElementStateTrackers.TryGetValue( UniqueID, out bool val ) )
        {
            val = false;
            ElementStateTrackers.Add( UniqueID, val );
        }
        return val;
    }

    public override void OnInspectorGUI()
    {
        if(old_target != GetTarget())
        {
            ElementStateTrackers.Clear();
            old_target = GetTarget();
        }
    }
}
#endif
