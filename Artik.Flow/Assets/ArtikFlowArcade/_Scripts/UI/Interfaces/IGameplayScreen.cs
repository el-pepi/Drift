using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFArcade {

public abstract class IGameplayScreen : GameScreen 
{
	public static IGameplayScreen instance;

	// --- Interface and methods to call:
	protected abstract void onToggleCoins(bool show);				// Should toggle the coins label on or off.
	protected abstract void onToggleScore(bool show);				// Should toggle the score and highscore labels on or off.
	protected abstract void onScoreUpdate(int score);				// Called when the score changed.
	protected abstract void onCoinsUpdate(int coins);				// Called when the coins changed.
	protected abstract void onHighscoreUpdate(int newHigh);			// Called when the highscore has changed.

	public abstract Vector3 getCoinsPosition();						// Must return the position of the coins' sprite transform.


	// --- Public API

	private bool _freezeCoins;
	// If set to true, the number of displayed coins will stay frozen. Handled automatically by this interface.
	public bool freezeCoins {
		get {
			return _freezeCoins;
		} 
		set {
			_freezeCoins = value;
			if(value)
				SaveGameSystem.instance.eventCoinsUpdate.RemoveListener(onCoinsUpdate);
			else {
				SaveGameSystem.instance.eventCoinsUpdate.AddListener(onCoinsUpdate);
				onCoinsUpdate(SaveGameSystem.instance.getCoins());		// Force-update the coin count
			}
		}
	}							

	public void toggleScore(bool show)
	{
		// Override depending on configuration
		if(ArtikFlowArcade.instance.configuration.displayScoreIngame)
			onToggleScore(show);
	}

	public void toggleCoins(bool show)
	{
		onToggleCoins(show);
	}


	// --- Partial private implementation (unity callbacks can be extended)

	protected override void Awake()
	{
		instance = this;
		base.Awake();
	}

	protected virtual void Start()
	{
		ArtikFlowArcade.instance.eventStateChange.AddListener(onArtikFlowStateChange);
		ArtikFlowArcade.instance.eventScoreUpdate.AddListener(onScoreUpdate);
		SaveGameSystem.instance.eventCoinsUpdate.AddListener(onCoinsUpdate);
		SaveGameSystem.instance.eventHighscoreUpdate.AddListener(onHighscoreUpdate);

		onToggleScore(false);
		onToggleCoins(false);

		onCoinsUpdate(SaveGameSystem.instance.getCoins());		// Force-update the coin count
	}

	private void onArtikFlowStateChange(ArtikFlowArcade.State oldstate, ArtikFlowArcade.State newstate)
	{
		// Score and coins toggling:
		if (newstate == ArtikFlowArcade.State.GAMEPLAY_SCREEN)
		{
			toggleScore(true);
			toggleCoins(ArtikFlowArcade.instance.configuration.displayCoinsIngame);
			onHighscoreUpdate(SaveGameSystem.instance.getHighScore());		// Force-update highscore
		}
		else if(newstate == ArtikFlowArcade.State.SHOP_SCREEN)
		{
			toggleScore(false);
			toggleCoins(false);
		}
		else
		{
			toggleScore(false);
			toggleCoins(true);
		}
	}

}

}