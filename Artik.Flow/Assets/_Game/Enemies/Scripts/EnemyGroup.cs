using UnityEngine;
using System.Collections;

public class EnemyGroup : MonoBehaviour {

	public int enemiesAlive;
	int totalGroup;
	public GameObject player;

	//public int currentPoint;
	//public int maxPoints;

	public Enemy enemyCero;

	void OnEnable()
	{
		EnableEnemies();
		enemiesAlive = transform.childCount-1;
	}

	void Awake()
	{
		gameObject.SetActive (false);
		enemyCero = transform.GetChild (0).GetComponent<Enemy>();
	}

	void OnDisable()
	{
		//if(EnemyManager.instance != null)
		//EnemyManager.instance.DeleteFromList (this);
		foreach (Transform item in transform)
		{
			item.gameObject.SetActive (false);
		}
		transform.position = Vector3.zero;
	}


	private void EnableEnemies()
	{
		foreach (Transform item in transform) 
		{

		}
	}

	private void AddDisable(Enemy enemy)
	{
		enemyCero = transform.GetComponentInChildren<Enemy> ();

		enemiesAlive--;
	}

	private void UpdatePoints()
	{
		foreach (Transform item in transform) 
		{
			if (item.gameObject.activeInHierarchy) 
			{
				enemyCero = item.GetComponent<Enemy> ();
				return;
			}
		}

	}

	private bool CheckPlayer()
	{
		if (Vector3.Distance (transform.position, player.transform.position) < 5f)
			return true;
		else 
		{
			return false;
		}
			
	}
		
	void Update () 
	{
		if (!GameManager.instance.playing)
		{
			gameObject.SetActive (false);
		}

		if (enemiesAlive <= 0)
		{
			if (gameObject.activeInHierarchy) 
			{
				gameObject.SetActive (false);
			}
		}
	}

}
