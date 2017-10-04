using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AFBase;

namespace AFArcade {

public class Arcade_Purchaser : MonoBehaviour 
{
	public static Arcade_Purchaser instance;

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		Purchaser.instance.eventPurchased.AddListener(onPurchase);
	}

	void onPurchase(string productID)
	{
		print("onPurchase " + productID);

		if (productID == "noads") {
			setNoads ();
		} else if (productID == "gems") {
			RewardNotification.instance.give (ArtikFlowArcade.instance.configuration.gemPackCount);
		} else if (productID == "duplicate") {
			setDuplicate ();
		} else if (productID == "unlockall") {
			unlockAll ();
		} else if (productID == "starterpack"){
			if (ArtikFlowArcade.instance.configuration.gemStarterPackCount > 0) {
				RewardNotification.instance.give (ArtikFlowArcade.instance.configuration.gemStarterPackCount);
			}
			setNoads ();
		}
		else if (productID == "superpack")
		{
			if (ArtikFlowArcade.instance.configuration.gemSuperPackCount > 0) {
					RewardNotification.instance.give (ArtikFlowArcade.instance.configuration.gemSuperPackCount);
			}
			setNoads ();
			setDuplicate();
		}
		else if(productID == "pack" || productID == "packhalf")
		{
			setNoads();
			setDuplicate();
			unlockAll();
			if(ArtikFlowArcade.instance.configuration.gemPackCount > 0)
				RewardNotification.instance.give(ArtikFlowArcade.instance.configuration.gemPackCount);
		}
	}

	// ---

	void setNoads()
	{
		SaveGameSystem.instance.setNoAds(true);
		Ads.instance.hideBanner();
	}

	void setDuplicate()
	{
		SaveGameSystem.instance.setDuplicate(true);
	}

	void unlockAll(){
		foreach (Character c in CharacterManager.instance.characters){
			CharacterManager.instance.unlockCharacter(c);
		}
	}

	// -------------------------------------------------------------------v

	public bool isDuringHolidayPack()
	{
		return timeLeftForHolidayPack() != TimeSpan.Zero;
	}

	public TimeSpan timeLeftForHolidayPack()
	{
		ArtikFlowArcadeConfiguration c = ArtikFlowArcade.instance.configuration;
		if (c.IAPPromoDaysLength < 1 || c.IAPPromoDaysLength > 366 || c.IAPPromoStartMonth < 1 || c.IAPPromoStartMonth > 12 || c.IAPPromoStartDay < 1 || c.IAPPromoStartDay > 31)
			return TimeSpan.Zero;

		DateTime now = DateTime.Now;
		
		DateTime start = new DateTime(now.Year, c.IAPPromoStartMonth, c.IAPPromoStartDay);
		DateTime end = start + new TimeSpan(c.IAPPromoDaysLength, 0, 0, 0);

		if (now >= start && now <= end)
		{
			return end - now;
		}
		else    // Try previous year, should fix errors when checking for new year holidays
		{
			start = start.AddYears(-1);
			end = end.AddYears(-1);

			if (now >= start && now <= end)
				return end - now;
		}

		return TimeSpan.Zero;
	}

	public bool isDuringStarterPack()
	{
		return TimeLeftForStarterPack() != TimeSpan.Zero && 
			(!CharacterManager.instance.hasAllCharacters() || 
			!SaveGameSystem.instance.hasNoAds() || 
			!SaveGameSystem.instance.hasDuplicate());
	}

	public TimeSpan TimeLeftForStarterPack()
	{
		DateTime start = SaveGameSystem.instance.getFirstPlayDate();
		TimeSpan time_passed = DateTime.Now - start;
		double hours = TimeSpan.FromTicks(time_passed.Ticks % TimeSpan.FromDays(3).Ticks).TotalHours;	// Point in the 72hs cycle

		if(hours < 24)			// Return time left for the first 24hs in a 72hs cycle
			return TimeSpan.FromDays(1) - TimeSpan.FromHours(hours);
		else					// Return 0 during the next 48hs in that 72hs cycle
			return TimeSpan.Zero;
	}

}

}