using UnityEngine;
using System;

public class PlayPhoneAndroidCallbacks : MonoBehaviour
{
	#region Init callbacks

	void onInitError(string arg)
	{
		var messageId = GetMessageId(arg);
		var error = PlayPhone.Plugin.GetResultMapValue<string> (messageId, "error");
		PlayPhone.Plugin.RaiseOnInitError (error);
		PlayPhone.Plugin.ClearResult(messageId);
	}

	void onInit(string arg)
	{
		var messageId = GetMessageId(arg);
		PlayPhone.Plugin.RaiseOnInit();
		PlayPhone.Plugin.ClearResult(messageId);
	}

	void onUserChanged(string arg)
	{
		var messageId = GetMessageId(arg);
		PlayPhone.Plugin.RaiseUserChanged();
		PlayPhone.Plugin.ClearResult(messageId);
	}

	void onUserChangedError(string arg)
	{
		var messageId = GetMessageId(arg);
		var error = PlayPhone.Plugin.GetResultMapValue<string> (messageId, "error");
		PlayPhone.Plugin.RaiseUserChangedError(error);
		PlayPhone.Plugin.ClearResult(messageId);
	}

	void OnApplicationPause(bool paused)
	{
		PlayPhone.Plugin.OnApplicationPause (paused);
	}

	void OnLaunchScreen(string arg)
	{
		var messageId = GetMessageId(arg);
		var screen = PlayPhone.Plugin.GetResultMapValue<string> (messageId, "getLaunchScreen");
		PlayPhone.Plugin.RaiseOnLaunchScreen (screen);
		PlayPhone.Plugin.ClearResult(messageId);
	}

	#endregion

	#region Billing callbacks

	void onBillingSuccess(string arg)
	{
		var messageId = GetMessageId(arg);
		var purchase = ReadPurchaseDetails(messageId);
		PlayPhone.Billing.RaiseOnSuccess (purchase);
		PlayPhone.Plugin.ClearResult(messageId);
	}

	void onBillingError(string arg)
	{
		var messageId = GetMessageId(arg);
		var error = PlayPhone.Plugin.GetResultMapValue<string>(messageId, PlayPhone.Consts.HASH_VALUES_PURCHASE_ERROR_MESSAGE);

		PlayPhone.Billing.RaiseOnError (error);
		PlayPhone.Plugin.ClearResult(messageId);
	}

	void onBillingCancel(string arg)
	{
		var messageId = GetMessageId(arg);
		PlayPhone.Billing.RaiseOnCancel ();
		PlayPhone.Plugin.ClearResult(messageId);
	}

	void onBillingRestore(string arg)
	{
		var messageId = GetMessageId(arg);
		var purchase = ReadPurchaseDetails(messageId);
		PlayPhone.Billing.RaiseOnRestore (purchase);
		PlayPhone.Plugin.ClearResult(messageId);
	}

	void onSubscriptions(string arg)
	{
		var messageId = GetMessageId(arg);
		var subscriptions = PlayPhone.Plugin.GetResultSubscriptions(messageId);
		PlayPhone.Billing.RaiseSubscriptions(subscriptions);
		PlayPhone.Plugin.ClearResult(messageId);
	}

	void onSubscriptionsError(string arg)
	{
		var messageId = GetMessageId(arg);
		var error = PlayPhone.Plugin.GetResultMapValue<string>(messageId, "error");
		PlayPhone.Billing.RaiseSubscriptionsError(error);
		PlayPhone.Plugin.ClearResult(messageId);
	}
	
	void onConsumeSuccess(string arg)
	{
		var messageId = GetMessageId(arg);
		var orderId = PlayPhone.Plugin.GetResultMapValue<string>(messageId, PlayPhone.Consts.HASH_VALUES_PURCHASE_ORDER_ID);
		PlayPhone.Billing.RaiseOnConsumeSuccess (orderId);
		PlayPhone.Plugin.ClearResult(messageId);
	}

	void onConsumeError(string arg)
	{
		var messageId = GetMessageId(arg);
		var orderId = PlayPhone.Plugin.GetResultMapValue<string>(messageId, PlayPhone.Consts.HASH_VALUES_PURCHASE_ORDER_ID);
		var error = PlayPhone.Plugin.GetResultMapValue<string>(messageId, PlayPhone.Consts.HASH_VALUES_PURCHASE_ERROR_MESSAGE);
		PlayPhone.Billing.RaiseOnConsumeError (orderId, error);
		PlayPhone.Plugin.ClearResult(messageId);
	}

	#endregion

	#region License callbacks

	void onLicensePass(string arg)
	{
		var messageId = GetMessageId(arg);
		PlayPhone.License.RaiseOnSuccess();
		PlayPhone.Plugin.ClearResult(messageId);
	}

	void onLicenseError(string arg)
	{
		var messageId = GetMessageId(arg);
		var error = PlayPhone.Plugin.GetResultMapValue<string>(messageId, "error");
		PlayPhone.License.RaiseOnError(error);
		PlayPhone.Plugin.ClearResult(messageId);
	}

	#endregion

	#region In App Products callbacks
	
	void onInAppProducts(string arg)
	{
		var messageId = GetMessageId(arg);
		var json = PlayPhone.Plugin.GetResultJson(messageId);
		PlayPhone.Plugin.RaiseOnInAppProducts(json);
		PlayPhone.Plugin.ClearResult(messageId);
	}

	void onInAppProductsError(string arg)
	{
		var messageId = GetMessageId(arg);
		var json = PlayPhone.Plugin.GetResultJson(messageId);
		PlayPhone.Plugin.RaiseOnInAppProductsError(json);
		PlayPhone.Plugin.ClearResult(messageId);
	}
	
	#endregion

	#region Get Leaderboards Data callbacks
	
	void onLeaderboardsData(string arg)
	{
		var messageId = GetMessageId(arg);
		var json = PlayPhone.Plugin.GetResultJson(messageId);
		PlayPhone.Plugin.RaiseOnLeaderboards(json);
		PlayPhone.Plugin.ClearResult(messageId);
	}

	void onLeaderboardsDataError(string arg)
	{
		var messageId = GetMessageId(arg);
		var json = PlayPhone.Plugin.GetResultJson(messageId);
		PlayPhone.Plugin.RaiseOnLeaderboardsError(json);
		PlayPhone.Plugin.ClearResult(messageId);
	}

	#endregion

	#region Player data callbacks

	void onRemoteGamePlayerData(string arg)
	{
		var messageId = GetMessageId(arg);
		var json = PlayPhone.Plugin.GetResultJson(messageId);
		PlayPhone.PlayerData.RaiseOnRemoteGamePlayerData(json);
		PlayPhone.Plugin.ClearResult(messageId);
	}

	void onRemoteGamePlayerDataError(string arg)
	{
		var messageId = GetMessageId(arg);
		var error = PlayPhone.Plugin.GetResultMapValue<string>(messageId, "error");
		PlayPhone.PlayerData.RaiseOnRemoteGamePlayerDataError(error);
		PlayPhone.Plugin.ClearResult(messageId);
	}

	void onCurrentPlayerData(string arg)
	{
		var messageId = GetMessageId(arg);
		var json = PlayPhone.Plugin.GetResultJson(messageId);
		PlayPhone.PlayerData.RaiseOnCurrentPlayerData(json);
		PlayPhone.Plugin.ClearResult(messageId);
	}

	void onCurrentPlayerDataError(string arg)
	{
		var messageId = GetMessageId(arg);
		var error = PlayPhone.Plugin.GetResultMapValue<string>(messageId, "error");
		PlayPhone.PlayerData.RaiseOnCurrentPlayerDataError(error);
		PlayPhone.Plugin.ClearResult(messageId);
	}

	void onFriendsData(string arg)
	{
		var messageId = GetMessageId(arg);
		var json = PlayPhone.Plugin.GetResultJson(messageId);
		PlayPhone.PlayerData.RaiseOnFriends(json);
		PlayPhone.Plugin.ClearResult(messageId);
	}

	void onFriendsDataError(string arg)
	{
		var messageId = GetMessageId(arg);
		var error = PlayPhone.Plugin.GetResultMapValue<string>(messageId, "error");
		PlayPhone.PlayerData.RaiseOnFriendsError(error);
		PlayPhone.Plugin.ClearResult(messageId);
	}

	void onPlayerData(string arg)
	{
		var messageId = GetMessageId(arg);
		var json = PlayPhone.Plugin.GetResultJson(messageId);
		PlayPhone.PlayerData.RaiseOnPlayerData(json);
		PlayPhone.Plugin.ClearResult(messageId);
	}

	void onPlayerDataError(string arg)
	{
		var messageId = GetMessageId(arg);
		var error = PlayPhone.Plugin.GetResultMapValue<string>(messageId, "error");
		PlayPhone.PlayerData.RaiseOnPlayerDataError(error);
		PlayPhone.Plugin.ClearResult(messageId);
	}

	#endregion

	#region Expansions callbacks

	void onServerExpansions(string arg)
	{
		var messageId = GetMessageId(arg);
		var expansions = PlayPhone.Plugin.GetReceivedExpansions(messageId);
		PlayPhone.Expansions.RaiseServerExpansions(expansions);
		PlayPhone.Plugin.ClearResult(messageId);
	}

	void onServerExpansionsError(string arg)
	{
		var messageId = GetMessageId(arg);
		PlayPhone.Expansions.RaiseServerExpansionsError(string.Empty);
		PlayPhone.Plugin.ClearResult(messageId);
	}

	void onExpansion(string arg)
	{
		var messageId = GetMessageId(arg);
		var expansions = PlayPhone.Plugin.GetReceivedExpansions(messageId);
		PlayPhone.Expansion expansion = null;
		if (expansions != null && expansions.Length > 0)
		{
			expansion = expansions[0];
		}
		PlayPhone.Expansions.RaiseExpansion(expansion);
		PlayPhone.Plugin.ClearResult(messageId);
	}

	void onExpansionError(string arg)
	{
		var messageId = GetMessageId(arg);
		PlayPhone.Expansions.RaiseExpansionError(string.Empty);
		PlayPhone.Plugin.ClearResult(messageId);
	}

	void onExpansionsToUpdate(string arg)
	{
		var messageId = GetMessageId(arg);
		var expansions = PlayPhone.Plugin.GetReceivedExpansions(messageId);
		PlayPhone.Expansions.RaiseExpansionsToUpdate(expansions);
		PlayPhone.Plugin.ClearResult(messageId);
	}

	void onExpansionsToUpdateError(string arg)
	{
		var messageId = GetMessageId(arg);
		PlayPhone.Expansions.RaiseExpansionsToUpdateError(string.Empty);
		PlayPhone.Plugin.ClearResult(messageId);
	}

	void onExpansionDownloaded(string arg)
	{
		var parts = arg.Split(' ');
		var id = parts[0];
		var success = bool.Parse(parts[1]);
		PlayPhone.Expansions.RaiseExpansionDownloaded(id, success);
	}

	void onExpansionProgress(string arg)
	{
		var parts = arg.Split(' ');
		var id = parts[0];
		var downloaded = long.Parse(parts[1]);
		var left = long.Parse(parts[2]);
		PlayPhone.Expansions.RaiseExpansionProgress(id, downloaded, left);
	}

	#endregion

	private static PlayPhone.Billing.PurchaseDetails ReadPurchaseDetails(long messageId)
	{
		var purchase = new PlayPhone.Billing.PurchaseDetails();

		purchase.Id = PlayPhone.Plugin.GetResultMapValue<string>(messageId, PlayPhone.Consts.HASH_VALUES_PURCHASE_PSGN_ITEM_ID_RETURNED);
		purchase.ItemId = PlayPhone.Plugin.GetResultMapValue<string>(messageId, PlayPhone.Consts.PURCHASE_GAME_ITEM_ID);
		purchase.Quantity = PlayPhone.Plugin.GetResultMapValue<int>(messageId, PlayPhone.Consts.PURCHASE_ITEM_QUANTITY);
		purchase.Receipt = PlayPhone.Plugin.GetResultMapValue<string>(messageId, PlayPhone.Consts.HASH_VALUES_PURCHASE_REAL_RECEIPT);
		purchase.Signature = PlayPhone.Plugin.GetResultMapValue<string>(messageId, PlayPhone.Consts.HASH_VALUES_PURCHASE_SIGNATURE);
		purchase.OrderId = PlayPhone.Plugin.GetResultMapValue<string>(messageId, "transaction_id");
		purchase.Description = PlayPhone.Plugin.GetResultMapValue<string>(messageId, "description");
		purchase.IconUrl = PlayPhone.Plugin.GetResultMapValue<string>(messageId, "icon");
		purchase.IsSubscription = PlayPhone.Plugin.GetResultMapValue<int>(messageId, "subscription") == 1;
		purchase.Name = PlayPhone.Plugin.GetResultMapValue<string>(messageId, "name");
		purchase.Price = PlayPhone.Plugin.GetResultMapValue<string>(messageId, "price");

		return purchase;
	}

	private static long GetMessageId(string arg)
	{
		if (!string.IsNullOrEmpty(arg))
		{
			string id;
			var i = arg.IndexOf(' ');
			if (i > 0)
			{
				id = arg.Substring(0, i);
			}
			else
			{
				id = arg;
			}

			long messageId;
			if (long.TryParse(id, out messageId))
			{
				return messageId;
			}
		}
		return -1;
	}
}
