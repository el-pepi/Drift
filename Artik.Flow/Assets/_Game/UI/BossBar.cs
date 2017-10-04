using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BossBar : Bar
{

	//public float[] timeToBoss;
	public List<ModuleTier> bossTiers;
	float time;
	float realTime;
	public int currentTier;
	public ModulePoolAManager modulePool;
	UILabel text;
	public bool spawnModules;
	public bool timeBoss;
	public BossManager bossManager;
	public PickUpManager energyManager;

	public bool spawnBoss;

	public int bossLevel;

//	public UILabel levelIndicator;

//	public UIButton bossSprite;

	public static BossBar instance;

	GameObject container;

	public bool checkNextBoss;


	public override void Reset()
	{
		ResetValue ();
		//if (!spawnBoss)
		PowerUpManager.instace.SetBoss ();
		spawnModules = true;
		timeBoss = true;
		currentTier = 0;
//		levelIndicator.text = bossLevel.ToString();

/*		if (spawnBoss)
		{
			bossSprite.defaultColor = Color.white;
		} else
		{
			bossSprite.defaultColor = Color.black;	
		}
*/
		if (bossLevel == -1 || bossLevel == 0 && !spawnBoss)
		{
			EnemyManager.instance.firstSpawn = true;
			EnemyManager.instance.canSpawn = true;
			ScoreManager.instance.timeStop = false;
			energyManager.timeEnabled = true;

			modulePool.ResetAndSetModules(0);
			time = 0f;
			ScoreManager.instance.SetTime (0);
			realTime = bossTiers [currentTier].timeToChange;
			return;
		}

		if (spawnBoss) 
		{
			energyManager.timeEnabled = false;
			spawnModules = false;
			EnemyManager.instance.canSpawn = false;
			ScoreManager.instance.timeStop = true;
			EnemyManager.instance.firstSpawn = false;
			bossManager.UpdateBoss (bossLevel);

			currentTier = bossLevel;

			time = bossTiers [currentTier].timeToChange;
			modulePool.ResetAndSetModules (bossTiers [currentTier].index + 1);


			realTime = bossTiers [currentTier].timeToChange;
		
			ScoreManager.instance.SetTime (bossTiers [currentTier].timeToChange);
			return;
		}
			
		EnemyManager.instance.firstSpawn = true;
		EnemyManager.instance.canSpawn = true;

		ScoreManager.instance.timeStop = false;
		energyManager.timeEnabled = true;;

		bossManager.UpdateBoss (bossLevel);

		currentTier = bossLevel;

		if(currentTier!=0)
			modulePool.ResetAndSetModules (bossTiers [currentTier-1].index+2);

		if(currentTier == 0)
			modulePool.ResetAndSetModules (0);
		
		time = 0;
		spawnModules = true;
		timeBoss = true;


		realTime = bossTiers [currentTier].timeToChange - bossTiers [currentTier - 1].timeToChange; 

		UpdateBossTime ();

	}

	public void UpdateBossTime()
	{
		ScoreManager.instance.SetTime (bossTiers [currentTier-1].timeToChange);
	}


	public override void InitAwake()
	{
		text = GetComponentInChildren<UILabel> ();
		modulePool = GameObject.FindObjectOfType<ModulePoolAManager> ();
		bossManager = GameObject.FindObjectOfType<BossManager> ();
		energyManager = GameObject.Find ("EnergyManager").GetComponent<PickUpManager>();

		container = transform.Find ("Container").gameObject;
		container.gameObject.SetActive (false);
		instance = this;
		foreach (var item in modulePool.moduleTiers)
		{
			if (item.NextBoss)
			{
				bossTiers.Add (item);
			}
		}
		realTime = bossTiers [currentTier].timeToChange;

		bossLevel = PlayerPrefs.GetInt("selectedLevel");
	}

	public void AddLevel()
	{
		if (bossLevel < bossManager.bossTiers.Length-1) 
		{
			bossLevel++;
		}
		else 
		{
			bossLevel = -1;	
		}

//		levelIndicator.text = bossLevel.ToString();
	}
	public void SpawnBossOnStart()
	{
		spawnBoss = !spawnBoss;
/*		if (spawnBoss)
		{
			bossSprite.defaultColor = Color.white;
		} else
		{
			bossSprite.defaultColor = Color.black;	
		}*/
	}
	void Update()
	{
		if (GameManager.instance.playing)
		{
			if (timeBoss) {
				time += Time.deltaTime;
				slider.value = time / realTime;
				text.text = Mathf.RoundToInt (realTime - time).ToString () + "M";
				if (time >= realTime) 
				{
					OnFullBar ();
					return;
				}
			
				if (time + LevelManager.instance.timeToLastModule >= realTime && spawnModules) 
				{
					energyManager.timeEnabled = false;
					spawnModules = false;
					EnemyManager.instance.canSpawn = false;
					Debug.Log ("SpawnTranstionModule");
				}
			}
		} 
	}

	public void Visible(bool state)
	{
		container.gameObject.SetActive (state);
	}

	public void KillBoss()
	{
		//EnemyManager.instance.firstSpawn = true;

		BossManager.instace.spawnBoss = true;

		LevelManager.instance.spawnEnemiesAndPickups = true;

		//energyManager.timeEnabled = true;
		modulePool.onBoss = false;

		if (currentTier < bossTiers.Count - 1) {
			ModulePoolAManager.instance.SetTierAndTime (bossTiers [currentTier].index);
		} 
		else
		{
			ModulePoolAManager.instance.SetEndless ();
		}


		Debug.Log ("Killboss");
	}

	public void CheckTime()
	{
		
		
		checkNextBoss = false;

		ScoreManager.instance.timeStop = false;
		if (currentTier < bossTiers.Count-1)
		{
			Debug.Log ("Add boss");
			AddLevel ();
			currentTier++;
		} else {
			
		}
		realTime = bossTiers [currentTier].timeToChange - bossTiers [currentTier - 1].timeToChange; 


		slider.value = 0;

		time = 0;
		spawnModules = true;
		timeBoss = true;
	}


	protected override void OnFullBar()
	{
		timeBoss = false;

		modulePool.onBoss = true;

	}
}
