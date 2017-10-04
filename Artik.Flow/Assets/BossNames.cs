using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossNames : MonoBehaviour {

	UISprite nameSprite;

	public static BossNames instance;

	Animator anim;
	void Awake () 
	{
		instance = this;
		nameSprite = GetComponentInChildren<UISprite> ();
		anim = GetComponent<Animator> ();
	}

	public void PlayAnimation(int bossLevel)
	{
		if (bossLevel > 4)
			bossLevel = 4;
		nameSprite.spriteName = bossLevel.ToString ();
		anim.Play ("SpawnName");
	}

	public void BossStartAttack()
	{
		BossManager.instace.SetHealthBoss ();
	}

}
