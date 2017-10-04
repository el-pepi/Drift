using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace AFArcade {

	public class Popup_Reward_Custom : AFArcade.Popup
	{
		UILabel label_GemsAmount;
		UILabel label_GemsDesc;




		static bool goBackToPopUp;

		static BackPopup backPopup;

		public enum BackPopup
		{
			IAP,
			Powerup,
			SkinShop
		}

		void Start()
		{
			
			transform.Find("Button_Claim").GetComponentInChildren<UILabel> ().text = Language.get ("Roulette.ClaimPrize");


			label_GemsAmount = transform.Find ("label_GemsAmount").GetComponent<UILabel>();
			label_GemsDesc = transform.Find ("label_GemsDesc").GetComponent<UILabel> ();



		}
		/*
		void OnEnable()
		{
			transform.Find("Sparkle").GetComponent<TweenScale>().enabled = true;
			transform.Find("Sparkle").GetComponent<TweenScale>().ResetToBeginning();
		}*/


		protected override void onShow()
		{
			

		}



		public void SetPopUp(int amount)
		{

			Audio.instance.playName ("daily_giftopen");

			label_GemsDesc.text = Language.get ("Roulette.YouGot") +" "+ Language.get ("Reward.Coins").Replace("%",amount.ToString());
			label_GemsAmount.text = amount.ToString();
			DriftGameplayScreen.instance.freezeCoins = true;
			SaveGameSystem.instance.setCoins(SaveGameSystem.instance.getCoins() + amount);



		}

		public static void GoBackToPopup(BackPopup popup)
		{
			goBackToPopUp = true;
			backPopup = popup;
		}

		public static void CancelGoBackToPopup()
		{
			goBackToPopUp = false;
		}


		public override void hide()
		{
			base.hide();
			if (goBackToPopUp)
			{
				goBackToPopUp = false;
				if (backPopup == BackPopup.IAP) {
					PopupManager.instance.showPopup<IPopup_IAP> ();
				}

				if (backPopup == BackPopup.Powerup) {
					PopupManager.instance.showPopup<IPopup_Powerups> ();
				}
			}
		}

			
		public void onCloseClick()
		{	
			DriftGameplayScreen.instance.freezeCoins = false;

			hide ();

		}


	}

}