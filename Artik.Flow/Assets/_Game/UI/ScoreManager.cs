using UnityEngine;
using System.Collections;
using AFArcade;

public class ScoreManager : MonoBehaviour
{

	public static ScoreManager instance;

	public float time;

	[HideInInspector]
	public UILabel textTime;

	[HideInInspector]
	public UILabel score;

	[HideInInspector]
	public UILabel gemText;
	 
	[HideInInspector]
	public UILabel hiScore;

	[HideInInspector]
	public UILabel healthPlayer;

	public bool debugBar;

	public bool timeStop;

	public PlayerHpUi hpUI;

	int tempHiScore;

	bool firstPlay = true;

	public bool camAddScore;
	void Start()
	{
		instance = this;

		hpUI = GameObject.FindObjectOfType<PlayerHpUi> ();

		textTime = transform.Find("DebugText").GetComponent<UILabel>();

		score = transform.Find("Score").GetComponent<UILabel>();

		gemText = transform.Find("Gem Text").GetComponent<UILabel>();
	
		hiScore = transform.Find("Hi-Score Text").GetComponent<UILabel>();

		healthPlayer = transform.Find ("PlayerHealth").GetComponent<UILabel>();


		if (!debugBar)
			textTime.gameObject.SetActive (false);

		// UIState (false);

		tempHiScore = SaveGameSystem.instance.getHighScore ();
		hiScore.text = AddCeros ( SaveGameSystem.instance.getHighScore ().ToString(),"[5A4E00FF]");
		gemText.text = AddCeros (SaveGameSystem.instance.getCoins().ToString(),"[316A80FF]");
	}

	public void AddScore(int amount)
	{
		GameManager.instance.eventAddScore.Invoke(amount);
		UpdateScore ();
	}



	public void SetTime(int newTime)
	{
		time = newTime;
		textTime.text = Mathf.RoundToInt (time).ToString ();
	}

	void Update()
	{
		if (GameManager.instance.playing)
		{
			if (firstPlay&&camAddScore)
			{
				firstPlay = false;
				StartCoroutine (Timer());
			}
			if (!timeStop) 
			{
				time += Time.deltaTime;
				textTime.text = Mathf.RoundToInt (time).ToString ();
			}
		
		} 

	}
		
	private string AddCeros(string numbers,string color)
	{
		string finalNumber = color;
		string cero = "0";
		for (int i = 0; i < 5-numbers.Length; i++) 
		{
			finalNumber += cero;
		}
		finalNumber += "[-]";
		return finalNumber += numbers ;

	} 

	public void UpdateAll()
	{
		UpdateScore ();
		UpdateHiScore ();
		UpdateGems ();
	}

	void UpdateScore()
	{
		score.text = AddCeros (ArtikFlowArcade.instance.getScore ().ToString(),"[810059FF]");
		UpdateHiScore ();
	}

	void UpdateHiScore()
	{
		if (SaveGameSystem.instance.getHighScore()> tempHiScore) {
			tempHiScore = ArtikFlowArcade.instance.getScore ();
			hiScore.text = AddCeros (ArtikFlowArcade.instance.getScore ().ToString (),"[5A4E59FF]");
		}
	}

	public void UpdateGems()
	{
		gemText.text = AddCeros (SaveGameSystem.instance.getCoins().ToString(),"[316AACFF]");

	}

	public void UIState(bool isActive)
	{
		for (int i = 0; i < transform.childCount; i++) 
		{
			transform.GetChild (i).gameObject.SetActive(isActive);
		}
	}

	public void UpdateHealth(int newHealth)
	{
		healthPlayer.text = newHealth.ToString();
	}

	IEnumerator Timer()
	{
		yield return new WaitForSeconds (1);
		if (GameManager.instance.playing&&camAddScore) {
			GameManager.instance.eventAddScore.Invoke (1);
			UpdateScore ();
			firstPlay = false;
			StartCoroutine (Timer ());
		} else
		{
			firstPlay = true;
		}
	}

}
