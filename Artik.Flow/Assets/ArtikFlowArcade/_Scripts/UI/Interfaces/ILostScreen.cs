using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFArcade {

public abstract class ILostScreen : GameScreen 
{
	// --- Interface and methods to call:

	protected abstract void onShow(ArtikFlowArcade.State oldState);		// Should display this screen.
	protected abstract void onHide();									// Should hide this screen.

	// Must be called when clicking a play button
	protected void play() { onPlay(); }


	// --- Partial private implementation (unity callbacks can be extended)

	protected virtual void Start()
	{
		ArtikFlowArcade.instance.eventStateChange.AddListener(onArtikFlowStateChange);
	}

	private void onArtikFlowStateChange(ArtikFlowArcade.State oldstate, ArtikFlowArcade.State newstate)
	{
		if (newstate != ArtikFlowArcade.State.LOST_SCREEN)
			onHide();
		else if(newstate == ArtikFlowArcade.State.LOST_SCREEN)
			onShow(oldstate);
	}

	private void onPlay()
	{
		LoadingScreen.instance.fadeInOut(() => {
			ArtikFlowArcade.instance.setState(ArtikFlowArcade.State.START_SCREEN);
		});
	}

}

}