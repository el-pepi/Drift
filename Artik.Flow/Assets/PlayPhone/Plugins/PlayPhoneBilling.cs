using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;

namespace PlayPhone
{
	public static class Billing
	{
		public class PurchaseDetails
		{
			public string Id { get ; set; }
			public string ItemId { get; set; }
			public bool IsSubscription { get; set; }
			public string Description { get; set; }
			public string Name { get; set; }
			public int Quantity { get; set; }
			public string IconUrl { get; set; }
			public string Receipt { get; set; } // Json string
			public string Signature { get; set; } // for Json string
			public string Price { get; set; }
			public string OrderId { get; set; }
		}

		public static event Action<PurchaseDetails> OnSuccess;
		public static event Action<string> OnError;
		public static event Action OnCancel;
		public static event Action<PurchaseDetails> OnRestore;

		public static event Action<string> OnSubscriptions;
		public static event Action<string> OnSubscriptionsError;

		public static event Action<string> OnConsumeSuccess;
		public static event Action<string, string> OnConsumeError;

		public static void Purchase(string itemId)
		{
			var values = new Dictionary<string, object> () {
				{ Consts.HASH_VALUES_PURCHASE_PSGN_ITEM_ID, itemId },
				{ Consts.HASH_VALUES_PURCHASE_DONT_AUTOCONSUME, 1 },
				{ Consts.HASH_VALUES_ENFORCE_SIGNATURE_CHECK, 1 },
			};
			Plugin.DoAction (Consts.PSGN_PURCHASE, values);
		}

		public static void Consume(string orderId)
		{
			var values = new Dictionary<string, object> () {
				{ Consts.HASH_VALUES_PURCHASE_ORDER_ID, orderId },
			};
			Plugin.DoAction (Consts.PSGN_CONSUME, values);
		}

		public static void GetSubscriptions()
		{
			Plugin.GetSubscriptions();
		}
		
		public static void RestorePurchases()
		{
			var values = new Dictionary<string, object> () {
				{ Consts.HASH_VALUES_PURCHASE_DONT_AUTOCONSUME, 1 },
				{ Consts.HASH_VALUES_ENFORCE_SIGNATURE_CHECK, 1 },
			};
			Plugin.DoAction (Consts.PSGN_RESTORE_PURCHASES, values);
		}

		internal static void RaiseOnSuccess(PurchaseDetails purchaseDetails)
		{
			if (OnSuccess != null)
			{
				OnSuccess (purchaseDetails);
			}
		}

		internal static void RaiseOnError(string error)
		{
			if (OnError != null) 
			{
				OnError (error);
			}
		}

		internal static void RaiseOnCancel()
		{
			if (OnCancel != null)
			{
				OnCancel ();
			}
		}

		internal static void RaiseOnRestore(PurchaseDetails purchaseDetails)
		{
			if (OnRestore != null)
			{
				OnRestore (purchaseDetails);
			}
		}

		internal static void RaiseSubscriptions(string subscriptions)
		{
			if (OnSubscriptions != null)
			{
				OnSubscriptions(subscriptions);
			}
		}

		internal static void RaiseSubscriptionsError(string error)
		{
			if (OnSubscriptionsError != null)
			{
				OnSubscriptionsError(error);
			}
		}

		internal static void RaiseOnConsumeError(string orderId, string error)
		{
			if (OnConsumeError != null)
			{
				OnConsumeError(orderId, error);
			}
		}

		internal static void RaiseOnConsumeSuccess(string orderId)
		{
			if (OnConsumeSuccess != null)
			{
				OnConsumeSuccess(orderId);
			}
		}
	}
}