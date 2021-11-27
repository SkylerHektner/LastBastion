using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu( fileName = "CosmeticBundle", menuName = "ScriptableObjects/CosmeticBundle", order = 0 )]
public class CosmeticBundle : CosmeticDisplayInterface
{
    public string Name;
    public string Description;
    public float Price;
    public List<Cosmetic> Cosmetics = new List<Cosmetic>();
    public bool Premium;
    public string ProductID;

    public override void ApplyUnlocks()
    {
        foreach( Cosmetic cosmetic in Cosmetics )
        {
            PD.Instance.UnlockMap.Set( cosmetic.unlock_flag, true, false );
            PD.Instance.UnlockMap.Set( cosmetic.unlock_flag, true, true );
        }
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

    public override bool GetIsPremium()
    {
        return Premium;
    }

    public override string GetProductID()
    {
        return ProductID;
    }

    public override bool IsUnlocked()
    {
        bool unlocked = true;
        foreach(Cosmetic cosmetic in Cosmetics)
        {
            unlocked = unlocked && PD.Instance.UnlockMap.Get( cosmetic.unlock_flag, false );
        }
        return unlocked;
    }
}

#if UNITY_EDITOR
[CustomEditor( typeof( CosmeticBundle ) )]
public class CosmeticBundleEditor : ExtendedEditor<CosmeticBundle>
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var target = GetTarget();

        LabelField( "Reminder: Cosmetic Bundles must be added to Global Data" );

        StringField( ref target.Name, "Name" );
        StringField( ref target.Description, "Description" );
        FloatField( ref target.Price, "Price" );
        ToggleField( ref target.Premium, "Premium" );
        if( target.Premium )
        {
            StringField( ref target.ProductID, "Product ID" );
        }

        ListField( target.Cosmetics, "Cosmetics", "Cosmetics", ( int index ) =>
        {
            var val = target.Cosmetics[index];
            UnityObjectField( ref val, false );
            target.Cosmetics[index] = val;
        },
        () =>
        {
            return null;
        } );
    }
}
#endif