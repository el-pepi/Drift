using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Events;
using System.Security.Cryptography;
using System.Text;

namespace AFArcade {

public class SaveGameSystem : MonoBehaviour
{
	public static SaveGameSystem instance;

	[System.Serializable]
	public class IntEvent : UnityEvent<int> { }
	[HideInInspector]
	public IntEvent eventCoinsUpdate = new IntEvent();      // (int coins)
	[HideInInspector]
	public IntEvent eventHighscoreUpdate = new IntEvent();  // (int newHighscore)
	[HideInInspector]
	public UnityEvent eventDailyGiftUpdate = new UnityEvent();
	[HideInInspector]
	public IntEvent eventGameModeChanged = new IntEvent();

	public int currentGamemodeId { get; protected set; }	// Current slot for score opetations.
	float lastStamp;

	void Awake(){
		instance = this;

		if( !ES2.Exists("save.artik") ){
			if( oldSaveGameExists() ){
				try {
					portOldSavegame();
				} catch {
					newSavegame();
				}
			} else {
				newSavegame();
			}
		}

		lastStamp = Time.time;
	}

	void Start(){
		// Avoid cuelgue, get savegame cached
		setCoins(getCoins());

		if( ArtikFlowArcade.instance.configuration.storeTarget == ArtikFlowArcadeConfiguration.StoreTarget.FRENCH_PREMIUM && getSessionCount() == 0 ){
			setNoAds(true);
			if( !ArtikFlowArcade.instance.configuration.FRENCH_PREMIUM_tryNBuy ){
				setCoins(500);
			}
		}
		eventCoinsUpdate.Invoke(getCoins());
		AFBase.GameServices.instance.eventAuthFinish.AddListener(onGameServicesAuth);
	}

	void onGameServicesAuth(bool success){
		if(success){
			cloudLoad();
		}
	}

	// ------- API -------

	public void cloudSave(){
		if( ArtikFlowArcade.instance.configuration.storeTarget == ArtikFlowArcadeConfiguration.StoreTarget.FRENCH_PREMIUM ){
			return;
		}

#if UNITY_ANDROID
		PlayerPrefs.SetFloat("SecondsPlayed", PlayerPrefs.GetFloat("SecondsPlayed", 0f) + (Time.time - lastStamp));
		lastStamp = Time.time;

		byte[] data = GetSaveBinary();
		AndroidCloudSave.SaveGame(data, new System.TimeSpan(((long)PlayerPrefs.GetFloat("SecondsPlayed")) * 1000 * 10000));
#endif
	}

	public void cloudLoad(){
		if( ArtikFlowArcade.instance.configuration.storeTarget == ArtikFlowArcadeConfiguration.StoreTarget.FRENCH_PREMIUM ){
			return;
		}

#if UNITY_ANDROID
		Debug.Log("=========== Attempting cloud load...");
		AndroidCloudSave.eventOverwriteSavedGame.RemoveAllListeners();
		AndroidCloudSave.eventOverwriteSavedGame.AddListener(SetSaveBinary);
		AndroidCloudSave.OpenSavedGame();
#endif
	}

	// Getters

	public int getCoins(){
		return ES2.Load<int>(path("coins"));
	}

	public int getHighScore(){
		if( currentGamemodeId > 0 ){
			if( !ES2.Exists(path("highscore" + currentGamemodeId)) ){
				ES2.Save<int>(0, path("highscore" + currentGamemodeId));
			}	
			
			return ES2.Load<int>(path("highscore" + currentGamemodeId));
		} else {
			
			return ES2.Load<int>(path("highscore"));
		}
	}

	public int getDailyHighScore(){
		return ES2.Load<int>(path("dailyHighscore"));
	}

	public DateTime getDailyHighScoreDate(){
		return ES2.Load<DateTime>(path("dailyHighscoreDate"));
	}

	/// <summary>
	/// Returns the amount of times the app has been opened.
	/// </summary>
	public int getSessionCount(){
		return ES2.Load<int>(path("sessionCount"));
	}

	/// <summary>
	/// Returns the amount of games played.
	/// </summary>
	public int getGamesPlayed(){
		return ES2.Load<int>(path("gamesPlayed"));
	}

	// --
	public DateTime getNextDailyTime(){
		return ES2.Load<DateTime>(path("nextDaily"));
	}

	public int getDailysCollected(){
		return ES2.Load<int>(path("dailysCollected"));
	}

	public bool hasNoAds(){
		return ES2.Load<bool>(path("iap_noads"));
	}

	public bool hasDuplicate(){
		return ES2.Load<bool>(path("iap_duplicate"));
	}

	public bool hasRated(){
		return ES2.Load<bool>(path("rated"));
	}

	public int getPowerupCount(string powerup_id){
		string powerup_path = path("powerup_" + powerup_id);
        if( !ES2.Exists(powerup_path) ){
			ES2.Save<int>(0, powerup_path);
		}

		return ES2.Load<int>(powerup_path);
	}

	public bool getTryNBuyUnlocked(){
		if( !ES2.Exists(path("tryNBuyUnlocked")) ){
			return false;
		}

		return ES2.Load<bool>(path("tryNBuyUnlocked"));
	}

	public DateTime getFirstPlayDate(){
		if( !ES2.Exists(path("firstPlayDate")) ){
			ES2.Save<DateTime>(DateTime.Now, path("firstPlayDate"));
		}

		return ES2.Load<DateTime>(path("firstPlayDate"));
	}

	/// Return a custom saved data. The type T has to be a supported type:
	/// http://docs.moodkie.com/easy-save-2/supported-types/
	public T getCustomData<T>(string key, T defaultValue){
		string keyPath = path("CustomData." + key);

		if( !ES2.Exists(keyPath) ){
			ES2.Save<T>(defaultValue, keyPath);
		}

		return ES2.Load<T>(keyPath);
	}

	// Setters

	public void setCoins(int newCoins){
		int coinsGiven = newCoins - getCoins();
		if( coinsGiven > 0 && hasDuplicate() ){    // Duplicate coins
			coinsGiven *= 2;
			newCoins = getCoins() + coinsGiven;
		}

		ES2.Save<int>(newCoins, path("coins"));
		eventCoinsUpdate.Invoke(getCoins());
	}

	public void submitScore(int score){
		if( score > getHighScore() ){
			string savepath;
			if( currentGamemodeId > 0 ){
				savepath = path("highscore" + currentGamemodeId);
			} else {
				savepath = path("highscore");
			}
			ES2.Save<int>(score, savepath);
			eventHighscoreUpdate.Invoke(score);
		}

		if( getDailyHighScoreDate().ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd") ){
			if( score > getDailyHighScore() ){
				ES2.Save<int>(score, path("dailyHighscore"));
			}
		} else {
			ES2.Save<DateTime>(DateTime.Now, path("dailyHighscoreDate"));
			ES2.Save<int>(score, path("dailyHighscore"));
		}
	}

	public void setSessionCount(int sessionCount){
		ES2.Save<int>(sessionCount, path("sessionCount"));
	}

	public void setGamesPlayed(int gamesPlayed){
		ES2.Save<int>(gamesPlayed, path("gamesPlayed"));
	}

	// --
	public void setNextDailyTime(DateTime time){
		ES2.Save<DateTime>(time, path("nextDaily"));

		eventDailyGiftUpdate.Invoke();
	}

	public void increaseDailysCollected(){
		ES2.Save<int>(getDailysCollected() + 1, path("dailysCollected"));
	}

	public void setNoAds(bool noads){
		ES2.Save<bool>(noads, path("iap_noads"));
	}

	public void setDuplicate(bool duplicate){
		ES2.Save<bool>(duplicate, path("iap_duplicate"));
	}

	public void setRated(bool state){
		ES2.Save<bool>(state, path("rated"));
	}

	public void setPowerupCount(string powerup_id, int count, bool sendEvent = false){
		ES2.Save<int>(count, path("powerup_" + powerup_id));
		if(sendEvent) {
			while(count > 0) {
				GoogleAnalyticsV4.instance.LogEvent("boosters", "buy booster", powerup_id + " sold", 0);
				count--;
			}
		}
	}

	public void setGamemodeId(int newGamemodeId){
		currentGamemodeId = newGamemodeId;
		eventGameModeChanged.Invoke(currentGamemodeId);
	}

	public void setTryNBuyUnlocked(bool toggle){
		ES2.Save<bool>(toggle, path("tryNBuyUnlocked"));
	}

	/// Save a custom data. The type T has to be a supported type:
	/// http://docs.moodkie.com/easy-save-2/supported-types/
	public void setCustomData<T>(string key, T value){
		string keyPath = path("CustomData." + key);

		ES2.Save<T>(value, keyPath);
	}

	// --- Low-level access ---

	string path(string tag){
		return "save.artik?tag=" + tag;
	}

	string getSavePath(){
		return Path.Combine(Application.persistentDataPath, "save.artik");
    }

	void portOldSavegame(){
		SaveGame oldSave = LoadOldSaveGame();

		ES2.Save<string>(oldSave.chars, path("chars"));
		ES2.Save<int>(0, path("selectedChar"));
		ES2.Save<int>(oldSave.highScore, path("highscore"));
		ES2.Save<int>(oldSave.dailyHighScore, path("dailyHighscore"));
		ES2.Save<DateTime>(oldSave.dailyHighScoreDate, path("dailyHighscoreDate"));
		ES2.Save<int>(oldSave.coins, path("coins"));
		ES2.Save<int>(oldSave.timesPlayed, path("gamesPlayed"));
		ES2.Save<int>(oldSave.timesPlayed, path("sessionCount"));
		ES2.Save<bool>(oldSave.noAds, path("iap_noads"));
		ES2.Save<bool>(false, path("iap_duplicate"));
		ES2.Save<bool>(oldSave.rated, path("rated"));
		ES2.Save<int>(0, path("dailysCollected"));
		ES2.Save<DateTime>(DateTime.Now, path("nextDaily"));
		ES2.Save<bool>(false, path("tryNBuyUnlocked"));
		ES2.Save<DateTime>(DateTime.Now, path("firstPlayDate"));
	}

	void newSavegame(){
		ES2.Save<string>("0", path("chars"));
		ES2.Save<int>(0, path("selectedChar"));
		ES2.Save<int>(0, path("highscore"));
		ES2.Save<int>(0, path("dailyHighscore"));
		ES2.Save<DateTime>(DateTime.Now, path("dailyHighscoreDate"));
		ES2.Save<int>(0, path("coins"));
		ES2.Save<int>(0, path("gamesPlayed"));
		ES2.Save<int>(0, path("sessionCount"));
		ES2.Save<bool>(false, path("iap_noads"));
		ES2.Save<bool>(false, path("iap_duplicate"));
		ES2.Save<bool>(false, path("rated"));
		ES2.Save<int>(0, path("dailysCollected"));
		ES2.Save<DateTime>(DateTime.Now, path("nextDaily"));
		ES2.Save<bool>(false, path("tryNBuyUnlocked"));
		ES2.Save<DateTime>(DateTime.Now, path("firstPlayDate"));
    }

	byte[] GetSaveBinary(){
		Debug.Log("============ GetSaveBinary()");

		return ES2.LoadRaw("save.artik");
	}

	void SetSaveBinary(byte[] data){
		Debug.Log("============ SetSaveBinary(" + data.Length + ")");
		if( data.Length == 0 ){
			Debug.Log("============ New cloud save file, skipping.");
			return;
		}

		ES2.SaveRaw(data, "save.artik");
		lastStamp = Time.time;

		eventCoinsUpdate.Invoke(getCoins());
	}

	private SaveGame LoadOldSaveGame(){
		if( !oldSaveGameExists() ){
			return new SaveGame();
		}

		SaveGame loadedGame;
		BinaryFormatter formatter = new BinaryFormatter();

		using (FileStream fileStream = new FileStream(GetOldSavePath(), FileMode.Open)){
			try {
				const string strEncrypt = "Pj0J0jE2uw0ALpQUvnu0srqcbIyWWsGyQk7mWiihLtOp2F7ltTNxEHgUC1dW8QwouusIV";
				byte[] dv = { 0x02, 0xF4, 0x14, 0x78, 0xAF, 0xA8, 0xE9, 0xE1 };
				var byKey = Encoding.UTF8.GetBytes(strEncrypt.Substring(14, 8));

				using (Stream cryptoStream = new CryptoStream(fileStream, new DESCryptoServiceProvider().CreateDecryptor(byKey, dv), CryptoStreamMode.Read)){
					loadedGame = formatter.Deserialize(cryptoStream) as SaveGame;
				}
			} catch (Exception e) {
				Debug.Log("Excepcion cargando, creando nuevo saveGame: " + e);
				loadedGame = new SaveGame();
			}
		}

		return loadedGame;
	}

	private bool DeleteOldSaveGame(){
		try {
			File.Delete(GetOldSavePath());
		} catch (Exception e) {
			Debug.Log("Exception1: " + e);
			return false;
		}
		return true;
	}

	private bool oldSaveGameExists(){
		return File.Exists(GetOldSavePath());
	}

	private string GetOldSavePath(){
		return Path.Combine(Application.persistentDataPath, "Main.artiks");
	}

	public void saveStringToTag(string str, string tag){
		ES2.Save<string>(str, path(tag));
	}

	public string loadStringFromTag(string tag){
		string result = "";
		if( ES2.Exists(path(tag)) ){
			result = ES2.Load<string>(path(tag));
		}
		return result;
	}	
}

}