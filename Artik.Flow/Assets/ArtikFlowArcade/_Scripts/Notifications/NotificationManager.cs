using UnityEngine;
using System.Collections;
using System;

namespace AFArcade {

public class NotificationManager : MonoBehaviour
{
	const string DAILYGIFT_NOTIF_ID = "dailyGiftNotificationID";
	const string GIFT1_LAUNCH_ID = "GIFT1";
	const string GIFT2_LAUNCH_ID = "GIFT2";
	const string GIFT3_LAUNCH_ID = "GIFT3";

	void Awake()
	{
		AFBase.LocalNotifications.instance.eventLaunchedWithNotification.AddListener(onNotificationLaunch);
	}

	void Start ()
	{
		SaveGameSystem.instance.eventDailyGiftUpdate.AddListener(onDailyGiftUpdate);

		StartCoroutine(rescheduleFutureNotifications());
	}

	void onNotificationLaunch(string launchID)
	{
		if (launchID == GIFT1_LAUNCH_ID && ArtikFlowArcade.instance.configuration.notificationFirstGift > 0)
			RewardNotification.instance.give(ArtikFlowArcade.instance.configuration.notificationFirstGift);
		else if (launchID == GIFT2_LAUNCH_ID && ArtikFlowArcade.instance.configuration.notificationSecondGift > 0)
			RewardNotification.instance.give(ArtikFlowArcade.instance.configuration.notificationSecondGift);
		else if (launchID == GIFT3_LAUNCH_ID && ArtikFlowArcade.instance.configuration.notificationThirdGift > 0)
			RewardNotification.instance.give(ArtikFlowArcade.instance.configuration.notificationThirdGift);
	}

	// ---

	IEnumerator rescheduleFutureNotifications()
	{
		AFBase.LocalNotifications.cancelAllNotifications();
		yield return null;

		// Current gift:
		updateDailyGiftNotification();
		yield return null;

		// Future reminders:
		// For one week, every 1 day, 'config.notificationFirstGift' gems gift
		// For one month, every 3 days, 'config.notificationSecondGift' gems gift
		// For one year, every 1 week, 'config.notificationThirdGift' gems gift

		DateTime notifDate = DateTime.Now;
		string desc;
		if (ArtikFlowArcade.instance.configuration.notificationFirstGift == 0)
			desc = Language.get("Notification.NoGift", false);
		else
			desc = Language.get("Notification.GemsGift", false).Replace("%", ArtikFlowArcade.instance.configuration.notificationFirstGift.ToString());
		for (int i = 0; i < 7; i++)
		{
			notifDate = notifDate.AddDays(1);
			AFBase.LocalNotifications.sendLocalNotification(notifDate, Application.productName, desc, GIFT1_LAUNCH_ID);
			yield return null;
		}

		if (ArtikFlowArcade.instance.configuration.notificationSecondGift == 0)
			desc = Language.get("Notification.NoGift", false);
		else
			desc = Language.get("Notification.GemsGift", false).Replace("%", ArtikFlowArcade.instance.configuration.notificationSecondGift.ToString());
		for (int i = 0; i < 10; i++)
		{
			notifDate = notifDate.AddDays(3);
			AFBase.LocalNotifications.sendLocalNotification(notifDate, Application.productName, desc, GIFT2_LAUNCH_ID);
			yield return null;
		}

		if (ArtikFlowArcade.instance.configuration.notificationThirdGift == 0)
			desc = Language.get("Notification.NoGift", false);
		else
			desc = Language.get("Notification.GemsGift", false).Replace("%", ArtikFlowArcade.instance.configuration.notificationThirdGift.ToString());
		notifDate = notifDate.AddDays(3);
		AFBase.LocalNotifications.sendLocalNotification(notifDate, Application.productName, desc, GIFT3_LAUNCH_ID, VoxelBusters.NativePlugins.eNotificationRepeatInterval.WEEK);
	}

	void updateDailyGiftNotification()
	{
		// Cancel previous
		string notifID = PlayerPrefs.GetString(DAILYGIFT_NOTIF_ID, "");
		if (notifID != "")
			AFBase.LocalNotifications.cancelNotification(notifID);
		
		// Set new
		notifID = AFBase.LocalNotifications.sendLocalNotification(SaveGameSystem.instance.getNextDailyTime(), Application.productName, Language.get("PushNotification.FreeGift", false));
		PlayerPrefs.SetString(DAILYGIFT_NOTIF_ID, notifID);
	}
	
	// ---

	void onDailyGiftUpdate()
	{
		// Skip if the time is now
		if ((SaveGameSystem.instance.getNextDailyTime() - DateTime.Now).TotalSeconds < 5f)
			return;

		updateDailyGiftNotification();
	}

}

}