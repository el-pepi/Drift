using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
	public static TutorialManager instace;

	public bool spawnTutorialZone;

	public Module tutorialZone;

	public bool onTutorial;

	public Transform revivePoint;

	public GameObject popUpEnergy;

	public GameObject handsGO;

	public GameObject tutorialContainer;

	public bool initialPause;

	void Awake ()
	{
		instace = this;
		handsGO.SetActive (false);
		tutorialContainer.SetActive (false);
		popUpEnergy.SetActive (false);
		tutorialZone.gameObject.SetActive (false);
	}


	public void SetTutorial()
	{
		
		if (onTutorial) 
		{
			tutorialZone.gameObject.SetActive (true);
			EnemyManager.instance.canSpawn = false;
			ScoreManager.instance.timeStop = true;
			EnemyManager.instance.firstSpawn = false;
			BossBar.instance.timeBoss = false;
			ScoreManager.instance.camAddScore = false;
		}
		else
		{
			tutorialZone.gameObject.SetActive (false);
		}
	}

	public void CheckFirstGame()
	{
		if (!PlayerPrefs.HasKey("Tutorial"))
		{
			onTutorial = true;
			spawnTutorialZone = true;
			Debug.Log ("!tutorial");
		}
		else
		{
			onTutorial = false;
			spawnTutorialZone = false;
		}
			

	}


	void Update()
	{
		if (initialPause)
		{
			if (Input.GetMouseButtonDown (0))
			{
				Time.timeScale = 1;
				initialPause = false;
			}
		}
	}


	public void DeltaTimeToCero()
	{
		Time.timeScale = 0;
		initialPause = true;
	}

	public void OutofTutorial()
	{
		if (onTutorial) 
		{
			popUpEnergy.SetActive (true);
			Time.timeScale = 0;
			onTutorial = false;
			handsGO.SetActive (false);
			tutorialContainer.SetActive (false);
			ScoreManager.instance.UIState (true);

			PlayerPrefs.SetInt ("Tutorial",1);
		}
	}

	public void ActivateHands()
	{
		handsGO.SetActive (true);
		tutorialContainer.SetActive (true);
	}


	public void OnTutorialDead()
	{
	 	GameManager.instance.car.TutorialDead (revivePoint);

	}

	public void ContinueButton()
	{
		Time.timeScale = 1;
		//GameManager.instance.flash.SetTrigger("Flash");
		//SoundManager.PlayByName("Flash");
		EnemyManager.instance.canSpawn = true;
		ScoreManager.instance.timeStop = false;
		ScoreManager.instance.camAddScore = true;
		EnemyManager.instance.firstSpawn = true;
		BarManager.instance.Visible (true);
		BossBar.instance.timeBoss = true;
		popUpEnergy.GetComponent<Animator> ().Play ("CloseEnergyPopUp");
		//popUpEnergy.SetActive (false);
		ReviveTime.instance.StartCounter ();
	}
}
