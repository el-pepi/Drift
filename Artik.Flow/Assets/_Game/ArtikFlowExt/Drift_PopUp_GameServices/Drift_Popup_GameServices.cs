using UnityEngine;
using System.Collections;

namespace AFArcade {

public class Drift_Popup_GameServices: IPopup_GameServices
{


	protected override void Awake()
	{
		base.Awake();
	}

	void Start()
	{

		gameObject.SetActive(false);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			base.close();
	}

	protected override void onShow()
	{
		base.onShow();
	}

	// --- Callbacks ---

	public void onAchievementsClick()
	{
		AFBase.GameServices.instance.ShowAchievementsUI();
	}

	public void onLeaderboardClick()
	{
		AFBase.GameServices.instance.ShowLeaderboardUI();
	}

}

}