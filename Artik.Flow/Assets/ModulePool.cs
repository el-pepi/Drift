using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModulePool : MonoBehaviour 
{
	public Module prefab;

	public List<Module> pool;

	public Tags[] tags;



	void Awake()
	{
		for (int i = 0; i < 2; i++)
		{
			Instantiate(prefab,transform);
		}

		foreach (Transform item in transform) 
		{
			pool.Add (item.GetComponent<Module>());
			//item.gameObject.SetActive (false);
		}

		tags = prefab.tag;
	}

	public Module GetItemFromPool()
	{
		
		foreach (Module item in pool) 
		{
			if (!item.gameObject.activeInHierarchy) 
			{	
				item.gameObject.SetActive (true);
				return item;
			}
		}

		Module tempmodule = (Module)Instantiate(prefab,transform)as Module;

		pool.Add (tempmodule);


		return tempmodule;

	}

	public void Reset()
	{
		foreach (Module item in pool)
		{
			item.gameObject.SetActive (false);
		}

	}


}
