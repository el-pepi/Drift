using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;
public class LevelManager : MonoBehaviour
{
	public static LevelManager instance;

	ModulePoolAManager pool;

	Module StartModule;

	public List<GameObject> modulesManager;

	Connector newConnector;
	public int currentModules = 0;
	EnemyManager enemyManager;

	public int lastIndex;

	public bool spawnEnemiesAndPickups;

	BossManager bossManager;

	public float timeToLastModule;

	PickUpManager energyManager;

	public int startPool;


	void Awake()
	{
		StartModule = GameObject.Find ("StartModule").GetComponent<Module>();
		energyManager = GameObject.Find ("EnergyManager").GetComponent<PickUpManager>();
		enemyManager = GameObject.FindObjectOfType<EnemyManager> ();
		pool = GetComponent<ModulePoolAManager> ();
		modulesManager = new List<GameObject> ();
		instance = this;
		bossManager =  GameObject.FindObjectOfType<BossManager> ();
	}
		
		
	public void ResetLevel()
	{
		bossManager.Reset ();

		modulesManager.Clear ();
		enemyManager.Reset ();
		modulesManager.Add (StartModule.gameObject);
		StartModule.gameObject.SetActive (true);
		StartModule.index = 0;
		newConnector = StartModule.exit;
		lastIndex = 0;

		currentModules = 0;


		spawnEnemiesAndPickups = false;



		pool.Reset ();


	}

	public void PlayLevel()
	{
		//pool.ResetModules (startPool);

		for (int i = 0; i < 3; i++) 
		{
			SpawnModule ();

		}
	}


	public void SpawnModule()
	{

		if (modulesManager.Count >= 6) 
		{
			modulesManager[0].gameObject.SetActive(false);
			modulesManager.RemoveAt (0);
		}
	
		if (GameManager.instance.playing) 
		{
			pool.PoolControler ();

		}

		var newModule = FindObjectWithTag (pool.currentModuleTier.pools,newConnector.tag);
		//var newTag = GetRandom (newConnector.tag);
		//var newModule = FindObjectWithTag (modules.ToArray(),newTag);
		newModule.gameObject.SetActive(true);
		var exitToMatch = newModule.getExit ();
		MatchConnectors (newConnector,exitToMatch);
		// newModule.gameObject.isStatic = true;
		newConnector = newModule.exit;
		modulesManager.Add (newModule.gameObject);
		newModule.index = lastIndex;
		lastIndex++;

		if (spawnEnemiesAndPickups)
		{
			if (!newConnector.HasTag (Tags.Boss))
			{
				spawnEnemiesAndPickups = false;
				enemyManager.SpawnAtLastModule ();
				energyManager.SpawnAtLastModule ();
				energyManager.timeEnabled = true;
			}
		}

		timeToLastModule = GetDistanceToLastModule ();
	}

	public void UpdateDistance()
	{
		timeToLastModule = GetDistanceToLastModule ();
	}

	public Module GetLastModule(int index)
	{
		return modulesManager [modulesManager.Count - index].GetComponent<Module> ();
	}


	private  TItem GetRandom<TItem>(TItem[] array)
	{
		return array [Random.Range (0, array.Length)];
	}

	public float GetDistanceToLastModule()
	{
		Car player = GameManager.instance.car;

		Module firstModule = player.GetCurrentModule ();
		if (firstModule == null)
			return 0;
		
		int indexFM = modulesManager.IndexOf (firstModule.gameObject);

		float timeToLastModule = 0;

		for (int i = indexFM; i < modulesManager.Count; i++)
		{
			Module thisModule = modulesManager [i].GetComponent<Module> ();

			timeToLastModule += thisModule.moduleTime;
		}

		return timeToLastModule;

	}




	private  Module FindObjectWithTag(IEnumerable<ModulePool> modules,Tags[] tagToMatch)
	{
		if (TutorialManager.instace.spawnTutorialZone)
		{
			
			TutorialManager.instace.spawnTutorialZone = false;
			return TutorialManager.instace.tutorialZone;
		}

		List<ModulePool> tempModules = new List<ModulePool>();
		foreach (var item in modules) 
		{
			for (int i = 0; i < item.tags.Length; i++)
			{
				foreach (var tag in tagToMatch)
				{
					if (item.tags [i] == tag) {

						tempModules.Add (item);
					}
				}
			}
		}
		return ModulePoolAManager.GetModule (tempModules.ToArray());

	}
		
	private void MatchConnectors(Connector oldConnector,Connector newConnector)
	{
		var newModule = newConnector.transform.parent;
		var forwardVectorToMatch = -oldConnector.transform.forward;
		var corrRotation = Azimuth (forwardVectorToMatch) - Azimuth(newConnector.transform.forward);
		newModule.RotateAround (newConnector.transform.position,Vector3.up,corrRotation);
		var correctiveTranslation = oldConnector.transform.position - newConnector.transform.position;
		newModule.transform.position += correctiveTranslation;
		newModule.transform.position = new Vector3 (newModule.transform.position.x,-5f,newModule.transform.position.z);
	}

	private  float Azimuth(Vector3 vector)
	{
		return Vector3.Angle (Vector3.forward,vector)*Mathf.Sign(vector.x);
	}




}
