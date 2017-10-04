using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BombManager : MonoBehaviour
{
	public List<BombModule> listBombs;
	public BombModule  bomb;

	public BombModule currentModule;
	void Awake()
	{
		listBombs = new List<BombModule> ();
		foreach (Transform item in transform)
		{
			listBombs.Add (item.GetComponent<BombModule>());
			item.GetComponent<BombModule> ().SetManager (this);
			item.gameObject.SetActive (false);
		}
	}


	public void Reset()
	{
		foreach (Transform item in transform)
		{

			item.GetComponent<BombModule> ().Reset ();

		}

	}



	private BombModule GetPickUp()
	{
		int random = Random.Range (0, listBombs.Count - 1);
		currentModule = listBombs [random];
		currentModule.gameObject.SetActive (true);
		listBombs.Remove (currentModule);
		return currentModule;

			BombModule tempObj = Instantiate (bomb,transform.position,Quaternion.identity,transform)as BombModule ;
			listBombs.Add (tempObj);
			tempObj.gameObject.SetActive (true);
			return tempObj;
	}


	public void SpawnBombModule(Module pModule,Transform point)
	{
		GetPickUp ().Spawn (pModule,point);
	}

}
