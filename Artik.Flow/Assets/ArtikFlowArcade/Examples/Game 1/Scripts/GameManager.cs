using UnityEngine;
using System.Collections;
using System;

namespace ArtikFlowExample
{
	public class GameManager : AFArcade.ArtikFlowArcadeGame
	{
		public static GameManager instance;

		public Animator examplePowerup;

		[HideInInspector]
		public bool playing;

		public override void Awake()
		{
			instance = this;

			base.Awake();
		}

		// ----------------------- Abstract implementation

		/// <summary> Called after the StartScreen. The game has to start playing. </summary>
		public override void play()
		{
			playing = true;
		}

		/// <summary> Called during the LoadingScreen. The game has to reset its state and idle. </summary>
		public override void reset(AFArcade.Character character)
		{
			playing = false;
			Player.instance.reset();
			Player.instance.setCharacter(character);

			// Test changing the current gamemode:
			// eventSetCurrentGamemode.Invoke(AFArcade.ArtikFlowArcade.instance.playsThisSession % 2);

			eventResetReady.Invoke();
		}

		/// <summary> Called during the GameplayScreen if the player revives. The game has to resume. </summary>
		public override void revive()
		{
			playing = true;
			Player.instance.transform.position = Vector3.zero;
		}

		/// <summary> Called during the CharSelectionScreen if the player changes the sound setting. true to turn the sound on, false to turn it off.</summary>
		public override void switchSound(bool state)
		{
			return;
		}

		// ----------------------- Custom functionality

		public void die()
		{
			playing = false;
			eventFinish.Invoke();
		}

		public void usePowerup()
		{
            if(AFArcade.SaveGameSystem.instance.getPowerupCount("example") > 0)
			{
				examplePowerup.transform.position = Player.instance.transform.position;
				examplePowerup.Play("powerup", -1, 0f);
                GameManager.instance.eventUsePowerup.Invoke("example", 1);
			}
		}

	}

}
