using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFArcade {

public abstract class IStartScreen : GameScreen 
{
	// --- Interface and methods to call:

	protected abstract void onShow(ArtikFlowArcade.State oldState);		// Should display this screen.
	protected abstract void onHide();									// Should hide this screen.

	// Must be called when the player presses to play
	protected void startGame() { onStartGame(); }

	// Must be called when selecting the daily gift
	protected void openDaily() { onOpenDaily(); }


	// --- Partial private implementation (unity callbacks can be extended)

	protected virtual void Start()
	{
		ArtikFlowArcade.instance.eventStateChange.AddListener(onArtikFlowStateChange);
	}

	private void onArtikFlowStateChange(ArtikFlowArcade.State oldstate, ArtikFlowArcade.State newstate)
	{
		if (newstate != ArtikFlowArcade.State.START_SCREEN && newstate != ArtikFlowArcade.State.DAILY_SCREEN)
			onHide();
		if (newstate == ArtikFlowArcade.State.START_SCREEN)
			onShow(oldstate);
	}

	private void onStartGame()
	{
		ArtikFlowArcade.instance.setState(ArtikFlowArcade.State.GAMEPLAY_SCREEN);
	}

	private void onOpenDaily()
	{
		if(ArtikFlowArcade.instance.configuration.replaceDailyWithRoulette)
			ArtikFlowArcade.instance.setState(ArtikFlowArcade.State.ROULETTE_SCREEN);
		else
			ArtikFlowArcade.instance.setState(ArtikFlowArcade.State.DAILY_SCREEN);
	}

}

}