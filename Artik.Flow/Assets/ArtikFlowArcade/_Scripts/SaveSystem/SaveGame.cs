using System;

namespace AFArcade {

[Serializable]
public class SaveGame
{
	// Unlocked characters, sepparated by *
	public string chars = "0";

	// Highest attained score
	public int highScore = 0;

	// Highest daily score
	public int dailyHighScore = 0;

	// Used to reset the daily highscore:
	public System.DateTime dailyHighScoreDate = System.DateTime.Now;

	// Amount of coins owned
	public int coins = 0;

	// Amount of games played
	public int timesPlayed = 0;

	// If no-ads has been bought
	public bool noAds = false;

	// SaveSystem version. If this number changes, the old savegame gets deleted
	public int gameVer = 1;

	// ----------------- Banners, and other game-specific functionality:

	// When the next daily reward becomes available
	public System.DateTime nextDaily = System.DateTime.Now;

	// Amount of daily rewards gotten
	public int dailysCollected = 0;

	// If the rate popup has already been shown
	public bool rated = false;

}

}