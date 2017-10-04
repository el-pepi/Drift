using UnityEngine;
using System.Collections;
using System;

namespace AFArcade {

public class Popup_Rate : IPopup_Rate
{
	[Tooltip("After how many games show the rate popup")]
	[HideInInspector]
	public int gameToShowRate;     // ArtikFlowConfiguration

	int currentStars;
	Transform stars;
	UIButton sendButton;

	protected override void Awake()
	{
		base.Awake();

		transform.Find("Label_RateAsk1").GetComponent<UILabel>().text = Language.get("Rate.RateAsk1");
		transform.Find("Label_RateAsk2").GetComponent<UILabel>().text = Application.productName.ToUpper();
		transform.Find("Label_RateAsk3").GetComponent<UILabel>().text = Language.get("Rate.RateAsk3");
		transform.Find("Button_Submit").Find("Label").GetComponent<UILabel>().text = Language.get("Rate.Submit");
			
		stars = transform.Find("Stars");
		sendButton = transform.Find("Button_Submit").GetComponent<UIButton>();
		sendButton.isEnabled = false;
		
		for (int i = 0; i < 5; i++)
			stars.Find("Sprite_Star" + i).Find("Sprite_Active").gameObject.SetActive(false);
	}

	void Start()
	{
		Texture icon = ArtikFlowArcade.instance.configuration.icon;
		transform.Find("Texture_Icon").GetComponent<UITexture>().mainTexture = icon;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			base.close();
	}

	void OnEnable()
	{
		setStars(0);
	}

	protected override void onShow()
	{
		base.onShow();

		SaveGameSystem.instance.setRated(true);
	}

	public void setStars(int count)
	{
		for (int i = 0; i < 5; i++)
		{
			Transform star = stars.Find("Sprite_Star" + i);
			if (i < count)
			{
				if (i >= currentStars)
				{
					star.Find("Sprite_Active").gameObject.SetActive(true);
					star.GetComponent<Animator>().Play("starOn", -1, 0f);
					star.GetComponent<ParticleSystem>().time = 0f;
					star.GetComponent<ParticleSystem>().Play();
				}
			}
			else
			{
				if (i < currentStars)
					star.GetComponent<Animator>().Play("starOff", -1, (count == 0) ? 1f : 0f);
			}
		}

		if (count > currentStars)
			Audio.instance.playName("rate_star");

		currentStars = count;

		sendButton.isEnabled = count > 0;
		sendButton.transform.GetChild(0).GetComponent<UILabel>().color = new Color(1f, 1f, 1f, (count > 0) ? 1f : 0.5f);
	}

	// --- Callbacks ---

	public void onSendClick()
	{
		Audio.instance.playName("button");

		if (currentStars >= 4)
		{
#if UNITY_ANDROID
			if (ArtikFlowArcade.instance.configuration.storeTarget == ArtikFlowArcadeConfiguration.StoreTarget.PLAYPHONE)
				Application.OpenURL(ArtikFlowArcade.instance.configuration.Playphone_StarUrl.Replace("%", Application.bundleIdentifier));
			else
				Application.OpenURL(ArtikFlowArcade.instance.configuration.Android_StarUrl.Replace("%", Application.bundleIdentifier));
#elif UNITY_IOS
			Application.OpenURL(ArtikFlowArcade.instance.configuration.iOS_StarUrl.Replace("%", ArtikFlowArcade.instance.configuration.iOS_StoreId));
#endif
		}

		base.hide();
	}

	public void onStar0Hover()
	{
		if (Input.GetMouseButton(0))
			setStars(1);
	}

	public void onStar1Hover()
	{
		if (Input.GetMouseButton(0))
			setStars(2);
	}

	public void onStar2Hover()
	{
		if (Input.GetMouseButton(0))
			setStars(3);
	}

	public void onStar3Hover()
	{
		if (Input.GetMouseButton(0))
			setStars(4);
	}

	public void onStar4Hover()
	{
		if (Input.GetMouseButton(0))
			setStars(5);
	}

	public void onStar0Click()
	{
		setStars(1);
	}

	public void onStar1Click()
	{
		setStars(2);
	}

	public void onStar2Click()
	{
		setStars(3);
	}

	public void onStar3Click()
	{
		setStars(4);
	}

	public void onStar4Click()
	{
		setStars(5);
	}

}

}