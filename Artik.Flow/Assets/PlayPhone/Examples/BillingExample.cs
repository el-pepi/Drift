using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class BillingExample : ExampleScreen
{
	private readonly List<PlayPhone.Billing.PurchaseDetails> restoredPurchaseDetails = 
									new List<PlayPhone.Billing.PurchaseDetails>();

	private string lastPurchase = "EMPTY";

	void Start ()
	{
		PlayPhone.Billing.OnSuccess += (purchaseDetails) => { 
			SetStatus("Purchased: " + purchaseDetails.Name + " (" + purchaseDetails.OrderId + ")");
//			var names = (from p in restoredPurchaseDetails select string.Format("{0}(id={1})", p.Name, p.ItemId)).ToArray();
			lastPurchase = purchaseDetails.OrderId;
		};
		PlayPhone.Billing.OnError += (error) => { 
			SetStatus("Error: " + error); 
		};
		PlayPhone.Billing.OnCancel += () => { 
			SetStatus("Canceled"); 
		};
		PlayPhone.Billing.OnRestore += (purchaseDetails) => {
			restoredPurchaseDetails.Add(purchaseDetails);
//			var names = (from p in restoredPurchaseDetails select string.Format("{0}(id={1})", p.Name, p.ItemId)).ToArray();
			SetStatus("Restored: " + purchaseDetails.Name + " (" + purchaseDetails.OrderId + ")");
			lastPurchase = purchaseDetails.OrderId;
		};
		PlayPhone.Billing.OnConsumeSuccess += (orderId) => {
			SetStatus("Consumed: " + orderId);
		};
		PlayPhone.Billing.OnConsumeError += (orderId, error) => {
			SetStatus("Consume Failed: " + orderId + " (" + error + ")");
		};
		PlayPhone.Billing.OnSubscriptions += (json) => {
			var list = PlayPhone.MiniJSON.Json.Deserialize(json) as IList;

			if (list == null || list.Count == 0)
			{
				SetStatus("No subscriptions to restore");
			}
			else
			{
				var subscriptions = list.Cast<Dictionary<string, object>>();
				var names = (from s in subscriptions select s["item_name"].ToString()).ToArray();
				SetStatus("Subscriptions restored: " + string.Join(", ", names)); 
			}
		};
		PlayPhone.Billing.OnSubscriptionsError += (error) => {
			SetStatus("Subscriptions error: " + error);
		};
		
		PlayPhone.Plugin.DoLaunchAction(true);
	}

	public override void Draw()
	{
		if (GUILayout.Button("Get Platform Id"))
		{
			SetStatus("PlatformId: " + PlayPhone.Plugin.getPlatformId());
		}
		if (GUILayout.Button("Purchase Item 1 (portal ID 110)"))
		{
			SetStatus("Purchasing (110)...");
			PlayPhone.Billing.Purchase ("110");
		}
		if (GUILayout.Button("Purchase Item 2 (portal ID 111)"))
		{
			SetStatus("Purchasing (111)...");
			PlayPhone.Billing.Purchase ("111");
		}
		if (GUILayout.Button("Purchase a durable item (portal ID 210)"))
		{
			SetStatus("Purchasing (210)...");
			PlayPhone.Billing.Purchase ("210");
		}
		if (GUILayout.Button("Test success purchase (portal ID 211)"))
		{
			SetStatus("Purchasing (211)...");
			PlayPhone.Billing.Purchase ("211");
		}
		if (GUILayout.Button("Test failed purchase (portal ID 212)"))
		{
			SetStatus("Purchasing (212)...");
			PlayPhone.Billing.Purchase ("212");
		}
		if (GUILayout.Button("Test subscription purchase (portal ID 2603)"))
		{
			SetStatus("Purchasing (2603)...");
			PlayPhone.Billing.Purchase ("2603");
		}
		if (GUILayout.Button("Restore Purchases"))
		{
			restoredPurchaseDetails.Clear();
			SetStatus("Restoring purchases...");
			PlayPhone.Billing.RestorePurchases();
		}
		if (GUILayout.Button("Check Subscriptions"))
		{
			SetStatus("Checking subscriptions...");
			PlayPhone.Billing.GetSubscriptions();
		}
		if (GUILayout.Button("Consume Last Purchase"))
		{
			SetStatus("Consuming last purchase "+lastPurchase);
			PlayPhone.Billing.Consume(lastPurchase);
		}
	}
}
