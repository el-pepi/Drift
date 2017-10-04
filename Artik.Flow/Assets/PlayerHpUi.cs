using UnityEngine; 
using System.Collections;
using System.Collections.Generic;
public class PlayerHpUi : MonoBehaviour
{
	public UISprite[] totalHps;
	List<UISprite> currentHps;
	public Transform heartsTrans;

	public int sAmount;
	public UISprite shield;

	public static PlayerHpUi instace;

	UIGrid grid;
	void Awake()
	{
		currentHps = new List<UISprite> ();
		heartsTrans = transform.Find ("Hearts").transform;
		totalHps = new  UISprite[heartsTrans.childCount];
		grid = heartsTrans.GetComponent<UIGrid> ();

		StateShield (false);

		for (int i = 0; i < totalHps.Length; i++)
		{
			totalHps [i] = heartsTrans.GetChild (i).GetComponent< UISprite>();
			totalHps [i].gameObject.SetActive (false);
		}

	}
	void Start()
	{
		instace = this;

	}
	public void UpdateAmountOfHearths(int amount)
	{	
		currentHps.Clear ();

		sAmount = PowerUpManager.instace.amountHP;

		for (int i = 0; i < totalHps.Length; i++)
		{
			totalHps [i].gameObject.SetActive (false);
		}

		for (int i = 0; i < amount; i++) 
		{
			totalHps [i].gameObject.SetActive (true);
			totalHps [i].spriteName = "LifeHeartON";
			currentHps.Add (totalHps[i]);
		}

		grid.repositionNow = true;
		
	}

	public void UpdateSprite()
	{
		if (PowerUpManager.instace.powerUpHealth) 
		{
			sAmount = PowerUpManager.instace.amountHP;

			if (sAmount > 1)
			{

				sAmount--;

			} else 
			{
				StateShield (false);
				PowerUpManager.instace.powerUpHealth = false;
				GameManager.instance.shield.SetTrigger("Finish");
			}
			return;
		}



		int lastIndex = currentHps.Count - 1;
		if (lastIndex >= 0) 
		{
			currentHps [lastIndex].spriteName = "LifeHeartOFF";
			currentHps.RemoveAt (lastIndex);
		}

	}



	public void StateShield(bool state)
	{
		
		if (state) 
			{
				shield.color = Color.white;
			} 
			else
			{
				shield.color = Color.grey;
			}
		

	}

}
