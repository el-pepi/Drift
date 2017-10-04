using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossAttack : MonoBehaviour
{
	
	public abstract void StartAttack ();

	public bool mainAttack;
	public bool secondaryAttack;
	public float attackLenght;

	public float attackTime = 5f;
	float time;
	public bool onAttack;
	[HideInInspector]
	public EntityMovement bossMovement;
	public bool onAnimation;


	void TimeToAttack()
	{
		if (time > attackTime) 
		{
			
			Attack ();

		} else 
		{
			time += Time.deltaTime;
		}
	}
	public virtual void InitAnimation (){}


	public void Attack()
	{
		if (!onAttack) 
		{
			time = 0;
			onAttack = true;
			StartAttack ();
		}
	}

	public virtual void Reset()
	{
		time = 0;
		onAttack = false;
		onAnimation = false;
	}

	public virtual void OnRevive()
	{
	}

	public virtual void Update()
	{
		if (GameManager.instance.playing)
		{
			if(!onAttack)
				TimeToAttack ();	
		}
	}



}
