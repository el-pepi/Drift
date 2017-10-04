using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class ModulePoolAManager : MonoBehaviour
{


	public bool onBoss;

	public static ModulePoolAManager instance;

	public ModuleTier[] moduleTiers;

	public ModuleTier currentModuleTier;

	public int currentIdexModule;

	public ModuleTier endlessPivot;

	void Awake()
	{


		instance = this;



		moduleTiers = GetComponentsInChildren<ModuleTier> ();
		int index = 0;
		foreach (var item in moduleTiers)
		{
			item.index = index;
			index++;
			item.Reset ();
		}

		currentModuleTier = moduleTiers [0];
	}
		

	private void ResetTier()
	{

		foreach (var item in moduleTiers) 
		{
			item.Reset ();
		}

	}


	public void PoolControler()
	{
		if (!ScoreManager.instance.timeStop) {
			int time = Mathf.RoundToInt (ScoreManager.instance.time);
			int scoreToChange = moduleTiers [currentIdexModule].timeToChange;
			if ((time + LevelManager.instance.timeToLastModule) >= scoreToChange) {


				/*if (currentTier < tiers.Length-1) 
				{
					currentTier++;
				} else {
					currentTier = 7;
					ScoreManager.instance.time = tiers [currentTier].timeToChange;
				}
				*/

				currentModuleTier = moduleTiers [currentIdexModule];

				if (moduleTiers [currentIdexModule].NextBoss) 
				{
					ScoreManager.instance.timeStop = true;
					Debug.Log ("Module Stop");
				}

				if (currentIdexModule < moduleTiers.Length - 1) {
					currentIdexModule++;
				} else 
				{

					SetEndless ();
				}
				CheckCurrentTier ();
			}
		}
	}


	public void SetEndless()
	{
		Debug.Log ("PoolEndless");
		currentIdexModule  = endlessPivot.index;
		currentModuleTier = endlessPivot;

		ScoreManager.instance.time = moduleTiers[currentIdexModule-1].timeToChange;
	//	ScoreManager.instance.timeStop = false;

	}

	public void SetTierAndTime(int tier)
	{
		if (currentIdexModule+2 < moduleTiers.Length - 1)
		{
			ScoreManager.instance.SetTime(moduleTiers [tier].timeToChange+1);
			currentIdexModule = tier+2;
			currentModuleTier = moduleTiers [currentIdexModule];
		}


	}

	public void SetBoss(int ind)
	{
		
		currentIdexModule = ind;
		CheckCurrentTier ();
	}


	private void CheckCurrentTier()
	{

		currentModuleTier = moduleTiers[currentIdexModule];
	}

	void Update()
	{

		if (GameManager.instance.playing) 
		{
			PoolControler ();
		}
	}

	public void Reset()
	{

		foreach (var item in moduleTiers)
		{
			item.Reset ();
		}
	}

	public void ResetAndSetModules(int modulePool)
	{
		ResetTier ();
		currentIdexModule  = modulePool;
		currentModuleTier = moduleTiers[modulePool];

	}




	public static Module GetModule(ModulePool[] array)
	{
		int rand = Random.Range (0, array.Length);
		ModulePool newModule = array [rand];


		return newModule.GetItemFromPool();

	}

}
