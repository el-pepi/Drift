using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChanceToDropLane
{
	[Space (20)]
	public Lanes lane;

	[Range(0,1)]
	public float min;
	[Range(0,1)]
	public float max;

}


public class EnemyPool : MonoBehaviour 
{
	public Enemy prefab;
	public int poolStartSize;

	[HideInInspector]
	public List<Enemy> pool;

	public ChanceToDropLane[] lanesDrop;

	void Awake()
	{
		foreach (Transform item in transform) 
		{
			pool.Add (item.GetComponent<Enemy>());
			item.gameObject.SetActive (false);
		}
	}

	void Start()
	{
		// Populate
		int toAdd = poolStartSize - pool.Count;
		while(toAdd > 0)
		{
			Enemy tempEnemy = Instantiate(prefab,transform)as Enemy;
			pool.Add (tempEnemy);
			tempEnemy.gameObject.SetActive (false);

			toAdd --;
		}
	}

	public Enemy GetItemFromPool()
	{
		foreach (Enemy item in pool) 
		{
			if (!item.gameObject.activeInHierarchy) 
			{	
				item.gameObject.SetActive (true);
				PickLane (item);
				return item;
			}
		}

		Enemy tempEnemy = Instantiate(prefab,transform)as Enemy;
		pool.Add (tempEnemy);
		tempEnemy.gameObject.SetActive (true);
		Debug.Log("*****************New pool size for " + transform.name + ": " + pool.Count);

		PickLane (tempEnemy);
		return tempEnemy;

	}

	public void Reset()
	{
		foreach (Enemy item in pool)
		{
			item.gameObject.SetActive (false);
		}
	}

	void PickLane(Enemy enem)
	{
		float chance = Random.value;

		foreach (var item in lanesDrop)
		{
			if (chance <= item.max && chance >= item.min)
			{
				enem.lane = item.lane;
			}
		}
	}

}
