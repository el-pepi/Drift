using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeLasers : BossAttack
{
	
	public GameObject lasersPrefab;

	public Vector3 offset;

	[Range(0,100)]
	public int doubleRate; 

	GameObject lasersParent;
	LaserLine[] lasers;
	public Drop drop;

	public float timeToShoot;

	public float timeActive;
	Animator anim;

	void Awake ()
	{
		drop.Init ();
		lasersParent = Instantiate (lasersPrefab)as GameObject;

		lasers = lasersParent.GetComponentsInChildren<LaserLine> ();

		foreach (var item in lasers) 
		{
			item.SetLaser (this);
		}

		bossMovement = GetComponent<EntityMovement> ();
		anim = transform.GetChild(0).GetComponent<Animator>();
	}



	public override void InitAnimation()
	{
		Reset ();
		anim.SetTrigger("3Laser");
		StartCoroutine (AttackAfterAnimation());
	}

	IEnumerator AttackAfterAnimation()
	{
		yield return new WaitForSeconds (anim.GetCurrentAnimatorClipInfo(2).Length);
		Attack ();
	}

	public override void Update()
	{
		if(bossMovement.playerVisible)
		base.Update ();
		
		lasersParent.transform.position = offset + transform.position;
	}

	public override void StartAttack()
	{
		lasersParent.gameObject.SetActive (true);
		int chance = Random.Range (0, 100);
		if (chance <= doubleRate )
		{
			PickRandomLaser (2);
		} else
		{
			PickRandomLaser (1);

		}
	}

	public override	void Reset()
	{
		base.Reset ();
		foreach (var item in lasers)
		{
			item.Reset ();
		}
		StopAllCoroutines ();
		lasersParent.gameObject.SetActive(false);
	}

	public override void OnRevive()
	{
		Reset ();
	}


	void PickRandomLaser(int l)
	{
		ShuffleArray<LaserLine> (lasers);
		for (int i = 0; i <l; i++)
		{
			lasers [i].SetTarget();
			lasers [i].ScaleTarget (timeToShoot, timeActive);
		}

		drop.manager.SpawnAtPosition (transform,12,-offset);


		SoundManager.PlayByName("BossTripleLaserCharge");
	}

	void ShuffleArray<T>(T[] arr)
	{
		for (int i = arr.Length-1; i > 0; i--)
		{
			int r = Random.Range (0,i);
			T temp = arr [i];
			arr [i] = arr [r];
			arr [r] = temp;
		}
	}
}
