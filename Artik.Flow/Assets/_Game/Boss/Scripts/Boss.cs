using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Boss : MonoBehaviour,IDamageable
{

	public float health;
	float currentHealth;
	public BossBar bossBar;
	Laser laser;
	EntityMovement entMov;

	public BossAttack[] attacks;

	public BossAttack[] mainAttacks;

	List<BossAttack> secondaryAttacks;

	public BossAttack currentMainAttack;

	public Animator anim;

	public float deadTime;
	 
	public float initTime;

	public bool firstEncounter;

	public ParticleSystem explodingParticles;

	public float spawnDistance;

	bool changeWep;

	public bool takeDamage;

	void Awake()
	{
		currentHealth = health;
		bossBar = GameObject.FindObjectOfType<BossBar> ();
		laser = GetComponent<Laser> ();
		entMov = GetComponent<EntityMovement> ();
		attacks = GetComponents<BossAttack> ();
		secondaryAttacks = new List<BossAttack>();
		SetMainAttacks ();
	}


	void Update()
	{
		if (GameManager.instance.playing)
		{
			if (entMov.playerVisible && firstEncounter)
			{
				firstEncounter = false;
			//	StartCoroutine (StartTime());

				PickAttack (true);

				if (mainAttacks.Length == 0) {
					foreach (var item in attacks) {
						item.InitAnimation ();
					}
				} else
				{
					currentMainAttack.InitAnimation ();
				}

				foreach (var item in secondaryAttacks) 
				{
					item.enabled = true;
					item.InitAnimation ();
				}

			}

			if (!firstEncounter && changeWep)
			{
				PickAttack (true);
			}

		}

	}

	public void HealthToCero()
	{
		anim.SetFloat("Health",0);
	}

	void SetMainAttacks()
	{
		int amountMain = -1;
		foreach (BossAttack item in attacks)
		{
			if (item.mainAttack) {
				amountMain++;
			}
			if(item.secondaryAttack)
			{
				secondaryAttacks.Add (item);
			}

		}
		mainAttacks = new BossAttack[amountMain+1];



		for (int i = 0; i < attacks.Length; i++) 
		{
			if (attacks [i].mainAttack && amountMain>=0)
			{
				mainAttacks [amountMain] = attacks [i];
				amountMain--;
			}
		}

	}

	void PickAttack(bool notFirst)
	{
		if (currentMainAttack != null) {
			if (currentMainAttack.onAnimation || currentMainAttack.onAttack)
				return;
		}

		ShuffleArray<BossAttack> (mainAttacks);
		changeWep = false;



		for (int i = 0; i < mainAttacks.Length-1; i++)
		{
			if (mainAttacks [i] != currentMainAttack)
			{

				if (currentMainAttack != null)
				{
					currentMainAttack.enabled = false;
					currentMainAttack.Reset ();
				}
				currentMainAttack = mainAttacks [i];
				Debug.Log ("main attack enable");
				currentMainAttack.enabled = true;
				if(notFirst)
				currentMainAttack.InitAnimation ();
				
				StartCoroutine (ChangeMainAttack());
				return;
			}
		}

	}

	IEnumerator ChangeMainAttack()
	{
		yield return new WaitForSeconds (currentMainAttack.attackLenght);
		changeWep = true;
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

	public void Reset()
	{
		currentHealth = health;
		//anim.SetFloat("Health",0);
		firstEncounter = false;
		StopAllCoroutines ();
		//anim.SetFloat("Health",currentHealth/health);
		takeDamage = false;
		if (entMov != null)
			entMov.Reset ();

		foreach (var item in attacks)
		{
			item.Reset ();
			item.enabled = false;
		}
		gameObject.SetActive (false);
	}

	public void SetHealth()
	{
		BossRoarEffect.instance.SetBossHead(transform.GetChild(0).GetChild(0).Find("Head"));
		anim.SetTrigger("Intro");
		SoundManager.PlayByName("BossIntroRoar");
		Third.instance.BossShake();
		anim.SetFloat("Health",0);
		Hashtable ht = new Hashtable();

		ht.Add ("from",0);
		ht.Add ("to",1);
		ht.Add ("time",1f);
		ht.Add ("onupdate","ChangeValue");
		ht.Add ("oncomplete","SetBoss");
		ht.Add ("easytype",iTween.EaseType.linear);
		iTween.ValueTo (this.gameObject,ht);
	}

	public void ChangeValue(float value)
	{
		anim.SetFloat("Health",value);
	}

	public void SetBoss()
	{
		takeDamage = true;

		FirstAttack ();

		foreach (var item in secondaryAttacks) 
		{
			item.enabled = true;
			item.InitAnimation ();
		}
	}

	void FirstAttack()
	{
		firstEncounter = false;
	//	StartCoroutine (StartTime());
		PickAttack (false);

		if (mainAttacks.Length != 0) {

			currentMainAttack.InitAnimation ();
		}
	}



	public virtual void TakeDamage(float damage)
	{
		if(currentHealth <= 0||!takeDamage) {
			return;
		}
		currentHealth -= damage;
		anim.SetFloat ("Health", currentHealth / health);
		anim.SetTrigger("Hurt");
		if (currentHealth <= 0)
		{
			StartCoroutine (TimeToDie());
		}
	}

	void Die()
	{

		GameManager.instance.flash.SetTrigger("Flash");
		ParticleManager.SpawnExplosion(transform.position+Vector3.up*10,2);
		//ParticleManager.SpawnExplosion(transform.position+Vector3.up *2,0);
		bossBar.KillBoss ();
		Reset ();
		gameObject.SetActive (false);
	}

	public void OnRevive()
	{
		firstEncounter = true;
		StopAllCoroutines ();

		if (entMov != null)
			entMov.Reset ();

		foreach (var item in attacks)
		{
			item.enabled = false;	
			item.OnRevive ();
		}
	}


	IEnumerator TimeToDie()
	{
		AttackOnAnimation (true);
		anim.SetBool("Dead",true);//.Play ("Boss1Die");
		explodingParticles.Play();
		SoundManager.PlayByName("BossDie");

		yield return new WaitForSeconds (deadTime);

		Die ();
	}

	IEnumerator StartTime()
	{
		AttackOnAnimation (true);

		yield return new WaitForSeconds (initTime);

		AttackOnAnimation (false);
	}

	void AttackOnAnimation(bool state)
	{
		foreach (var item in attacks) 
		{
			item.onAnimation = state;
		}
	}
}
