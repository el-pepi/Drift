using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;
using System.Collections.Generic;
using AFBase;

/* -------------------------------------------------------------
*	ArtikFlowArcade main script.
*	Zamaroht | January, 2016
*
*	Main ArtikFlowArcade class. Takes care of the synchronization with the game scene.
------------------------------------------------------------- */

namespace AFArcade {

public class ArtikFlowArcade : ArtikFlowGamemode
{
	public const string VERSION = "v1.7.3.2 2017-05-31";

	public static ArtikFlowArcade instance;

	// Nombre de la escena principal del juego. Debe contener un GameObject con un script que implemente ArtikFlowImplementation.
	private string gameScene;       // ArtikFlowConfiguration

	// Si es distinto a 0, se empieza con esta cantidad de coins.
	private int moneyCheat;         // ArtikFlowConfiguration

	ArtikFlowArcadeGame game;
	public ArtikFlowArcadeConfiguration configuration { get; private set; }   // Configuration obtained from the ArtikFlowImplementation
	public GameObject gameAudio { get; private set; }
	public bool madeNewHighscore { get; private set; }					// Flag setted to true if a new highscore has been made this game

	[HideInInspector]
	public int playsThisSession;
	[HideInInspector]
	public int revivesThisGame;
	int currentScore;
	Character currentCharacter;
	int currentGameMode;

	public enum State
	{
		LOADING,			// Used only for init
		START_SCREEN,
		GAMEPLAY_SCREEN,
		LOST_SCREEN,
		DAILY_SCREEN,
		SHOP_SCREEN,
		ROULETTE_SCREEN,
	}
	public State flowState { get; private set; }

	[System.Serializable]
	public class IntEvent : UnityEvent<int> { }
	[HideInInspector]
	public IntEvent eventScoreUpdate = new IntEvent();  // (int score)
	[HideInInspector]
	public UnityEvent eventGameFinish = new UnityEvent();

	public class StateEvent : UnityEvent<State, State> { }
	[HideInInspector]
	public StateEvent eventStateChange = new StateEvent();

	public class BoolEvent : UnityEvent<bool> { }
	[HideInInspector]
	public BoolEvent eventToggleSound = new BoolEvent();	// (bool toggle)

	bool tookScreenshot;


	const string unlockedModesKey = "UNLOCKED_MODES";
	List<int> unlockedModes;
	[System.NonSerialized]
	public IntEvent ModeUnlocked;

	int gemsThisMatch = 0;
	int boostersThisMatch = 0;

	void Awake()
	{
		if (instance != null)
			throw new Exception("[ERROR] There is more than one ArtikFlow instance in the game!");

		if(ArtikFlowBase.instance == null)
		{
			SceneManager.LoadScene("ArtikFlowBase");
			return;
		}

		instance = this;

		configuration = (ArtikFlowArcadeConfiguration) ArtikFlowBase.instance.configuration;

		gameScene = configuration.gameScene;
		moneyCheat = configuration.moneyCheat;
		if(configuration.gameAudio != null)
		{
			gameAudio = Instantiate(configuration.gameAudio);
			DontDestroyOnLoad(gameAudio);
		}

		PopupManager popupManager = GameObject.FindObjectOfType<PopupManager>();
		if (popupManager == null)
			throw new Exception("[ERROR] No 'PopupManager' has been found.");

		unlockedModes = new List<int>();
		ModeUnlocked = new IntEvent();

		if(ES2.Exists(unlockedModesKey)) {
			unlockedModes =  new List<int>(ES2.LoadArray<int>(unlockedModesKey));
		}
	}

	IEnumerator Start ()
	{
		flowState = State.LOADING;
		
		AsyncOperation async = SceneManager.LoadSceneAsync(gameScene, LoadSceneMode.Additive);
		yield return async;     // Waits for scene load to complete.

		SceneManager.SetActiveScene(SceneManager.GetSceneByName(gameScene));

		onSetCurrentGamemode(0);
		Invoke("initialize", 0.5f);
	}

	void initialize()
	{	
		eventScoreUpdate.Invoke(0);

		Debug.Log("[INFO] ArtikFlow Arcade initialized successfully. Implementation found: " + game);

		currentCharacter = CharacterManager.instance.getSelectedCharacter();
		setState(State.START_SCREEN);

		switchSound(true);

		if (moneyCheat > 0)
			SaveGameSystem.instance.setCoins(moneyCheat);
		SaveGameSystem.instance.setSessionCount(SaveGameSystem.instance.getSessionCount() + 1);

		eventInitialized.Invoke();
	}

	/// <summary> Listener for game.eventSetCurrentGamemode </summary>
	public void onSetCurrentGamemode(int newGamemodeId,bool reset = false)
	{
		if(game != null) {
			game.eventFinish.RemoveAllListeners();
			game.eventResetReady.RemoveAllListeners();
			game.eventAddCoins.RemoveAllListeners();
			game.eventAddScore.RemoveAllListeners();
			game.eventTakeScreenshot.RemoveAllListeners();
			game.eventUsePowerup.RemoveAllListeners();
			game.eventNeedGems.RemoveAllListeners();
			game.eventToggleSound.RemoveAllListeners();
			//game.eventSetCurrentGamemode.RemoveAllListeners();

			game.OnChanged();

			Destroy(game.gameObject);
			game = null;
		}

		if(configuration.gameModes.Length == 0) {
			game = GameObject.FindObjectOfType<ArtikFlowArcadeGame>();
		} else {
			game = Instantiate( configuration.gameModes[newGamemodeId].gameObject).GetComponent<ArtikFlowArcadeGame>();
		}

		if(game == null) {
			throw new Exception("[ERROR] Couldn't find an ArtikFlowImplementation in the scene: " + gameScene);
		}

		game.eventFinish.AddListener(onGameFinish);
		game.eventResetReady.AddListener(onResetReady);
		game.eventAddCoins.AddListener(onAddCoins);
		game.eventAddScore.AddListener(onAddScore);
		game.eventTakeScreenshot.AddListener(onTakeScreenshot);
		game.eventUsePowerup.AddListener(onUsePowerup);
		game.eventNeedGems.AddListener(onNeedGems);
		game.eventToggleSound.AddListener(onSoundToggled);
		//game.eventSetCurrentGamemode.AddListener(onSetCurrentGamemode);

		if(reset){
			game.reset(currentCharacter);
		}
		
		SaveGameSystem.instance.setGamemodeId(newGamemodeId);
		currentGameMode = newGamemodeId;
	}

	public int getCurrentGameMode(){
		return currentGameMode;
	}

	public void setState(State newState)
	{
		State oldState = flowState;

		// Manage the old State

		if (oldState == State.LOADING)
		{
			
		}
		else if (oldState == State.START_SCREEN)
		{
			
		}
		else if (oldState == State.GAMEPLAY_SCREEN)
		{
			
		}
		else if (oldState == State.LOST_SCREEN)
		{

		}
		else if (oldState == State.DAILY_SCREEN)
		{
			
		}
		else if (oldState == State.SHOP_SCREEN)
		{

		}

		// Manage the new State

		if (newState == State.START_SCREEN)
		{
			revivesThisGame = 0;
			if(oldState != State.SHOP_SCREEN)		// If oldState == SHOP, the game.reset() gets called in ArtikFlow.setCharacter()
				game.reset(currentCharacter);
		}
		else if (newState == State.GAMEPLAY_SCREEN)
		{
			tookScreenshot = false;
            game.play();
		}
		else if (newState == State.LOST_SCREEN)
		{
			playsThisSession++;
			SaveGameSystem.instance.setGamesPlayed(SaveGameSystem.instance.getGamesPlayed() + 1);

			if (playsThisSession >= configuration.gameToShowRate && !configuration.disableRatePopup && !SaveGameSystem.instance.hasRated() && configuration.storeTarget != ArtikFlowArcadeConfiguration.StoreTarget.FRENCH_PREMIUM)
				PopupManager.instance.showPopup<IPopup_Rate>();
			else if (Arcade_Purchaser.instance.isDuringHolidayPack() && playsThisSession >= 1 && playsThisSession % configuration.IAPPromoGameToShow == 0)
				PopupManager.instance.showPopup<IPopup_HalfPack>();
			/*
			else if (playsThisSession == configuration.gameToShowIAP && SaveGameSystem.instance.getSessionCount() <= 1)
				PopupManager.instance.showPopup<IPopup_IAP>();
			*/
		}
		else if (newState == State.DAILY_SCREEN)
		{
			
		}
		else if (newState == State.SHOP_SCREEN)
		{
			
		}
		
		flowState = newState;

		eventStateChange.Invoke(oldState, newState);
	}

	// --------------------------- Functions to use from the game scene

	public int getScore()
	{
		return currentScore;
	}

	public Character getCurrentCharacter()
	{
		return currentCharacter;
	}

	// --------------------------- Other functions

	public void setCharacter(Character newCharacter)
	{
		currentCharacter = newCharacter;
		CharacterManager.instance.setSelectedCharacter(newCharacter);

		game.reset(currentCharacter);
	}

	public void terminateGame()
	{
		if (!tookScreenshot)
		{
			// PopupManager.instance.transform.Find("Background").gameObject.SetActive(false);

			ScreenshotTaker.instance.takeScreenshot(() => {
				terminateGame();
			});
			tookScreenshot = true;
			return;
		}
		// PopupManager.instance.transform.Find("Background").gameObject.SetActive(true);

		setState(State.LOST_SCREEN);
		eventGameFinish.Invoke();

		// Report score to game services
		int score = ArtikFlowArcade.instance.getScore();
		GameServices.instance.ReportScore(SaveGameSystem.instance.currentGamemodeId, score);

		// Report to analytics
		//GoogleAnalyticsV4.instance.LogEvent("Gameplay", "Finish", "Score", getScore());
		GoogleAnalyticsV4.instance.LogEvent("social", "impression", "result share impression", gemsThisMatch);

		/*
		Analytics.CustomEvent("gameOver", new Dictionary<string, object>
		{
			{ "score", getScore() },
			{ "highscore", SaveGameSystem.instance.getHighScore() },
			{ "totalCoins", SaveGameSystem.instance.getCoins() },
			{ "sessionCount", SaveGameSystem.instance.getSessionCount() },
			{ "unlockedChars", SaveGameSystem.instance.getUnlockedChars().Length },
		});*/

		// Stop Replay
		Invoke("stopRecording", 2f);
	}

	void stopRecording()
	{
		Replay.instance.stopRecording();
	}

	/// <summary> Returns wheter the player can revive this session or not </summary>
	public bool canRevive()
	{
		return (revivesThisGame == 0) && 
			(getScore() >= SaveGameSystem.instance.getHighScore() / 2f || 
			Arcade_BasePlayerStats.instance.alwaysRevive());
	}

	public void revive()
	{
		Popup currentPopup = PopupManager.instance.getCurrentPopup();
		if (currentPopup is IPopup_Revive)
			currentPopup.hide();
        
		revivesThisGame++;
        game.revive();
	}

	public void switchSound(bool state)
	{
		game.switchSound(state);
	}

	// --------------------------- Callbacks from the game implementation

	/// <summary> Listener for game.eventFinish </summary>
	void onGameFinish()
	{
		if (flowState != State.GAMEPLAY_SCREEN || PopupManager.instance.getCurrentPopup() is IPopup_Revive)
			return;
		
		if (canRevive())
		{
			bool res = PopupManager.instance.showPopup<IPopup_Revive>();
			if(!res)
				terminateGame();
		}
		else
			terminateGame();
    }

	/// <summary> Listener for game.eventResetReady </summary>
	void onResetReady()
	{
		currentScore = 0;
		madeNewHighscore = false;
		eventScoreUpdate.Invoke(currentScore);
		
		if(gemsThisMatch > 0) {
			GoogleAnalyticsV4.instance.LogEvent("session", "gems", "average gems per match", gemsThisMatch);
			gemsThisMatch = 0;
		}
		if(boostersThisMatch > 0) {
			GoogleAnalyticsV4.instance.LogEvent("boosters", "use booster", "average boosters used per match", gemsThisMatch);
			boostersThisMatch = 0;
		}
	}

	/// <summary> Listener for game.eventAddCoins </summary>
	void onAddCoins(int count)
	{
		SaveGameSystem.instance.setCoins(SaveGameSystem.instance.getCoins() + count);
		gemsThisMatch += count;
	}

	/// <summary> Listener for game.eventAddScore </summary>
	void onAddScore(int count)
	{
		if (flowState != State.GAMEPLAY_SCREEN)
			return;

		currentScore += count;
		if (currentScore > SaveGameSystem.instance.getHighScore())
			madeNewHighscore = true;

		SaveGameSystem.instance.submitScore(currentScore);		// Updates highscore if bigger

        eventScoreUpdate.Invoke(currentScore);
	}

	/// <summary> Listener for game.eventTakeScreenshot </summary>
	void onTakeScreenshot()
	{
		ScreenshotTaker.instance.takeScreenshot();
		tookScreenshot = true;
	}

	/// <summary> Listener for game.eventUsePowerup </summary>
	void onUsePowerup(string powerupId, int count)
	{
		if (count <= 0)
		{
			Debug.LogError("Invalid call to eventUsePowerup, count <= 0.");
			return;
		}
		else
		{
			bool found = false;
			foreach(Powerup p in configuration.powerups)
			{
				if (p.internalId == powerupId)
				{
					found = true;
					break;
				}
			}

			if(!found)
			{
				Debug.LogError("Invalid call to eventUsePowerup, powerup '" + powerupId + "' doesn't exist. Is it added in the ArtikFlowConfiguration?");
				return;
			}
		}

		int current = SaveGameSystem.instance.getPowerupCount(powerupId);

		if(current - count < 0)
        {
			Debug.LogError("Error using powerup '" + powerupId + "', count: " + count + "; only " + current + " available! Are you checking SaveGameSystem.getPowerupCount before calling the event?");
			return;
        }

		SaveGameSystem.instance.setPowerupCount(powerupId, current - count);
		
		boostersThisMatch += count;
	}

	/// <summary> Listener for game.eventNeedGems </summary>
	void onNeedGems()
	{
		if(flowState == State.GAMEPLAY_SCREEN && configuration.enableCoins && configuration.gemPackCount > 0)
			PopupManager.instance.showPopup<IPopup_NeedGems>();
	}

	/// <summary> Listener for game.eventToggleSound </summary>
	void onSoundToggled(bool toggle)
	{
		eventToggleSound.Invoke(toggle);
	}

	public void increaseThresholdCounter(int mode){
		// Increases the counter that takes acount of the steps to reach the mode unlock threshold
		int counter = 0;
			if( ES2.Exists("save.artik?tag=" + "mode" + mode) ){
				counter = ES2.Load<int>("save.artik?tag=" + "mode" + mode);
		}
		counter++;
		ES2.Save<int>(counter, "save.artik?tag=" + "mode" + mode);

		if( counter >= configuration.gameModes[mode].unlockThreshold ){
			unlockMode(mode);
			GoogleAnalyticsV4.instance.LogEvent("missions", "unlock", "gamemode " + configuration.gameModes[mode].gameObject.name + " unlocked mission", 0);
		}
	}

	public int GetThresholdCounter(int mode){
		if(ES2.Exists("save.artik?tag=" + "mode" + mode)) {
			return ES2.Load<int>("save.artik?tag=" + "mode" + mode);
		}
		return 0;
	}


	public void unlockMode(int mode){
		unlockedModes.Add(mode);

		ES2.Save(unlockedModes.ToArray(), unlockedModesKey);

		ModeUnlocked.Invoke(mode);
	}

	public bool IsUnlocked(int mode){
		return unlockedModes.Contains(mode);
	}

	public int GetThreshold(int mode){
		return configuration.gameModes[mode].unlockThreshold;
	}

	public bool BuyModeWithGems(int mode){
		if(SaveGameSystem.instance.getCoins() >= configuration.gameModes[mode].unlockGems) {
			SaveGameSystem.instance.setCoins(SaveGameSystem.instance.getCoins() - configuration.gameModes[mode].unlockGems);
			unlockMode(mode);
			GoogleAnalyticsV4.instance.LogEvent("missions", "unlock", "gamemode " + configuration.gameModes[mode].gameObject.name + " unlocked gems", 0);

			return true;
		}
		return false;
	}

	public int GetGemsToBuyMode(int mode){
		return configuration.gameModes[mode].unlockGems; 
	}

	void OnApplicationQuit()
	{
		if(playsThisSession > 0) {
			GoogleAnalyticsV4.instance.LogEvent("session", "matches", "average matches per session", playsThisSession);
		}
	}
}

}