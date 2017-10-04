using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleTier : MonoBehaviour
{
	public ModulePool[] pools;
	public int timeToChange;
	public bool NextBoss;
	[HideInInspector]
	public int index;
	public float moduleTime;


	public Module ChooseModule()
	{
		
		ShuffleArray<ModulePool> (pools);
		return pools[0].GetItemFromPool ();

	}
		
	void ShuffleArray<T>(T[] arr)
	{
		for (int i = arr.Length-1; i > 0; i--)
		{
			int r = Random.Range (0,i);
			T temp = arr [i];
			arr [i] = arr [r];
			arr [r] = temp;
		}
	}

	public void Reset()
	{
		foreach (var item in pools)
		{
			item.Reset ();
		}
	}
}
