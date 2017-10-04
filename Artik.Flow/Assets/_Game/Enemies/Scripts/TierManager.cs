using UnityEngine;
using System.Collections;
using System.Collections.Generic;





public class TierManager : MonoBehaviour 
{
	public Tier[] tiersPool;

	public Tier currentTier;


	public int currentPoolIndex = 0;


	void Awake()
	{

		tiersPool = GetComponentsInChildren<Tier> ();


		currentTier = tiersPool [0];
	}

	void Update()
	{
		if(GameManager.instance.playing)
			PoolControler ();
	}



	private void PoolControler()
	{
		int time = Mathf.RoundToInt (ScoreManager.instance.time);
		int scoreToChange = currentTier.timeToChangeEnemy;
		if (time>= scoreToChange)
		{
			
			if (currentPoolIndex < tiersPool.Length - 1)
			{
				currentPoolIndex++;
			}

			currentTier = tiersPool [currentPoolIndex];
		}
	}

	public void CheckTier()
	{
		int time = Mathf.RoundToInt (ScoreManager.instance.time);
		for (int i = 0; i < tiersPool.Length; i++) 
		{
			if (time >= tiersPool [i].timeToChangeEnemy) 
			{
			currentPoolIndex = i + 1;
			currentTier = tiersPool [currentPoolIndex];
			}
		}

	}


	public float CurrentDistance ()
	{
		return Random.Range (currentTier.minDistance, currentTier.maxDistance);
	}

	public float ReturnSpawnRate()
	{
		return currentTier.spawnRate;
	}


	public void Reset()
	{
		currentPoolIndex = 0;
		currentTier = tiersPool [0];
		foreach (var item in tiersPool) {
			item.Reset ();
		}
	}




	public  Enemy GetEnemyGroup()
	{
		return currentTier.ChooseEnemy ();
	}

}
