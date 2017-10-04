using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealthBarManager : MonoBehaviour
{
	public static HealthBarManager instance;
	public GameObject hpPrefab;
	public int enableTime;

	List<HealthBar> bars;

	void Awake()
	{
		instance = this;
		bars = new List<HealthBar>();
	}

	public HealthBar GetHpBar(Enemy enemy)
	{
		foreach (HealthBar item in bars) 
		{
			if(item.inUse == false) {
				item.SetTarget(enemy);
				return item;
			}
		}

		HealthBar bar = Instantiate(hpPrefab, transform).GetComponent<HealthBar>();
		bars.Add(bar);
		bar.SetTarget(enemy);
		return bar;
	}
}