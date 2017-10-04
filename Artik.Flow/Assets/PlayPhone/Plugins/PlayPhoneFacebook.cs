using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace PlayPhone
{
	public static class Facebook
	{
		public static void Login()
		{
			Plugin.DoAction(Consts.PSGN_FACEBOOK_LOGIN);
		}

		public static void Logout()
		{
			Plugin.DoAction(Consts.PSGN_FACEBOOK_LOGOUT);
		}

		public static bool IsLoggedIn()
		{
			return Plugin.IsFacebookConnected();
		}

		public static void Post(string name, string description, string link, string pictureUrl, string caption)
		{
			var values = new Dictionary<string, object>() {
				{ Consts.HASH_VALUES_FACEBOOK_STREAM_NAME, name },
				{ Consts.HASH_VALUES_FACEBOOK_STREAM_DESCRIPTION, description },
				{ Consts.HASH_VALUES_FACEBOOK_STREAM_CAPTION, caption },
				{ Consts.HASH_VALUES_FACEBOOK_STREAM_LINK, link },
				{ Consts.HASH_VALUES_FACEBOOK_STREAM_PICTURE, pictureUrl },
			};

			Plugin.DoAction(Consts.PSGN_FACEBOOK_UPDATE_STREAM, values);
		}

		public static void Invite(string facebookAccountId)
		{
			var values = new Dictionary<string, object>() {
				{ Consts.HASH_VALUES_FACEBOOK_ID, facebookAccountId },
			};

			Plugin.DoAction(Consts.PSGN_FACEBOOK_INVITE_ONE, values);
		}

		public static void MassInvite()
		{
			Plugin.DoAction(Consts.PSGN_FACEBOOK_MASS_INVITE);
		}

		public static void OpenGraph()
		{
			var values = new Dictionary<string, object>() {
				{ Consts.HASH_VALUES_FACEBOOK_GRAPH_ACTION, "play" },
				{ Consts.HASH_VALUES_FACEBOOK_GRAPH_NAMESPACE, "playphonesdkdemo" },
				// TODO bundle
				//{ Consts.HASH_VALUES_FACEBOOK_GRAPH_PARAM_BUNDLE, ""}
			};

			Plugin.DoAction(Consts.PSGN_FACEBOOK_UPDATE_GRAPH, values);
		}
	}
}
