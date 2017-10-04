using UnityEngine;
using System.Collections;
using UnityEngine.Purchasing;
using System;
using UnityEngine.Events;

// Script robado de http://unity3d.com/es/learn/tutorials/topics/analytics/integrating-unity-iap-your-game-beta
// Modificado para las necesidades de ArtikFlow

namespace AFBase {
	
// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
public class Purchaser : MonoBehaviour, IStoreListener
{
	public static Purchaser instance;

	public class StringEvent : UnityEvent<string> { };
	[HideInInspector]
	public StringEvent eventPurchased = new StringEvent();

	private static IStoreController m_StoreController;                                                                  // Reference to the Purchasing system.
	private static IExtensionProvider m_StoreExtensionProvider;                                                         // Reference to store-specific Purchasing subsystems.

	// Product identifiers for all products capable of being purchased: "convenience" general identifiers for use with Purchasing, and their store-specific identifier counterparts 
	// for use with and outside of Unity Purchasing. Define store-specific identifiers also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

	private	ArtikProduct[] products;	// ArtikFlowConfiguration

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		products = ArtikFlowBase.instance.configuration.products;

		if(ArtikFlowBase.instance.configuration.storeTarget == ArtikFlowBaseConfiguration.StoreTarget.DEFAULT_STORE)
		{
			// If we haven't set up the Unity Purchasing reference
			if (m_StoreController == null)
			{
				// Begin to configure our connection to Purchasing
				InitializePurchasing();
			}
		}
		else if (ArtikFlowBase.instance.configuration.storeTarget == ArtikFlowBaseConfiguration.StoreTarget.PLAYPHONE)
		{
			PlayPhone.Billing.OnSuccess += (purchaseDetails) =>
			{
				foreach (ArtikProduct product in products)
				{
					if (product.playphoneID == purchaseDetails.ItemId)
					{
						onPurchased(product.productID);
						break;
					}
				}
			};
		}
    }

	private void Billing_OnSuccess(PlayPhone.Billing.PurchaseDetails obj)
	{
		throw new NotImplementedException();
	}

	public void InitializePurchasing()
	{
		// If we have already connected to Purchasing ...
		if (IsInitialized())
		{
			// ... we are done here.
			return;
		}

		// Create a builder, first passing in a suite of Unity provided stores.
		var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

		// Add the products
		foreach(ArtikProduct product in products)
			builder.AddProduct(product.productID, product.type, new IDs() { { product.appleID, AppleAppStore.Name }, { product.googleID, GooglePlay.Name } });

		UnityPurchasing.Initialize(this, builder);
	}

	public bool IsInitialized()
	{
		// Only say we are initialized if both the Purchasing references are set.
		return m_StoreController != null && m_StoreExtensionProvider != null;
	}

	public string localizedPrice(string productID)
	{
		if (!IsInitialized())
			return "...";

		Product product = m_StoreController.products.WithID(productID);
		return product.metadata.localizedPriceString;
	}

	public void BuyProductID(string productId)
	{
		if(ArtikFlowBase.instance.configuration.storeTarget == ArtikFlowBaseConfiguration.StoreTarget.DEFAULT_STORE)
		{
			// If the stores throw an unexpected exception, use try..catch to protect my logic here.
			try
			{
				// If Purchasing has been initialized ...
				if (IsInitialized())
				{
					// ... look up the Product reference with the general product identifier and the Purchasing system's products collection.
					Product product = m_StoreController.products.WithID(productId);

					// If the look up found a product for this device's store and that product is ready to be sold ... 
					if (product != null && product.availableToPurchase)
					{
						Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));// ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
						m_StoreController.InitiatePurchase(product);
					}
					// Otherwise ...
					else
					{
						// ... report the product look-up failure situation  
						Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
					}
				}
				// Otherwise ...
				else
				{
					// ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or retrying initiailization.
					Debug.Log("BuyProductID FAIL. Not initialized.");
				}
			}
			// Complete the unexpected exception handling ...
			catch (Exception e)
			{
				// ... by reporting any unexpected exception for later diagnosis.
				Debug.Log("BuyProductID: FAIL. Exception during purchase. " + e);
			}
		}
		else if(ArtikFlowBase.instance.configuration.storeTarget == ArtikFlowBaseConfiguration.StoreTarget.PLAYPHONE)
		{
			foreach(ArtikProduct product in products)
			{
				if(product.productID == productId)
				{
					PlayPhone.Billing.Purchase(product.playphoneID);
					break;
				}
			}

		}

	}


	// Restore purchases previously made by this customer. Some platforms automatically restore purchases. Apple currently requires explicit purchase restoration for IAP.
	public void RestorePurchases()
	{
		// If Purchasing has not yet been set up ...
		if (!IsInitialized())
		{
			// ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
			Debug.Log("RestorePurchases FAIL. Not initialized.");
			return;
		}

		// If we are running on an Apple device ... 
		if (Application.platform == RuntimePlatform.IPhonePlayer ||
			Application.platform == RuntimePlatform.OSXPlayer)
		{
			// ... begin restoring purchases
			Debug.Log("RestorePurchases started ...");

			// Fetch the Apple store-specific subsystem.
			var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
			// Begin the asynchronous process of restoring purchases. Expect a confirmation response in the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
			apple.RestoreTransactions((result) => {
				// The first phase of restoration. If no more responses are received on ProcessPurchase then no purchases are available to be restored.
				Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
			});
		}
		// Otherwise ...
		else
		{
			// We are not running on an Apple device. No work is necessary to restore purchases.
			Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
		}
	}

	//  
	// --- IStoreListener
	//

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		// Purchasing has succeeded initializing. Collect our Purchasing references.
		Debug.Log("OnInitialized: PASS");

		// Overall Purchasing system, configured with products for this application.
		m_StoreController = controller;
		// Store specific subsystem, for accessing device-specific store features.
		m_StoreExtensionProvider = extensions;
	}


	public void OnInitializeFailed(InitializationFailureReason error)
	{
		// Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
		Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
	}


	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
	{
		// A consumable product has been purchased by this user.
		onPurchased(args.purchasedProduct.definition.id);

		Debug.Log(string.Format("ProcessPurchase: Product: '{0}'", args.purchasedProduct.definition.id));

		return PurchaseProcessingResult.Complete;
	}


	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
	{
		// A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing this reason with the user.
		Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
	}

	// ---

	void onPurchased(string productID)
	{
		print("Bought: " + productID);

		eventPurchased.Invoke(productID);
	}

}

}