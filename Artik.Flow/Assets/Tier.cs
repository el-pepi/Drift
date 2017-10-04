using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyP
{
	public EnemyPool pool;
	public ChanceToDropLane[] laneChance;
	[Space (20)]
	[Range(0,1)]
	public float minToDrop;

	[Range(0,1)]
	public float maxToDrop;

}

public class Tier : MonoBehaviour 
{
	public EnemyP[] enemyPools;
	[HideInInspector]
	public EnemyPool enemPool;
	[Space (10)]
	public float minDistance;
	public float maxDistance;
	public int timeToChangeEnemy;
	public float spawnRate;


	public Enemy ChooseEnemy()
	{
		UpdatePool ();

		return enemPool.GetItemFromPool ();

	}

	void UpdatePool()
	{
		float chance = Random.value;

		foreach (var item in enemyPools)
		{
			if (chance >= item.minToDrop&& chance <= item.maxToDrop)
			{
				item.pool.lanesDrop = item.laneChance;
				enemPool = item.pool;
				return;
			}
		}

	}

	public void Reset()
	{
		foreach (var item in enemyPools)
		{
			item.pool.Reset ();
		}
	}

}
