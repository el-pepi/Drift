using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFArcade
{

public class DriftShopScreen : IShopScreen 
{
    // Config
    private int VIDEO_REWARD = 20;
    private float CAR_ROTATION_SPEED = 35f;
		public static DriftShopScreen instance;
    // Refs
    UIScrollView carScrollView;
    UIGrid carGrid;

    UILabel coinsLabel;

    UILabel nameLabel;
    UILabel descriptionLabel;
    UILabel tierLabel;
    UISprite lockSprite;
    UITexture defenseTexture;
    UITexture weaponTexture;
    //UIGrid statsGrid;

    GameObject leftArrow;
    GameObject rightArrow;
    UIGrid buttonHolder;
    UIButton buttonVideo;
    UIButton buttonGems;
    UIButton buttonIAP;
    UIButton buttonPlay;

    Transform car3DHolder;
    Quaternion originalHolderRotation;
    GameObject car3DInstance;

    // State
    DriftCharacter selectedCar;
	
	WeaponsEquipped wEquipped;
	
	public BarShop healthBarShop;
	
	public BarShop damageBarShop;

	ShopTierManager shopTierManager;

	protected override void Awake()
    {
        base.Awake();
		
		instance = this;

		shopTierManager = GetComponent<ShopTierManager> ();
        // Refs
        coinsLabel = transform.Find("Top").Find("Coins").Find("Label").GetComponent<UILabel>();

       // statsGrid = transform.Find("Middle").Find("Grid").GetComponent<UIGrid>();
			/*
        defenseTexture = statsGrid.transform.Find("Stats").Find("Defense").Find("Texture_Desc").GetComponent<UITexture>();
        weaponTexture = statsGrid.transform.Find("Stats").Find("Weapon").Find("Texture_Desc").GetComponent<UITexture>();
			*/
        nameLabel = transform.Find("Middle").Find("Title").Find("Label_Title").GetComponent<UILabel>();
		descriptionLabel = transform.Find("Middle").Find("LegendaryDescription").Find("Description").GetComponent<UILabel>();
        tierLabel = transform.Find("Middle").Find("Title").Find("Label_Tier").GetComponent<UILabel>();
        lockSprite = transform.Find("Middle").Find("Title").Find("Sprite_Locked").GetComponent<UISprite>();

        leftArrow = transform.Find("Bottom").Find("Arrows").Find("Left").gameObject;
        rightArrow = transform.Find("Bottom").Find("Arrows").Find("Right").gameObject;

        buttonHolder = transform.Find("Middle").Find("BuyButtons").GetComponent<UIGrid>();
		buttonVideo = buttonHolder.transform.Find("Button_Video").GetComponentInChildren<UIButton>();
		buttonGems = buttonHolder.transform.Find("Button_Gems").GetComponentInChildren<UIButton>();
		buttonIAP = buttonHolder.transform.Find("Button_IAP").GetComponentInChildren<UIButton>();
		buttonPlay = buttonHolder.transform.Find("Button_Play").GetComponentInChildren<UIButton>();
			/*
        statsGrid.transform.Find("Stats").Find("Defense").Find("Label_Title").GetComponent<UILabel>().text = Language.get("Drift.Defense");
        statsGrid.transform.Find("Stats").Find("Weapon").Find("Label_Title").GetComponent<UILabel>().text = Language.get("Drift.Weapons");
			*/
        buttonVideo.transform.Find("Label_VideoEarn").GetComponent<UILabel>().text = Language.get("IAP.NeedGems");

        carScrollView = transform.Find("Bottom").Find("Scroll View").GetComponent<UIScrollView>();
		
		wEquipped = transform.GetComponentInChildren<WeaponsEquipped> ();
		
		//healthBarShop = transform.Find("Middle").Find("Bars").Find("HealthBar").GetComponent<BarShop> ();
        // Car holder
        car3DHolder = transform.Find("Car3D").Find("CarHolder");
        originalHolderRotation = car3DHolder.transform.localRotation;
        if(car3DHolder.childCount > 0)
            Destroy(car3DHolder.GetChild(0).gameObject);
    }

    protected new void Start()
    {
        base.Start();
        
        SaveGameSystem.instance.eventCoinsUpdate.AddListener(onCoinsUpdated);
        AFBase.Purchaser.instance.eventPurchased.AddListener(onIAPPurchase);
        
        populateCharacters();
        selectCharacter(0);
    }

    void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && PopupManager.instance.getCurrentPopup() == null)
			onBackClick();

        // Dynamic drag update
        if(carScrollView.isDragging)
        {
            float lengthPerCar = carGrid.cellWidth;
            int centeredCarId = Mathf.RoundToInt((-carScrollView.transform.localPosition.x - lengthPerCar) / lengthPerCar);
            if(centeredCarId < 0)
                centeredCarId = 0;
            else if(centeredCarId > ArtikFlowArcade.instance.configuration.characters.Length - 1)
                centeredCarId = ArtikFlowArcade.instance.configuration.characters.Length - 1;

            if(selectedCar.id != centeredCarId)
                updateInterface(carGrid.GetChild(centeredCarId).gameObject);
        }
	}

    void OnEnable()
    {
        StartCoroutine(car3DLoader());
        StartCoroutine(carRotation());
        StartCoroutine(rewardedVideoChecker());
    }

    void OnDisable()
    {
        if(car3DInstance != null)
            Destroy(car3DInstance);

        Resources.UnloadUnusedAssets();
    }

    // -------------------------------

    protected void populateCharacters()
    {
        // Populate characters
        carGrid = transform.Find("Bottom").Find("Scroll View").Find("Grid").GetComponent<UIGrid>();
		GameObject template = carGrid.transform.Find("Template").gameObject;

		foreach (DriftCharacter c in CharacterManager.instance.characters)
		{
			GameObject item = NGUITools.AddChild(template.transform.parent.gameObject, template);
            item.transform.Find("Texture").GetComponent<UITexture>().mainTexture = c.menuTexture;
            item.transform.Find("Label_Name").GetComponent<UILabel>().text = /*(c.id + 1 < 10 ? "0" : "") + (c.id + 1) + "- " + */c.carName;
			item.name = "" + c.id + "." + c.carName;
            item.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

            updateGridCharacter(item);
		}

		Destroy(template);

		carGrid.GetComponent<UIGrid>().enabled = true;
        carGrid.GetComponent<UICenterOnChild>().onCenter += onCharSelected;
    }

    protected void selectCharacter(int charId)
    {
        Transform uiCar = carGrid.transform.GetChild(charId);

        carGrid.GetComponent<UICenterOnChild>().CenterOn(uiCar);
        updateInterface(uiCar.gameObject);
    }
	

	

    protected void updateInterface(GameObject UIChar)
    {
        int id;
		bool success = int.TryParse(UIChar.name.Split('.')[0], out id);
		if (!success)
			return;

        selectedCar = (DriftCharacter) CharacterManager.instance.getCharacter(id);
        

        // Description
        nameLabel.text = selectedCar.carName;
		
		wEquipped.SetWeaponsActive (selectedCar.weaponsIconsActive);

		healthBarShop.UpdateBar (selectedCar.healthShopBar);

		damageBarShop.UpdateBar (selectedCar.damageShopBar);

		shopTierManager.SetShopTier (selectedCar.tierString);

        if(selectedCar.descriptionKey.Length > 0)
        {
				
            descriptionLabel.transform.parent.gameObject.SetActive(true);
			descriptionLabel.text =	Language.get(selectedCar.descriptionKey);
				
        }
        else
            descriptionLabel.transform.parent.gameObject.SetActive(false);
			
      //  statsGrid.Reposition();
		tierLabel.text = Language.get(selectedCar.tierString);
		lockSprite.gameObject.SetActive(!CharacterManager.instance.isOwned(selectedCar));

       // defenseTexture.mainTexture = selectedCar.defenseTexture;
        //weaponTexture.mainTexture = selectedCar.weaponTexture;

        // Arrows
        leftArrow.GetComponent<UISprite>().enabled = (getSelectedTransform().GetSiblingIndex() >= 1);
        rightArrow.GetComponent<UISprite>().enabled = (getSelectedTransform().GetSiblingIndex() < getSelectedTransform().parent.childCount - 1);

        // Update character colliders
        //UIChar.GetComponent<BoxCollider>().size = new Vector3(219 - 30, 160, 0);
        iTween.ScaleTo(UIChar, new Vector3(1.1f, 1.1f, 1.1f), 0.35f);
        int idx = UIChar.transform.GetSiblingIndex() - 1;
        if(idx >= 0)
        {
            Transform sibling = UIChar.transform.parent.GetChild(idx);
            //sibling.GetComponent<BoxCollider>().size = new Vector3(219 + 30, 160, 0);
            iTween.ScaleTo(sibling.gameObject, new Vector3(0.8f, 0.8f, 0.8f), 0.35f);
        }   
        idx = UIChar.transform.GetSiblingIndex() + 1;
        if(idx < UIChar.transform.parent.childCount)
        {
            Transform sibling = UIChar.transform.parent.GetChild(idx);
            //sibling.GetComponent<BoxCollider>().size = new Vector3(219 + 30, 160, 0);
            iTween.ScaleTo(sibling.gameObject, new Vector3(0.8f, 0.8f, 0.8f), 0.35f);
        }

        // Buttons
		if(CharacterManager.instance.isOwned(selectedCar))
        {
            buttonPlay.gameObject.SetActive(true);
            buttonGems.gameObject.SetActive(false);
            buttonVideo.gameObject.SetActive(false);
            buttonIAP.gameObject.SetActive(false);
        }
        else
        {
            buttonPlay.gameObject.SetActive(false);
            buttonGems.gameObject.SetActive(true);

			buttonGems.transform.GetComponentInChildren<UILabel>().text = "" + selectedCar.price;
            if(selectedCar.iapProduct == null)
            {
                buttonVideo.gameObject.SetActive(true);
                buttonIAP.gameObject.SetActive(false);
            }
            else
            {
                string price = AFBase.Purchaser.instance.localizedPrice(selectedCar.iapProduct.productID);
                if(price == "...")
                    buttonIAP.GetComponentInChildren<UILabel>().text = Language.get("HalfPack.Purchase");
                else
                    buttonIAP.GetComponentInChildren<UILabel>().text = price;

                buttonIAP.gameObject.SetActive(true);
                buttonVideo.gameObject.SetActive(false);
            }
        }

        buttonHolder.Reposition();
    }
	
    protected void updateGridCharacter(GameObject UIChar)
    {
        int id;
		bool success = int.TryParse(UIChar.name.Split('.')[0], out id);
		if (!success)
			return;

        DriftCharacter c = (DriftCharacter) CharacterManager.instance.getCharacter(id);
		UIChar.transform.Find("Sprite_Lock").gameObject.SetActive(!CharacterManager.instance.isOwned(c));
    }

    void buy(Character c)
    {
        // Can buy?
		if (c.price < 0)
		{
            base.unlockCharacter(c);

			UnityEngine.Analytics.Analytics.CustomEvent("characterSocialUnlock", new Dictionary<string, object> {
				{ "network", (c.price == -1 ? "facebook" : (c.price == -2 ? "instagram" : (c.price == -3 ? "twitter" : "unknown"))) },
			});

			if (c.price == -1)
				Application.OpenURL(ArtikFlowArcade.instance.configuration.Facebook_buy_Url);
			else if (c.price == -2)
				Application.OpenURL(ArtikFlowArcade.instance.configuration.Instagram_buy_Url);
			else if (c.price == -3)
				Application.OpenURL(ArtikFlowArcade.instance.configuration.Twitter_buy_Url);

			StartCoroutine(unlockLike(c));
		}
		else if (SaveGameSystem.instance.getCoins() >= c.price)
		{
			Audio.instance.playName("buy_button");
			SaveGameSystem.instance.setCoins(SaveGameSystem.instance.getCoins() - c.price);
			unlock(c);
		}
		else
		{
			Audio.instance.playName("buy_button_disabled");
			if(Arcade_Purchaser.instance.isDuringStarterPack())
				PopupManager.instance.showPopup<IPopup_HalfPack>();
			else
				PopupManager.instance.showPopup<IPopup_IAP>();
		}
    }

    public void unlock(Character c)
	{
		CharacterManager.instance.purchaseCharacter(c);
		//base.unlockCharacter(c);
        // Get grid char:
        Transform gridChar = null;
        foreach(Transform t in carGrid.transform)
        {
            if(t.name.StartsWith(c.id + "."))
            {
                gridChar = t;
                break;
            }
        }
    	
        if(gridChar != null)
        {
            updateInterface(gridChar.gameObject);
            updateGridCharacter(gridChar.gameObject);
        }
		

	}

	IEnumerator unlockLike(Character c)
	{
		yield return new WaitForSeconds(2f);
		unlock(c);
	}

    // ------------------------------- Co-routines

    IEnumerator rewardedVideoChecker()
    {
        bool prev;

        while(true)
        {
            prev = buttonVideo.isEnabled;
            buttonVideo.isEnabled = AFBase.Ads.instance.isRewardedVideoAvailable();

            if(prev != buttonVideo.isEnabled)
            {
                if(buttonVideo.isEnabled)
                    buttonVideo.transform.Find("Label_VideoEarn").GetComponent<UILabel>().text = Language.get("IAP.NeedGems");
                else
                    buttonVideo.transform.Find("Label_VideoEarn").GetComponent<UILabel>().text = Language.get("Drift.Unavailable");
            }
            
            yield return new WaitForSeconds(2.5351f);
        }
    }

    IEnumerator car3DLoader()
    {
        ResourceRequest carLoadRequest = null;
        DriftCharacter charOnLoad = null;
        bool showed = false;

        while(true)
        {
            if(selectedCar != charOnLoad)
            {
                charOnLoad = selectedCar;
                showed = false;
                carLoadRequest = Resources.LoadAsync<GameObject>(selectedCar.shopCarResource);

                // Show loading indicator?
            }

            if(carLoadRequest != null && carLoadRequest.isDone && !showed)
            {
                showCar(selectedCar, (GameObject) carLoadRequest.asset);
                showed = true;
            }
            
            yield return null;      // Once per frame
        }
    }

    IEnumerator carRotation()
    {
        DriftCharacter rotatingCar = selectedCar;
        bool manual_rotation = false;
        bool performing_rotation = false;
        Vector3 last_position = Vector3.zero;
        Vector3 accumulated_delta = Vector3.zero;
        float lastInputStamp = 0f;

        while(true)
        {
            if(rotatingCar != selectedCar)     // Car changed
            {
                /*manual_rotation = false;
                performing_rotation = false;
                accumulated_delta = Vector3.zero;*/
                rotatingCar = selectedCar;
            }

            if(manual_rotation)         // Manual rotation
            {
                if(performing_rotation)     // Currently rotating
                {
                    if(Input.GetMouseButtonUp(0))
                    {
                        lastInputStamp = Time.time;
                        performing_rotation = false;
                    }

                    if(Input.GetMouseButton(0))
                    {
                        accumulated_delta += (Input.mousePosition - last_position) / Screen.width * 400f;
                        last_position = Input.mousePosition;
                    }
                }
                else                        // Waiting for input
                {
                    if(initiatingCarRotation())
                    {
                        last_position = Input.mousePosition;
                        performing_rotation = true;
                    }
                    else if(Time.time - lastInputStamp > 1.5f)
                        manual_rotation = false;
                }
            }
            else                        // Automatic rotation
            {
                accumulated_delta += new Vector3(-CAR_ROTATION_SPEED * Time.deltaTime, 0f, 0f);

                if(initiatingCarRotation())
                {
                    last_position = Input.mousePosition;
                    manual_rotation = true;
                    performing_rotation = true;
                }
            }

            // Slowly deploy the accumulated_delta
            Vector3 to_deploy = accumulated_delta / 6f;
            accumulated_delta -= to_deploy;
            car3DHolder.Rotate(0f, -to_deploy.x, 0f);

            yield return null;      // Wait for next frame
        }

    }

    bool initiatingCarRotation()
    {
        return Input.GetMouseButtonDown(0) && Input.mousePosition.y > Screen.height / 2f && Input.mousePosition.y < Screen.height / 5f * 4f;
    }

    void showCar(DriftCharacter character, GameObject car3DAsset)
    {
        if(car3DInstance != null)
            Destroy(car3DInstance);

        car3DInstance = Instantiate(car3DAsset);
        car3DInstance.transform.name = character.carName;
        car3DInstance.transform.parent = car3DHolder;
        car3DInstance.transform.localPosition = Vector3.zero;
        car3DInstance.transform.localScale = Vector3.one;
        car3DInstance.transform.localRotation = Quaternion.identity;
    }

    Transform getSelectedTransform()
    {
        // return carGrid.GetComponent<UICenterOnChild>().centeredObject.transform;
        return carGrid.GetChild(selectedCar.id);
    }

    // -------------------------------

    protected override void onHide()
    {
        gameObject.SetActive(false);
    }

    protected override void onShow(ArtikFlowArcade.State oldState)
    {
			
        GetComponent<Animator>().enabled = true;
        gameObject.SetActive(true);

		SetListenerVideoReward ();

        selectCharacter(ArtikFlowArcade.instance.getCurrentCharacter().id);
        onCoinsUpdated(SaveGameSystem.instance.getCoins());

        car3DHolder.transform.localRotation = originalHolderRotation;
		
		if(oldState == ArtikFlowArcade.State.LOST_SCREEN)
		GameManager.instance.reset (selectedCar);
    }

    void onIAPPurchase(string productID)
    {
        foreach (DriftCharacter c in CharacterManager.instance.characters)
        {
            if(c.iapProduct != null && c.iapProduct.productID == productID)
            {
                unlock(c);
                return;
            }
        }
    }

    void onCoinsUpdated(int coins)
    {
        coinsLabel.text = "" + coins;
    }

    void onCharSelected(GameObject UIChar)
	{
        updateInterface(UIChar);
	}

    public void onAnimInFinish()
	{
		// GetComponent<Animator>().enabled = false;
	}

	public void onAnimOutFinish()
	{
		base.goBack();
		gameObject.SetActive(false);
	}

    // -------------------------------

    public void onBackClick()
    {
        // gameObject.SetActive(false);
       // base.selectCharacter(selectedCar);
		
        GetComponent<Animator>().enabled = true;
		GetComponent<Animator>().Play("DriftShopOut", -1, 0f);
    }

    public void onVideoClick()
    {
		AFBase.Ads.instance.ShowRewardedVideo ();
    }
	
	void GiveReward(int reward)
	{

		PopupManager.instance.showPopup<Popup_Reward_Custom> ();

		Popup_Reward_Custom popupReward = (Popup_Reward_Custom)PopupManager.instance.getCurrentPopup ();

		popupReward.SetPopUp (reward);
	}

	void SetListenerVideoReward()
	{

		AFBase.Ads.RewardedVideoListener listener = delegate (AFBase.Ads.RewardedVideoResult result) {

			if(result == AFBase.Ads.RewardedVideoResult.AVAILABLE)
			{

			}

			if(result == AFBase.Ads.RewardedVideoResult.SUCCESS)
			{

				Popup_Reward_Custom.GoBackToPopup (Popup_Reward_Custom.BackPopup.IAP);
				GiveReward(ArtikFlowArcade.instance.configuration.videoReward);

				GoogleAnalyticsV4.instance.LogEvent("video_reward", "play", "results video reward", 0);



			}
			else if (result == AFBase.Ads.RewardedVideoResult.SKIPPED)
			{

			}
			else if(result == AFBase.Ads.RewardedVideoResult.FAIL)
			{

			}
		};
		AFBase.Ads.instance.SetRewardedVideoCallback(listener);

	}
    public void onGemsClick()
    {
        buy(selectedCar);
    }

    public void onIAPClick()
    {
        AFBase.Purchaser.instance.BuyProductID(selectedCar.iapProduct.productID);
    }

    public void onLeftClick()
    {
        scroll(-1);
    }

    public void onRightClick()
    {
        scroll(1);
    }

    // 1 for right, -1 for left
    void scroll(int direction)
    {
        Transform selected = getSelectedTransform();
        int newIndex = selected.GetSiblingIndex() + direction;
        
        selectCharacter(newIndex);
    }
    
    public void onSelectClick()
    {
        updateInterface(getSelectedTransform().gameObject);
    }

    public void onPlayClick()
    {
        base.selectCharacter(selectedCar);
        onBackClick();
    }

}

}