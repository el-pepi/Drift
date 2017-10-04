using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFArcade {

public class CrossPromoClickBlocker : MonoBehaviour 
{

	void Start()
	{
		gameObject.SetActive(false);

		vg_interstitial crosspromo = GameObject.FindObjectOfType<vg_interstitial>();

		VascoGames.MoreGames.MoreGamesManager.Instance.ShowEventHandler.AddListener(onVgMoreGamesShow);
		VascoGames.MoreGames.MoreGamesManager.Instance.CloseEventHandler.AddListener(onVgMoreGamesClose);

		if(crosspromo != null) {
			crosspromo.eventShowing.AddListener(onVgInterstitialShowEvent);
		}
	}

	void onVgInterstitialShowEvent(bool showing)
	{
		gameObject.SetActive(showing);
	}
	

	void onVgMoreGamesShow(){
		gameObject.SetActive(true);
	}

	void onVgMoreGamesClose(){
		gameObject.SetActive(false);
	}
}

}