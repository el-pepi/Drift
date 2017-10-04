using UnityEngine;
using System.Collections;

public class SpawnDrops : BossAttack
{
	
	EntityMovement movement;

	public Drop[] drops;


	public BombManager bombsManager;

	int currentBomb;
	Animator anim;

	void Awake () 
	{
		foreach (var item in drops)
		{
			item.Init ();
		}
		//bombsManager = GameObject.FindObjectOfType<BombManager> ();
		movement = GetComponent<EntityMovement> ();
		anim = transform.GetChild(0).GetComponent<Animator>();
	}
		
	public override void InitAnimation()
	{
		anim.SetTrigger("BombSpawn");
		StartCoroutine (AttackAfterAnimation());
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "BombExit") 
		{
			onAttack = false;
		}

	}

	IEnumerator AttackAfterAnimation()
	{
		yield return new WaitForSeconds (anim.GetCurrentAnimatorClipInfo(2).Length);
		onAnimation = false;
		Attack ();
	}

	public override void Reset()
	{
		base.Reset ();
		currentBomb = 0;
	}

	public override void OnRevive()
	{
		base.Reset ();
		currentBomb = 0;
		bombsManager.Reset();
	}

	public override void StartAttack()
	{
		/*if (currentBomb >= amountOfBombsToEnergy)
		{
			currentBomb = 0;
			int randWidth = Random.Range (5,15);
			drops [0].manager.SpawnAtPosition (movement.transform,randWidth);
		} else
		{
			currentBomb++;
			bombsManager.SpawnBombModule(movement.currentModule,movement.GetCurrentWaypointTrans());	
		}*/
		bombsManager.SpawnBombModule(movement.currentModule,movement.GetCurrentWaypointTrans());

	}
}