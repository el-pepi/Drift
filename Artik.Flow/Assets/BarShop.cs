using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarShop : MonoBehaviour
{
	UIGrid gridManager;
	List<Transform> bitList;

	void Awake()
	{
		gridManager = transform.Find("Grid").GetComponent<UIGrid> ();
		bitList = new List<Transform> ();

		bitList = gridManager.GetChildList ();
	}

	public void UpdateBar(int amount)
	{
		int counter = 0;
		foreach (var item in bitList)
		{
			counter++;
			if (counter <= amount) {
				item.gameObject.SetActive (true);
			} else
			{
				item.gameObject.SetActive (false);
			}

		}
	}

}
