using UnityEngine;
using System.Collections;

namespace AFArcade {

public class FlowButtons : IFlowButtons
{
	GameObject buttonSoundOn;
	GameObject buttonSoundOff;
	Transform shopNewNotif;
	Transform statsNotif;

	UILabel labelVersion;

	float soundChangeStamp;
	int soundChangeCount;

	protected override void Awake()
	{
		base.Awake();

		buttonSoundOn = transform.Find("Button_SoundOn").gameObject;
		buttonSoundOff = transform.Find("Button_SoundOff").gameObject;
		shopNewNotif = transform.Find("Grid").Find("2 Button_Shop").Find("New");
		statsNotif = transform.Find("Grid").Find("3 Button_Stats").Find("New");
		labelVersion = transform.Find("Label_Version").GetComponent<UILabel>();
		labelVersion.text = "Base: " + AFBase.ArtikFlowBase.BASE_VERSION + "\nArcade: " + ArtikFlowArcade.VERSION;
	}

	protected override void Start()
	{
		base.Start();

		if (ArtikFlowArcade.instance.configuration.storeTarget == ArtikFlowArcadeConfiguration.StoreTarget.FRENCH_PREMIUM && ArtikFlowArcade.instance.configuration.FRENCH_PREMIUM_tryNBuy)
			AFBase.TryNBuy.instance.eventGamePurchased.AddListener(onTryNBuyPurchased);

		refreshVisibleButtons();
	}

	void OnEnable()
	{
		transform.Find("Grid").GetComponent<TweenPosition>().enabled = true;
		transform.Find("Grid").GetComponent<TweenPosition>().ResetToBeginning();
	}

	// --- Interface implementation ---

	protected override void onShow(ArtikFlowArcade.State oldState)
	{
		buttonSoundOff.GetComponent<Animator>().enabled = false;
		buttonSoundOn.GetComponent<Animator>().enabled = false;

		shopNewNotif.gameObject.SetActive( CharacterManager.instance.canBuyACharacter() );
		statsNotif.gameObject.SetActive( Arcade_BasePlayerStats.instance.shouldHighlightGameServices() );

		gameObject.SetActive(true);
	}

	protected override void onHide()
	{
		labelVersion.gameObject.SetActive(false);
		gameObject.SetActive(false);
	}

	protected override void onSound_turnOn()
	{
		ArtikFlowArcade.instance.switchSound(true);
		Audio.instance.setVolume(1f);
		buttonSoundOn.SetActive(true);
		buttonSoundOff.SetActive(false);

		buttonSoundOn.GetComponent<Animator>().enabled = true;

		// Version hidden show:

		if (Time.time - soundChangeStamp < 1f)
		{
			soundChangeCount++;

			if (soundChangeCount >= 10)
			{
				labelVersion.gameObject.SetActive(true);
			}
        }
		else
			soundChangeCount = 0;

		soundChangeStamp = Time.time;
	}

	protected override void onSound_turnOff()
	{
		ArtikFlowArcade.instance.switchSound(false);
		Audio.instance.setVolume(0f);
		buttonSoundOn.SetActive(false);
		buttonSoundOff.SetActive(true);

		buttonSoundOff.GetComponent<Animator>().enabled = true;
	}

	// ---

	void onTryNBuyPurchased()
	{
		LoadingScreen.instance.fadeInOut(() => {
			refreshVisibleButtons();
			ArtikFlowArcade.instance.setState(ArtikFlowArcade.State.START_SCREEN);
		});
    }

	void refreshVisibleButtons()
	{
		transform.Find("Grid").Find("2 Button_Shop").gameObject.SetActive(ArtikFlowArcade.instance.configuration.charactersEnabled);

		if (ArtikFlowArcade.instance.configuration.storeTarget == ArtikFlowArcadeConfiguration.StoreTarget.FRENCH_PREMIUM)
		{
			transform.Find("Grid").Find("0 Button_Like").gameObject.SetActive(false);
			transform.Find("Grid").Find("1 Button_Star").gameObject.SetActive(false);
			transform.Find("Grid").Find("3 Button_Stats").gameObject.SetActive(false);
			transform.Find("Grid").Find("4 Button_Noads").gameObject.SetActive(false);

			if (ArtikFlowArcade.instance.configuration.FRENCH_PREMIUM_tryNBuy && !SaveGameSystem.instance.getTryNBuyUnlocked())
			{
				transform.Find("Grid").Find("1 Button_TryNBuy").gameObject.SetActive(true);
				transform.Find("Grid").Find("3 Button_TryNBuySMS").gameObject.SetActive(AFBase.TryNBuy.instance.needSMSButton());
			}
			else
			{
				transform.Find("Grid").Find("1 Button_TryNBuy").gameObject.SetActive(false);
				transform.Find("Grid").Find("3 Button_TryNBuySMS").gameObject.SetActive(false);
			}

			transform.Find("Grid").Find("1 Button_TryNBuy").GetComponentInChildren<UILabel>().text = Language.get("TryNBuy.FullGame");
        }
		else
		{
			transform.Find("Grid").Find("0 Button_Like").gameObject.SetActive(true);
			transform.Find("Grid").Find("1 Button_Star").gameObject.SetActive(true);
			transform.Find("Grid").Find("3 Button_Stats").gameObject.SetActive(true);
			transform.Find("Grid").Find("4 Button_Noads").gameObject.SetActive(true);

			transform.Find("Grid").Find("1 Button_TryNBuy").gameObject.SetActive(false);
			transform.Find("Grid").Find("3 Button_TryNBuySMS").gameObject.SetActive(false);
		}

		transform.Find("Grid").GetComponent<UIGrid>().enabled = true;   // Execute grid reordering

		if (ArtikFlowArcade.instance.configuration.shopIcon != null)
		{
			transform.Find("Grid").Find("2 Button_Shop").Find("Icon").GetComponent<UITexture>().mainTexture = ArtikFlowArcade.instance.configuration.shopIcon;
		}
	}

	void likeUnlock(){
		foreach(Character c in CharacterManager.instance.characters){
			if( c.price == -1 && !CharacterManager.instance.isUnlocked(c) ){
				CharacterManager.instance.unlockCharacter(c);
				shopNewNotif.gameObject.SetActive(true);
			}
		}
	}

	// --- Callbacks ---

	public void onLikeClick()
	{
		base.like();

		Invoke("likeUnlock", 1f);
	}

	public void onStarClick()
	{
		base.rate();
	}

	public void onShopClick()
	{
		base.shop();
	}

	public void onStatsClick()
	{
		base.stats();
	}

	public void onNoadsClick()
	{
		base.iap();
	}

	public void onTryNBuy()
	{
		AFBase.TryNBuy.instance.tryNBuy();
	}

	public void onTryNBuySMS()
	{
		AFBase.TryNBuy.instance.tryNBuySMS();
	}

}

}