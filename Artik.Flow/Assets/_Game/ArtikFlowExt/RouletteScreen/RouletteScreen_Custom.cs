using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AFArcade {

	public class RouletteScreen_Custom : IRouletteScreen 
	{
		Transform rouletteFront;
		Transform pickPivot;
		UILabel label_NextFreeSpinCounter;

		BoxCollider buttonFreeSpin;
		BoxCollider buttonGemsSpin;

		UISprite sprite_FreeSpinBack;
		UISprite sprite_FreeSpin;
		Color color_FreeSpinBack;
		Color color_FreeSpinLabel;

		UISprite sprite_GemsSpinBack;
		UISprite sprite_GemsSpin;
		Color color_GemsSpinBack;
		Color color_GemsSpinLabel;

		public Color backColor;
		public Color disable_LabelColor;

		List<RuntimeRouletteSlice> currentSlices;
		int currentWinningSlice;

		bool spinning;
		bool hasExtraSpin;

		bool dailySpin;

		bool onExtraSpin;

		public static RouletteScreen_Custom customInstance;

		UILabel label_FreeSpin;
		UILabel label_GemsSpin;

		GameObject button_Close;

		Animator lightsAnimator;

		Transform go_parent;


		bool forceWin;

		protected override void Awake()
		{
			base.Awake();

			go_parent = transform.Find ("Anim");

			rouletteFront = go_parent.Find("Roulette").Find("RouletteFront");
			pickPivot = go_parent.Find("Roulette").Find("PickPivot");
			label_NextFreeSpinCounter = go_parent.Find("Label_FreeSpin").GetComponent<UILabel>();
			buttonFreeSpin = go_parent.Find("Button_Spin").GetComponent<BoxCollider>();
			buttonGemsSpin = go_parent.Find("Button_SpinGems").GetComponent<BoxCollider>();
			label_FreeSpin = buttonFreeSpin.GetComponentInChildren<UILabel> ();
			label_GemsSpin = buttonGemsSpin.GetComponentInChildren<UILabel> ();

			sprite_FreeSpin = buttonFreeSpin.transform.Find ("Anim").Find("Button_Sprite").GetComponent<UISprite>();
			sprite_FreeSpinBack = buttonFreeSpin.transform.Find ("Anim").Find("Button_BackSprite").GetComponent<UISprite>();
			color_FreeSpinBack = sprite_FreeSpinBack.color;
			color_FreeSpinLabel = label_FreeSpin.color;

			sprite_GemsSpin = buttonGemsSpin.transform.Find ("Anim").Find("Button_Sprite").GetComponent<UISprite>();
			sprite_GemsSpinBack = buttonGemsSpin.transform.Find ("Anim").Find("Button_BackSprite").GetComponent<UISprite>();
			color_GemsSpinBack = sprite_GemsSpinBack.color;
			color_GemsSpinLabel = label_GemsSpin.color;

			go_parent.Find ("Title").GetComponent<UILabel> ().text = Language.get("Drift.Roulette.Title");

			button_Close = go_parent.Find("Button_Close").gameObject;

			lightsAnimator = go_parent.Find("LightManager").GetComponent<Animator>();

			customInstance = this;

		}

		protected override void Start()
		{
			base.Start();

			label_GemsSpin.text = ArtikFlowArcade.instance.configuration.gemsToSpin.ToString();
		}

		void Update()
		{
			if (!spinning && Input.GetKeyDown(KeyCode.Escape) && PopupManager.instance.getCurrentPopup() == null)
				onBack();

			if(RouletteButton.getSecondsUntilReward() > 0 && !hasExtraSpin)
			{
				string timeLeft = RouletteButton.getTimeString(RouletteButton.getSecondsUntilReward());
				label_NextFreeSpinCounter.text = Language.get("Roulette.TimeLeft").Replace("%",timeLeft); //Language.get("DropIt.Roulette.Freespin0").Replace("%", timeLeft);
			}
			else if(label_NextFreeSpinCounter.gameObject.activeSelf)
			{
				label_NextFreeSpinCounter.gameObject.SetActive(false);
				buttonGemsSpin.gameObject.SetActive(false);
				buttonFreeSpin.gameObject.SetActive(true);
			}

		}



		private void populateRoulette()
		{
			rouletteFront.eulerAngles = Vector3.zero;

			currentSlices = base.getRouletteItems(); 
			if (forceWin) 
			{
				forceWin = false;
				currentWinningSlice = PickWinningAmount (50);
			}
			else
			{
				currentWinningSlice = base.pickWinningSlice (currentSlices);
			}

			GameObject template = rouletteFront.GetChild(0).gameObject;
			Vector2 templateDistance = template.transform.position - rouletteFront.position;

			int i = 0;
			foreach(RuntimeRouletteSlice slice in currentSlices)
			{
				// Create new child
				GameObject newItem = NGUITools.AddChild(rouletteFront.gameObject, template);
				newItem.name = "" + i + "." + slice.item.name;

				// Set its params
				UITexture itemTexture = newItem.GetComponent<UITexture>();
				float original_width = itemTexture.width;
				itemTexture.mainTexture = slice.item.itemTexture;
				itemTexture.MakePixelPerfect();
				float factor = itemTexture.width / original_width;
				itemTexture.width = (int) original_width;
				itemTexture.height = (int) (itemTexture.height / factor);

				slice.item.setText();
				UILabel label = newItem.transform.Find("Label").GetComponent<UILabel>();
				if(slice.item.itemText == null || slice.item.itemText.Length == 0)
					label.gameObject.SetActive(false);
				else	
				{
					label.gameObject.SetActive(true);
					label.text = slice.item.itemText;
				}

				// Set its position
				float angle = getAngleFromSliceIndex(i, currentSlices.Count);
				newItem.transform.eulerAngles = new Vector3(0f, 0f, angle);
				newItem.transform.position = rouletteFront.position + (Quaternion.Euler(0f, 0f, angle) * templateDistance);

				i ++;
			}

			template.SetActive(false);
		}

		private void unpopulateRoulette()
		{
			foreach(Transform t in rouletteFront)
			{
				if(t.name == "Template")
					t.gameObject.SetActive(true);
				else
					Destroy(t.gameObject);
			}
		}

		int PickWinningAmount(int winningAmount)
		{
			int i = 0;

			foreach (var item in currentSlices)
			{

				if(item.item.itemCount == winningAmount)
					return i;
				i++;
			}

			return 0;
		}


		float getAngleFromSliceIndex(int sliceIdx, int sliceCount)
		{
			return (360f / sliceCount) * sliceIdx;
		}

		IEnumerator performSpin(int winningIndex)
		{
			SetButton (false);

			lightsAnimator.Play ("RouletteLights_Spin");
			button_Close.SetActive (false);

			float anglePerSlice = 360f / currentSlices.Count;
			float angleTarget = clamp360(getAngleFromSliceIndex(winningIndex, currentSlices.Count) * 1);

			float angleDifference = (clamp360(rouletteFront.eulerAngles.z - angleTarget) * 1) / 360f;
			angleDifference += UnityEngine.Random.Range(-anglePerSlice / 360f / 3f, anglePerSlice / 360f / 3f);
			iTween.RotateBy(rouletteFront.gameObject, iTween.Hash("z", -8f + angleDifference, "easeType", iTween.EaseType.easeOutCubic, "time", 6f));
			yield return new WaitForSeconds(5.5f);

			spinning = false;
			lightsAnimator.Play ("RouletteLights_Win");
			yield return new WaitForSeconds (1.5f);
			givePrize();
		}



		IEnumerator movePick()
		{
			float prev_rot = rouletteFront.eulerAngles.z;
			float anglePerSlice = 360f / currentSlices.Count;
			float offsetted_roulette_rot;

			while(true)
			{
				offsetted_roulette_rot = rouletteFront.eulerAngles.z + (anglePerSlice / 2f);

				float dif = offsetted_roulette_rot % anglePerSlice - prev_rot % anglePerSlice;


				if (dif > 0 && spinning) 
				{
					pickPivot.eulerAngles = new Vector3 (0f, 0f, 62f - ((dif / anglePerSlice) * 32f));

					AFArcade.Audio.instance.playName ("roulette_tick");
				}
				pickPivot.eulerAngles = new Vector3(0f, 0f, pickPivot.eulerAngles.z / 1.2f);
				prev_rot = offsetted_roulette_rot;

				yield return null;
			}
		}

		float clamp360(float val)
		{
			while(val < 0)
				val += 360;
			while(val >= 360)
				val -= 360;
			return val;
		}

		void givePrize()
		{
			Audio.instance.playName ("daily_giftopen");



			button_Close.SetActive (true);

			float anglePerSlice = 360f / currentSlices.Count;
			float offsetted_roulette_rot = rouletteFront.eulerAngles.z + (anglePerSlice / 2f);
			int winIdx = (currentSlices.Count - Mathf.FloorToInt(clamp360(offsetted_roulette_rot) / anglePerSlice)) % currentSlices.Count;

			RouletteItem prizeWon = currentSlices[winIdx].item;
			IPopup_RouletteWin.prize = prizeWon;
			PopupManager.instance.showPopup<IPopup_RouletteWin>();

			// Post-prize
			if(onExtraSpin)
			{
				onExtraSpin = false;

				SaveGameSystem.instance.increaseDailysCollected ();

				DateTime nextReward = DateTime.Now.AddMinutes (calculateTimeForNextReward ());

				SaveGameSystem.instance.setNextDailyTime (nextReward);
			}
			label_NextFreeSpinCounter.gameObject.SetActive(true);
			SetButton (true);
			buttonFreeSpin.gameObject.SetActive(false);
			buttonGemsSpin.gameObject.SetActive(true);
			lightsAnimator.Play ("RouletteLights_idle");
			Invoke("repopulateRoulette", 0.7f);
		}

		void repopulateRoulette()
		{
			unpopulateRoulette();
			populateRoulette();
		}

		int calculateTimeForNextReward()
		{
			int[] timeForRewards = ArtikFlowArcade.instance.configuration.timeForRewards;

			int collected = SaveGameSystem.instance.getDailysCollected ();

			if (collected == 0)		// First gift is instantaneous
				return 0;
			else
				collected--;

			if (collected >= timeForRewards.Length)
				return timeForRewards[timeForRewards.Length - 1];

			return timeForRewards[collected];
		}

		// --- Callbacks ---

		protected override void onHide()
		{
			Audio.instance.playName ("button");
			gameObject.SetActive(false);
			//
			unpopulateRoulette();
		}

		void SetButton(bool state)
		{
			buttonFreeSpin.enabled = state;
			buttonGemsSpin.enabled = state;

			if (state)
			{
				sprite_FreeSpin.spriteName = "PopUpButtonYELLOW";
				sprite_FreeSpinBack.color = color_FreeSpinBack;
				label_FreeSpin.color = color_FreeSpinLabel;

				sprite_GemsSpin.spriteName = "PopUpButtonGEMSpurchase";
				sprite_GemsSpinBack.color = color_GemsSpinBack;
				label_GemsSpin.color = color_GemsSpinLabel;

			}

			if (!state)
			{
				sprite_FreeSpin.spriteName = "PopUpButtonGRAY";
				sprite_FreeSpinBack.color = backColor;
				label_FreeSpin.color = disable_LabelColor;

				sprite_GemsSpin.spriteName = "PopUpButtonGEMSpurchaseGRAY";
				sprite_GemsSpinBack.color = backColor;
				label_GemsSpin.color = disable_LabelColor;

			}
		}

		protected override void onShow(ArtikFlowArcade.State oldState)
		{
			gameObject.SetActive(true);

			lightsAnimator.Play ("RouletteLights_idle");

			if(DailyButton.getSecondsUntilReward() > 0 && !hasExtraSpin)
			{
				buttonFreeSpin.gameObject.SetActive(false);
				buttonGemsSpin.gameObject.SetActive(true);
				label_NextFreeSpinCounter.gameObject.SetActive(true);
			}
			else
			{
				buttonFreeSpin.gameObject.SetActive(true);
				buttonGemsSpin.gameObject.SetActive(false);
				label_NextFreeSpinCounter.gameObject.SetActive(false);
			}
			label_FreeSpin.text = Language.get ("Roulette.FreeSpin");
			populateRoulette();
			StartCoroutine(movePick());
		}

		protected override void spin()
		{
			StartCoroutine(performSpin(currentWinningSlice));
			spinning = true;
		}

		public override void giveExtraFreeSpin()
		{
			hasExtraSpin = true;
			buttonFreeSpin.gameObject.SetActive(true);
			buttonGemsSpin.gameObject.SetActive(false);
		}

		public void onFreeSpinClick()
		{
			if (spinning)
				return;

			if(!hasExtraSpin)		// This is a free gifted spin
				IPopup_RouletteWin.videoSpinsLeft = 1;		// Show video button on next RouletteWin popup.


			onExtraSpin = true;
			spin();
		}

		public void onGemsSpinClick()
		{
			if (spinning)
				return;

			int cost = ArtikFlowArcade.instance.configuration.gemsToSpin;
			if(cost > SaveGameSystem.instance.getCoins())
				PopupManager.instance.showPopup<IPopup_IAP>();
			else
			{
				SaveGameSystem.instance.setCoins(SaveGameSystem.instance.getCoins() - cost);
				spin();
			}
		}
		public void onAnimOutFinish()
		{
			spinning = false;
			ArtikFlowArcade.instance.setState(ArtikFlowArcade.State.START_SCREEN);
		}
		public void onBack()
		{
			GetComponent<Animator> ().SetTrigger ("Close");

		}

	}

}