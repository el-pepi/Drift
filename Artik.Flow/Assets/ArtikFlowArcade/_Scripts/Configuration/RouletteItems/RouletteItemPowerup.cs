using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFArcade {

[CreateAssetMenu(menuName = "RouletteItems/Powerup")]
public class RouletteItemPowerup : RouletteItem
{
	[Tooltip("Poweup to give.")]
	public Powerup powerup;

	public override void setText()
	{
		if(itemCount > 1)
			base.itemText = "x" + itemCount;
		else
			base.itemText = "";
	}

	public override void onEarn()
	{
		SaveGameSystem.instance.setPowerupCount(powerup.internalId, SaveGameSystem.instance.getPowerupCount(powerup.internalId) + itemCount);
	}

}

}