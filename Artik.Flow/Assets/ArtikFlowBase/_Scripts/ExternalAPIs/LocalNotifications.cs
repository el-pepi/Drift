using UnityEngine;
using System.Collections;
using System;
using VoxelBusters.NativePlugins;
using System.Collections.Generic;
using UnityEngine.Events;

namespace AFBase {

/// <summary>Push Notification System for Android and iOS</summary>
public class LocalNotifications : MonoBehaviour
{
	public static LocalNotifications instance;

	public class StringEvent : UnityEvent<string> { };
	[HideInInspector]
	public StringEvent eventLaunchedWithNotification = new StringEvent();

	void Awake()
	{
		instance = this;

		NotificationService.DidLaunchWithLocalNotificationEvent += onNotificationLaunch;
	}

	void onNotificationLaunch(CrossPlatformNotification notif)
	{
		string launchID = "";
		if (notif.UserInfo.Contains("launchID"))
			launchID = (string) notif.UserInfo["launchID"];

		eventLaunchedWithNotification.Invoke(launchID);
	}

	// ---

	public static string sendLocalNotification(DateTime fireDate, string titleAndroid, string message, string launchID = "", eNotificationRepeatInterval repeatInterval = eNotificationRepeatInterval.NONE)
	{
		// Note to developer: If the implementation changes, copy the eNotificationRepeatInterval enum to this class.
		if (notificationsActive() && ArtikFlowBase.instance.configuration.storeTarget != ArtikFlowBaseConfiguration.StoreTarget.FRENCH_PREMIUM)
		{
			NPBinding.NotificationService.RegisterNotificationTypes(NotificationType.Alert | NotificationType.Badge | NotificationType.Sound);

			CrossPlatformNotification.AndroidSpecificProperties _androidProps = new CrossPlatformNotification.AndroidSpecificProperties();
			_androidProps.ContentTitle = Application.productName;
			_androidProps.TickerText = message;
			_androidProps.LargeIcon = "notif.png";

			CrossPlatformNotification.iOSSpecificProperties _iosProps = new CrossPlatformNotification.iOSSpecificProperties();
			_iosProps.HasAction = true;
			_iosProps.AlertAction = Language.get("Notification.Play", false);

			CrossPlatformNotification notif = new CrossPlatformNotification();
			notif.UserInfo = new Dictionary<string, string>() { { "launchID", launchID} };
			notif.AlertBody = message;
			notif.FireDate = fireDate;
			notif.RepeatInterval = repeatInterval;
			notif.AndroidProperties = _androidProps;
			notif.iOSProperties = _iosProps;

			NPBinding.NotificationService.ScheduleLocalNotification(notif);

			return notif.GetNotificationID();
		}

		return "";
	}

	public static void cancelAllNotifications()
	{
		NPBinding.NotificationService.CancelAllLocalNotification();
	}

	public static void cancelNotification(string notificationID)
	{
		NPBinding.NotificationService.CancelLocalNotification(notificationID);	
	}

	/// <summary>Sets notification</summary>
	/// <param name="notificationActive">True for active notification, else false</param>
	public static void setNotificationsActive(bool notificationActive)
    {
        PlayerPrefs.SetInt("setNotif", notificationActive ? 1 : 0);
    }

    /// <summary>Checks if the notifications are active</summary>
    public static bool notificationsActive()
    {
        return !PlayerPrefs.HasKey("setNotif") || PlayerPrefs.GetInt("setNotif") == 1;
    }

}

}