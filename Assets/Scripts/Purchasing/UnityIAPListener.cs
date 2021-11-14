using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class UnityIAPListener : IStoreListener
{
    public bool Initialized => controller != null && extensions != null;
    private IStoreController controller;
    private IExtensionProvider extensions;

    public UnityIAPListener()
    {
        var builder = ConfigurationBuilder.Instance( StandardPurchasingModule.Instance() );
        builder.AddProduct( "consumable", ProductType.Consumable );

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
    }

    /// <summary>
    /// Called when a purchase completes.
    /// May be called at any time after OnInitialized().
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase( PurchaseEventArgs purchaseEvent )
    {
        Debug.Log( $"UnityIAPListener: Purchase Processing {purchaseEvent}" );

        // TODO: Scan for the purchased product
        return PurchaseProcessingResult.Complete;

        //if( String.Equals( args.purchasedProduct.definition.id, kProductIDConsumable, StringComparison.Ordinal ) )
        //{

        //}
    }

    void BuyProductID( string productId )
    {
        // If Purchasing has been initialized ...
        if( Initialized )
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = controller.products.WithID( productId );

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if( product != null && product.availableToPurchase )
            {
                Debug.Log( string.Format( "Purchasing product asychronously: '{0}'", product.definition.id ) );
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                controller.InitiatePurchase( product );
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
                Debug.Log( "BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase" );
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.Log( "BuyProductID FAIL. Not initialized." );
        }
    }
}
