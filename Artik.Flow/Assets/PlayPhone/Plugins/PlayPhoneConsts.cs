using UnityEngine;
using System.Collections;

namespace PlayPhone
{
	public class Consts 
	{

		public const string PROPERTY_TEST_MODE = "test_mode";
		public const string PROPERTY_ANDROID_MARKET_PUB_KEY = "android_market_pub";
		public const string PROPERTY_SECRET_KEY = "secret_key";
		public const string PROPERTY_ICON_GRAVITY = "ICON_GRAVITY";
		public const string PROPERTY_ICON = "ICON";
		public const string PROPERTY_ICON_RESOURCE = "ICON_RESOURCE";
		public const string PROPERTY_ICON_NOTIFICATION_RESOURCE = "ICON_NOTIFICATION_RESOURCE";
		public const string PROPERTY_VALUE_ICON_SHOW = "show";
		public const string PROPERTY_VALUE_ICON_HIDE = "hide";
		public const string PROPERTY_PLAY_MARKET_PUB_KEY = "PLAY_MARKET_PUB";
		// Public Preset Actions

		public const string PSGN_DASHBOARD = "PSGN_DASHBOARD";
		public const string PSGN_DASHBOARD_FRIENDS = "PSGN_DASHBOARD_FRIENDS"; 
		public const string PSGN_DASHBOARD_PROFILE = "PSGN_DASHBOARD_PROFILE";
		public const string PSGN_DASHBOARD_GAMES = "PSGN_DASHBOARD_GAMES";
		public const string PSGN_DASHBOARD_NOTIFICATIONS = "PSGN_DASHBOARD_NOTIFICATIONS";
		public const string PSGN_DASHBOARD_MYPLAY = "PSGN_DASHBOARD_MYPLAY";
		public const string PSGN_DASHBOARD_EARN = "PSGN_DASHBOARD_EARN";
		public const string PSGN_LOGIN = "PSGN_LOGIN";

		public const string PSGN_FACEBOOK_LOGIN = "PSGN_FACEBOOK_LOGIN";
		public const string PSGN_FACEBOOK_LOGOUT = "PSGN_FACEBOOK_LOGOUT";
		public const string PSGN_FACEBOOK_MASS_INVITE = "PSGN_FACEBOOK_MASS_INVITE";
		public const string PSGN_FACEBOOK_INVITE_ONE = "PSGN_FACEBOOK_INVITE_ONE";
		public const string PSGN_FACEBOOK_UPDATE_STREAM = "PSGN_FACEBOOK_UPDATE_STREAM";
		public const string PSGN_FACEBOOK_UPDATE_GRAPH = "PSGN_FACEBOOK_UPDATE_GRAPH";
		public const string PSGN_SHOW_ICON = "PSGN_SHOW_ICON";
		public const string PSGN_HIDE_ICON = "PSGN_HIDE_ICON";
		public const string PSGN_PURCHASE = "PSGN_PURCHASE";
		public const string PSGN_CONSUME = "PSGN_CONSUME";
		public const string PSGN_ADD_LEADERBOARD_SCORE = "PSGN_ADD_LEADERBOARD_SCORE";
		public const string PSGN_ADD_ACHIEVEMENT = "PSGN_ADD_ACHIEVEMENT";
		public const string PSGN_RESTORE_PURCHASES = "PSGN_RESTORE_PURCHASES";
		public const string PSGN_GET_AVAILABLE_EXPANSIONS = "PSGN_GET_AVAILABLE_EXPANSIONS";
		public const string PSGN_GET_EXPANSION = "PSGN_GET_EXPANSION";
		public const string PSGN_GET_SUBSCRIPTION = "PSGN_GET_SUBSCRIPTIONS";

		public const string PSGN_LAUNCH_ARG_ACTION = "do";
		public const string PSGN_LAUNCH_ARG_SCREEN = "jump_to";
		public const string PSGN_LAUNCH_SCREEN_OFFERS = "offers";

		// Public Hash Map Values
		public const string HASH_VALUES_PLAYER_GUID = "guid";
		public const string HASH_VALUES_PURCHASE_PSGN_ITEM_ID = "gitem_id";
		public const string HASH_VALUES_PURCHASE_PSGN_ITEM_ID_RETURNED = "id";
		public const string HASH_VALUES_LEADERBOARD_ID = "leaderboard_id";
		public const string HASH_VALUES_LEADERBOARD_SCORE = "leaderboard_score";
		public const string HASH_VALUES_ACHIEVEMENT_ID = "achievement_id";
		public const string HASH_VALUES_ACTIVITY = "ACTIVITY";
		public const string HASH_VALUES_CONTEXT = "CONTEXT";
		public const string HASH_VALUES_ACTION = "ACTION";
		public const string HASH_VALUES_FACEBOOK_ID = "fbid";

		public const string HASH_VALUES_PLAYER_DATA = "player";
		public const string HASH_VALUES_FRIEND_DATA = "friends";

		public const string HASH_VALUES_FACEBOOK_STREAM_LINK = "link";
		public const string HASH_VALUES_FACEBOOK_STREAM_NAME = "name";
		public const string HASH_VALUES_FACEBOOK_STREAM_PICTURE = "picture";
		public const string HASH_VALUES_FACEBOOK_STREAM_CAPTION = "caption";
		public const string HASH_VALUES_FACEBOOK_STREAM_DESCRIPTION = "description";

		public const string HASH_VALUES_FACEBOOK_GRAPH_NAMESPACE = "namespace";
		public const string HASH_VALUES_FACEBOOK_GRAPH_ACTION = "action";
		public const string HASH_VALUES_FACEBOOK_GRAPH_PARAM_BUNDLE = "params";

		public const string HASH_VALUES_PURCHASE_RECEIPT = "purchase_receipt";
		public const string HASH_VALUES_PURCHASE_REAL_RECEIPT = "receipt";
		public const string HASH_VALUES_PURCHASE_SIGNATURE = "signature";
		public const string HASH_VALUES_PURCHASE_ERROR_MESSAGE = "error";
		public const string HASH_VALUES_PURCHASE_ERROR_CODE = "error_code";
		public const string HASH_VALUES_PURCHASE_SERVICE_ERROR_CODE = "service_error_code";
		
		public const string HASH_VALUES_PURCHASE_DONT_AUTOCONSUME = "dont_consume";
		public const string HASH_VALUES_PURCHASE_ORDER_ID = "order_id";
		public const string HASH_VALUES_ENFORCE_SIGNATURE_CHECK = "check_signature";

		public const string PURCHASE_GAME_ITEM_ID = "item_id";
		public const string PURCHASE_ITEM_QUANTITY = "quantity";

		// Public Data Values
		public const string PSGN_PLAYER = "PSGN_PLAYER";
		public const string PSGN_FRIENDS = "PSGN_FRIENDS";
		public const string PSGN_GUID = "PSGN_GUID";
		public const string DATA_PLAYER_ID = "player_id";

		// Proxy Activity
		public const string PROXY_ACTION = "PSGN_ACTION";
		public const string PROXY_VALUES = "PSGN_VALUES";

		public const int ERROR_MISSING_ITEM_KEY = 1;
		public const int ERROR_INVALID_ITEM = 2;
		public const int ERROR_NETWORK = 3;
		public const int ERROR_NETWORK_DELAYED = 4;
		public const int ERROR_CANCELED = 5;
		public const int ERROR_BILLING_SERVICE_FAILED = 6;
		public const int ERROR_NOT_SIGNED = 7;
		public const int ERROR_INTERNAL_ERROR = 8;
		public const int ERROR_BILLING_UNAVAILABLE = 9;
		public const int ERROR_PURCHASE_INVALID = 10;
	}
}