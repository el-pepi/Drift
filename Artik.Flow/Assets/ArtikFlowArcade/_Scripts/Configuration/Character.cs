using UnityEngine;
using System.Collections;
using System;

namespace AFArcade {

public class Character : ScriptableObject
{
	[HideInInspector]
	public int id;

	public string internalName = "Default";
	public string displayName = "";
	
	[Tooltip("Character price. -1 for FB Link, -2 for Instagram Link, -3 for Twitter Link.")]
	public int price = 100;
	public Texture menuTexture;

	[HideInInspector]	// Deprecated
	public Texture deadTexture;

	[Tooltip("If this is a promo character, the day/month the promotion starts")]
	public int promoStartDay;
	[Tooltip("If this is a promo character, the day/month the promotion starts")]
	public int promoStartMonth;
	[Tooltip("If this is a promo character, how many days the promo lasts from the promoStartDate")]
	public int promoDaysLength;

	public int unlockThreshold = 0;
}

}