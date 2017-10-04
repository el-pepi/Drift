using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveTime : MonoBehaviour
{


	UILabel text;
	UISprite[] back;

	public int waitTime;

	int time;

	public static ReviveTime instance;

	void Awake ()
	{
		instance = this;
		text = GetComponentInChildren<UILabel> ();
		back = GetComponentsInChildren<UISprite> ();
		text.enabled = false;
		foreach (var item in back)
		{
			item.enabled = false;
		}
	}
		

	IEnumerator Counter()
	{
		
		yield return new WaitForSecondsRealtime (1);//WaitForSeconds (1);
		if (time > 1) 
		{
			time--;
			text.text = time.ToString ();
			StartCoroutine (Counter ());
		}
		else
		{
			FinishCounter ();
		}

	}

	public void StartCounter()
	{
		time = waitTime;
		GetComponent<Animator> ().Play ("SetActive");

		StartCoroutine (Counter());
		text.text = time.ToString ();
		Time.timeScale = 0;
	}

	private void FinishCounter()
	{
		Time.timeScale = 1;
		GetComponent<Animator> ().Play ("CloseReviveTime");
		/*text.enabled = false;
		foreach (var item in back)
		{
			item.enabled = false;
		}*/
	}	


}
