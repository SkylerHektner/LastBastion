using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum TestEnum
{
    Option1 = 1,
    Option2 = 2,
    Option3 = 4,
}

[System.Serializable]
public class ComplexClass
{
    public int IntField;
    public string StringField;
    public Sprite SpriteField;
}

[CreateAssetMenu( fileName = "TestObject", menuName = "ScriptableObjects/Testing/TestObject", order = 0 )]
[System.Serializable]
public class TestScriptable : ScriptableObject
{
    public TestScriptable TestObjectField;
    public int TestIntField;
    public int TestIntSliderField;
    public long TestLongField;
    public float TestFloatFlield;
    public float TestFloatSliderField;
    public double TestDoubleField;
    public string TestStringField;
    public string TestLargeStringField;
    public bool TestToggleField;
    public TestEnum TestEnumFlagsField;
    public TestEnum TestEnumField;
    public Gradient TestGradientField;
    public Color TestColorField;
    public Sprite TestSpriteField;
    public AudioClip TestAudioClipField;
    public Vector2 TestVector2Field;
    public Vector2Int TestVector2IntField;
    public Vector3 TestVector3Field;
    public Vector3Int TestVector3IntField;
    public Vector4 TestVector4Field;
    public AnimationCurve TestCurveField;
    public Bounds TestBoundsField;
    public BoundsInt TestBoundsIntField;
    public float TestMinSliderField;
    public float TestMaxSliderField;
    public Rect TestRectField;
    public RectInt TestRectIntField;
    public int TestLayerField;
    public string TestTagField;
    public List<int> TestSimpleListField;
    public List<ComplexClass> TestComplexListField;
}

#if UNITY_EDITOR
[CustomEditor( typeof( TestScriptable ) )]
public class TestScriptableEditor : ExtendedEditor<TestScriptable>
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var target = GetTarget();

        SectionHeader( "Generic Objects" );
        UnityObjectField( ref target.TestObjectField, false, "Test Object Field" );

        SectionHeader("Primitives");
        IntField( ref target.TestIntField, "Int Field" );
        IntSliderField( ref target.TestIntSliderField, 0, 100, "Test Int Slider Field" );
        LongField( ref target.TestLongField, "Test Long Field" );
        FloatField( ref target.TestFloatFlield, "Test Float Field" );
        FloatSliderField( ref target.TestFloatSliderField, 0.0f, 1.0f, "Test Float Slider Field" );
        DoubleField( ref target.TestDoubleField, "Test Double Field" );
        ToggleField( ref target.TestToggleField, "Test Toggle Field" );

        SectionHeader("Strings");
        StringField( ref target.TestStringField, "Test String Field" );
        LargeStringField( ref target.TestLargeStringField, "Test Large String Field" );

        SectionHeader("Enums + Foldouts");
        FoldoutArea( "Test Foldout Area - Enums", "Enum", () =>
        {
            EnumFlagsField( ref target.TestEnumFlagsField, "Test Enum Flags Field" );
            EnumField( ref target.TestEnumField, "Test Enum Field" );
        } );

        SectionHeader("Graphics");
        GradientField( ref target.TestGradientField, "Test Gradient Field" );
        ColorField( ref target.TestColorField, "Test Color Field" );

        SectionHeader("Utility");
        SpriteField( ref target.TestSpriteField, "Test Sprite Field" );
        AudioClipField( ref target.TestAudioClipField, "Test Audio Clip Field" );

        SectionHeader("Vectors + Toggle Areas");
        ToggleArea( "Test Toggle Area - Vectors", "Vectors", () =>
         {
             Vector2Field( ref target.TestVector2Field, "Test Vector2 Field" );
             Vector2IntField( ref target.TestVector2IntField, "Test Vector2Int Field" );
             Vector3Field( ref target.TestVector3Field, "Test Vector3 Field" );
             Vector3IntField( ref target.TestVector3IntField, "Test Vector3int Field" );
             Vector4Field( ref target.TestVector4Field, "Test Vector4 Field" );
         } );

        SectionHeader("Exotic");
        CurveField( ref target.TestCurveField, "Test Curve Field" );
        BoundsField( ref target.TestBoundsField, "Test Bounds Field" );
        BoundsIntField( ref target.TestBoundsIntField, "Test Bounds Int Field" );
        MinMaxSliderField( ref target.TestMinSliderField, ref target.TestMaxSliderField, 0.0f, 10.0f, "Test Min Max Slider Field" );
        RectField( ref target.TestRectField, "Test Rect Field" );
        RectIntField( ref target.TestRectIntField, "Test Rect Int Field" );

        SectionHeader("Physics");
        LayerField( ref target.TestLayerField, "Test Layer Field" );
        TagField( ref target.TestTagField, "Test Tag Field" );

        SectionHeader("Dynamic Lists");
        ListField<int>( target.TestSimpleListField, "Test Simple List Field", "Simple List", ( int index ) =>
         {
             int val = target.TestSimpleListField[index];
             IntField( ref val );
             target.TestSimpleListField[index] = val;
         },
        () =>
        {
            return 0;
        } );

        Seperator();
        ListField<ComplexClass>( target.TestComplexListField, "Test Complex List Field", "Complex List", 
            ( int index ) =>
         {
             var val = target.TestComplexListField[index];
             IntField( ref val.IntField, "Int Field" );
             StringField( ref val.StringField, "String Field" );
             SpriteField( ref val.SpriteField, "Sprite Field" );
         },
        () =>
        {
            return new ComplexClass();
        },
        (int index) => 
        {
            return $"Complex Class Entry {index}";
        });
    }
}
#endif