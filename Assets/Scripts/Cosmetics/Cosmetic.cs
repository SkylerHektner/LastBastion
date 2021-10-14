using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

public class CosmeticDisplayInterface : ScriptableObject
{
    public virtual string GetName() { return null; }
    public virtual string GetDescription() { return null; }
    public virtual float GetPrice() { return 0.0f; }
    public virtual void ApplyUnlocks() { }
}

[CreateAssetMenu( fileName = "Cosmetic", menuName = "ScriptableObjects/Cosmetic", order = 0 )]
public class Cosmetic : CosmeticDisplayInterface
{
    public string Name;
    public string Description;
    public UnlockFlag unlock_flag;
    public CosmeticCategory category;
    public float Price;

    public AnimatorOverrideController override_controller;
    public Sprite sprite;
    public Material material;

    public override void ApplyUnlocks()
    {
        PD.Instance.UnlockMap.Set( unlock_flag, true, false );
        PD.Instance.UnlockMap.Set( unlock_flag, true, true );
    }

    public override string GetDescription()
    {
        return Description;
    }

    public override string GetName()
    {
        return Name;
    }

    public override float GetPrice()
    {
        return Price;
    }
}

#if UNITY_EDITOR
[CustomEditor( typeof( Cosmetic ) )]
public class CosmeticEditor : ExtendedEditor<Cosmetic>
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var target = GetTarget();

        LabelField( "Reminder: Cosmetics must be added to Global Data" );

        StringField( ref target.Name, "Name" );
        StringField( ref target.Description, "Description" );
        FloatField( ref target.Price, "Price" );
        EnumField( ref target.unlock_flag, "Unlock Flag" );
        EnumField( ref target.category, "Category" );

        switch( target.category )
        {
            case CosmeticCategory.LaunchArrow:
            {
                UnityObjectField( ref target.sprite, false, "Launch Arrow Sprite" );
            }
            break;
            case CosmeticCategory.SawTrail:
            {
                UnityObjectField( ref target.material, false, "Particle Emitter Material" );
            }
            break;
            case CosmeticCategory.SawSkin:
            {
                UnityObjectField( ref target.override_controller, false, "Saw Animator Override" );
            }
            break;
        }
    }
}
#endif

public enum CosmeticCategory
{
    LaunchArrow,
    SawTrail,
    SawSkin,
}