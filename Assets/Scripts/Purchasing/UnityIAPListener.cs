using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Events;
using System;

public class UnityIAPListener : IStoreListener
{
    public bool Initialized => controller != null && extensions != null;
    private IStoreController controller;
    private IExtensionProvider extensions;
    public UnityEvent PurchaseCompletedEvent = new UnityEvent();
    public UnityEvent PurchaseFailedEvent = new UnityEvent();

    public UnityIAPListener()
    {
        var builder = ConfigurationBuilder.Instance( StandardPurchasingModule.Instance() );

        foreach( Cosmetic cosmetic in Spectator.Instance.GD.Cosmetics )
        {
            if( !cosmetic.BelongsToBundle && cosmetic.Premium )
            {
                Debug.Assert( !String.IsNullOrEmpty( cosmetic.ProductID ), $"ERROR! Cosmetic {cosmetic.name} is premium but has no product ID!" );
                builder.AddProduct( cosmetic.ProductID, ProductType.NonConsumable );
            }
        }
        foreach( CosmeticBundle bundle in Spectator.Instance.GD.CosmeticBundles )
        {
            if( bundle.Premium )
            {
                Debug.Assert( !String.IsNullOrEmpty( bundle.ProductID ), $"ERROR! Cosmetic Bundle {bundle.name} is premium but has no product ID!" );
                builder.AddProduct( bundle.ProductID, ProductType.NonConsumable );

            }
        }

        UnityPurchasing.Initialize( this, builder );
    }

    /// <summary>
    /// Called when Unity IAP is ready to make purchases.
    /// </summary>
    public void OnInitialized( IStoreController controller, IExtensionProvider extensions )
    {
        Debug.Log( "UnityIAPListener: Initialized" );
        this.controller = controller;
        this.extensions = extensions;
    }

    /// <summary>
    /// Called when Unity IAP encounters an unrecoverable initialization error.
    ///
    /// Note that this will not be called if Internet is unavailable; Unity IAP
    /// will attempt initialization until it becomes available.
    /// </summary>
    public void OnInitializeFailed( InitializationFailureReason error )
    {
        Debug.LogWarning( $"UnityIAPListener: Initialized FAILED {error}" );
    }


    /// /// <summary>
    /// Called when a purchase fails.
    /// </summary>
    public void OnPurchaseFailed( Product product, PurchaseFailureReason failureReason )
    {
        Debug.Log( $"UnityIAPListener: Purchase Failed {product} for {failureReason}" );
        PurchaseFailedEvent.Invoke();
    }

    /// <summary>
    /// Called when a purchase completes.
    /// May be called at any time after OnInitialized().
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase( PurchaseEventArgs purchaseEvent )
    {
        Debug.Log( $"UnityIAPListener: Purchase Processing {purchaseEvent}" );

        foreach( Cosmetic cosmetic in Spectator.Instance.GD.Cosmetics )
        {
            if( cosmetic.Premium && String.Equals( purchaseEvent.purchasedProduct.definition.id, cosmetic.ProductID, StringComparison.Ordinal ) )
            {
                cosmetic.ApplyUnlocks();
            }
        }
        foreach( CosmeticBundle bundle in Spectator.Instance.GD.CosmeticBundles )
        {
            if( bundle.Premium && String.Equals( purchaseEvent.purchasedProduct.definition.id, bundle.ProductID, StringComparison.Ordinal ) )
            {
                bundle.ApplyUnlocks();
            }
        }

        PurchaseCompletedEvent.Invoke();
        return PurchaseProcessingResult.Complete;
    }

    public void BuyProductID( string productId )
    {
        if( Initialized )
        {
            Product product = controller.products.WithID( productId );

            if( product != null && product.availableToPurchase )
            {
                Debug.Log( $"Purchasing product asychronously: {product.definition.id}" );
                // Expect a response either through ProcessPurchase or OnPurchaseFailed async
                controller.InitiatePurchase( product );
            }
            else
            {
                Debug.LogError( $"BuyProductID {productId} FAILED. either is not found or is not available for purchase" );
            }
        }
        else
        {
            Debug.LogError( $"BuyProductID {productId} FAILED. Not initialized." );
        }
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        throw new NotImplementedException();
    }
}
