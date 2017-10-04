using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFArcade {

public abstract class IShopScreen : GameScreen 
{
	// --- Interface and methods to call:

	protected abstract void onShow(ArtikFlowArcade.State oldState);		// Should display this screen.
	protected abstract void onHide();									// Should hide this screen.

	// Must be called when exiting the shop
	protected void goBack() { onGoBack(); }

	// Must be called when selecting a character to play. It must be unlocked!
	protected void selectCharacter(Character c) { onSelectChar(c); }

	// Must be called to unlock a character and save it persistently
	protected void unlockCharacter(Character c) { onUnlockChar(c); }


	// --- Partial private implementation (unity callbacks can be extended)

	protected virtual void Start()
	{
		ArtikFlowArcade.instance.eventStateChange.AddListener(onArtikFlowStateChange);
	}

	private void onArtikFlowStateChange(ArtikFlowArcade.State oldstate, ArtikFlowArcade.State newstate)
	{
		if (newstate != ArtikFlowArcade.State.SHOP_SCREEN)
			onHide();
		else if(newstate == ArtikFlowArcade.State.SHOP_SCREEN)
			onShow(oldstate);
	}

	private void onGoBack()
	{
		ArtikFlowArcade.instance.setState(ArtikFlowArcade.State.START_SCREEN);
		onHide();
	}

	private void onSelectChar(Character c)
	{
		ArtikFlowArcade.instance.setCharacter(c);
	}

	private void onUnlockChar(Character c)
	{
		CharacterManager.instance.unlockCharacter(c);
	}

}

}