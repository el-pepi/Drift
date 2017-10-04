using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFArcade {

[CreateAssetMenu(menuName = "RouletteItems/Gems")]
public class RouletteItemGems : RouletteItem
{
	public override void onEarn()
	{
		// RewardNotification.instance.give(itemCount);
		SaveGameSystem.instance.setCoins(SaveGameSystem.instance.getCoins() + itemCount);
	}

}

}