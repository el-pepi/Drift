using UnityEngine;
using System.Collections;

namespace AFArcade {

public class Popup_NeedGems : IPopup_NeedGems
{
	public static bool goBackToPowerups;

	UILabel gemsPrice;

	protected override void Awake()
	{
		base.Awake();

		// Localization
		transform.Find("Row_IAPs").Find("Button_Gems").Find("Label_Details").GetComponent<UILabel>().text = Language.get("IAP.Gems").Replace("%", "" + ArtikFlowArcade.instance.configuration.gemPackCount);
		gemsPrice = transform.Find("Row_IAPs").Find("Button_Gems").Find("Label_Price").GetComponent<UILabel>();
		
		transform.Find("Row_Back").Find("Label_Notice").GetComponent<UILabel>().text = Language.get("IAP.NeedGems");

		gemsPrice.text = "...";
	}

	void Start()
	{
		AFBase.Purchaser.instance.eventPurchased.AddListener(onPurchased);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			base.close();
	}

	protected override void onShow()
	{
		base.onShow();

		gemsPrice.text = AFBase.Purchaser.instance.localizedPrice("gems");
		gemsPrice.transform.parent.GetComponent<UIButton>().isEnabled = true;
	}

	protected override void onHide()
	{
		base.onHide();

		if(goBackToPowerups)
		{
			goBackToPowerups = false;
			PopupManager.instance.showPopup<IPopup_Powerups>();
		}
	}

	// --- Callbacks ---

	public void onGems()
	{
		AFBase.Purchaser.instance.BuyProductID("gems");
	}

	void onPurchased(string productId)
	{
		// The IAP is processed and saved in Purchaser.cs

		if (productId == "gems")
			base.hide();
	}

}

}