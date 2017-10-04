using UnityEngine;
using System.Collections;
using GooglePlayGames;
using AFBase;

namespace AFArcade {

/*
* Makes use of Google's Player Stats API to increase player engagement. Requires Google Play Games.
* https://developers.google.com/games/services/android/stats
* https://docs.google.com/document/d/17HQekR_U5MzpU5_9bu_JjknQTeSgcevaWJtYheR69-o
*/
public class Arcade_BasePlayerStats : MonoBehaviour
{
	public static Arcade_BasePlayerStats instance;

	protected const float LOW = 0.3f;
	protected const float HIGH = 0.7f;

	protected virtual void Awake()
	{
		// Dont call base.Awake() from children

#if UNITY_ANDROID
		if(ArtikFlowArcade.instance.configuration.disablePlayerStatsAPI == false)
		{
			gameObject.AddComponent<Arcade_AndroidPlayerStats>();
			Destroy(this);
			return;
		}
		else
			instance = this;
#elif UNITY_IOS
		instance = this;		// Replace with iOS implementation, if it ever exists
#endif
	}

	// ---

	/// Should the revive popup be always shown, regardless of score made?
	public virtual bool alwaysRevive()
	{
		return false;
	}

	/// Should the frequency of video ads in the lost screen be increased
	public virtual bool increaseVideoAdFrequency()
	{
		return false;
	}

	/// If the GameServices should be highlighted or not
	public virtual bool shouldHighlightGameServices()
	{
		return false;
	}

}

}