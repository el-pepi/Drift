using UnityEngine;
using System.Collections;
using AFArcade;

public class PlayerHealth : MonoBehaviour
{
	Rigidbody rb;
	[Range(1,6)]
	public int health;
	[HideInInspector]
	public int currentHealth;

	public bool canCollide;

	public int safeTime;

	public GameObject[] damages;

	public float power;

	public PlayerHpUi hpUi;



	void Awake()
	{
		currentHealth = health;
		rb = GetComponent<Rigidbody> ();
	}

	public void Reset()
	{
		currentHealth = health;
		hpUi = ScoreManager.instance.hpUI;
		if(hpUi != null)
		hpUi.UpdateAmountOfHearths (currentHealth);
		canCollide = true;
		//UpdateDmgFX();
		NoDmgFX();
	}

	public void AddHealth(int amount)
	{
		currentHealth += amount;
	}

	void OnCollisionEnter(Collision col)
	{

		if (col.gameObject.CompareTag ("Obstacle")||col.gameObject.CompareTag ("Enemy")) 
		{
			TakeDamage(col.gameObject);

			if (col.rigidbody != null && col.gameObject.tag == "Enemy" && col.rigidbody.isKinematic) 
			{
				col.rigidbody.isKinematic = false;
				col.rigidbody.AddForce (col.transform.up  *power,ForceMode.VelocityChange);
				col.rigidbody.AddTorque (col.contacts[0].point*100,ForceMode.VelocityChange);
				SoundManager.PlayByName("CarHit");
				ParticleManager.EmitParticleAt("CarHitSparks",col.contacts[0].point,15);
			}
			/*if(currentHealth>0)
			StartCoroutine (TimerCollision());*/
		}



	}

	void OnTriggerEnter(Collider col)
	{

		if (col.gameObject.CompareTag ("EndOfTutorial")) 
		{
			TutorialManager.instace.OutofTutorial ();
		}


		if (col.gameObject.CompareTag ("Obstacle")) 
		{
			TakeDamage(col.gameObject);
			SoundManager.PlayByName("CarHit");
		}

		if (col.gameObject.CompareTag ("Bomb"))
		{
			if(col.GetComponent<Renderer>().enabled)
			{
				TakeDamage(col.gameObject);
				SoundManager.PlayByName ("CarHit");
			}
		}

		if (col.gameObject.CompareTag ("ChaserGO")) 
		{
			rb.isKinematic = true;
			GameManager.instance.die ();
			UpdateDmgFX();
		}

		if (col.gameObject.CompareTag ("BossEntrance"))
		{
			GameManager.instance.onBoss = true;
			ScoreManager.instance.camAddScore = false;
			EnergyCircle.instance.BarToCero();
			Third.instance.SetOffsetAndMinSize (22f,85f,110f, true);
			//BarManager.instance.energyBar.BarToCero ();
			BossManager.instace.SpawnBoss ();
			BossBar.instance.checkNextBoss = true;
			BarManager.instance.Visible (false);
		}

		if (col.gameObject.CompareTag ("BossExit"))
		{
			if (!BossBar.instance.checkNextBoss)
				return;
			ColorManager.instace.SetColors ();
			Third.instance.SetOffsetAndMinSize (14f,65f,70f,true);
			GameManager.instance.flash.SetTrigger("Flash");
			SoundManager.PlayByName("Flash");
			ScoreManager.instance.timeStop = false;
			BossBar.instance.CheckTime ();
			BarManager.instance.Visible (true);
			ScoreManager.instance.camAddScore = true;
			GameManager.instance.onBoss = false;
			PowerUpManager.instace.CheckMaxLevel ();
		}
			
		if (col.gameObject.CompareTag ("Gem")) 
		{
			col.GetComponent<Rigidbody>().isKinematic = false;
			col.gameObject.GetComponent<Enemy> ().Die ();

			col.gameObject.GetComponent<GemPickUp> ().OnCollision ();

			SoundManager.PlayByName("Gem");
		}
	}

	void TakeDamage(GameObject c){
		Third.instance.Hit();


		DamagePlayer dmgP = c.GetComponent<DamagePlayer> ();
		if (dmgP == null)
			return;
		if (dmgP != null) 
		{
			if (canCollide) 
			{
				for (int i = 0; i < dmgP.damageToPlayer; i++)
				{
					hpUi.UpdateSprite ();	
				}
				currentHealth -= dmgP.damageToPlayer;
				StartCoroutine (TimerCollision());
			}
			if(dmgP.damageType == DamagePlayer.DamageType.laser) {
				ParticleManager.EmitParticleAt("LaserHitLight",transform.position + Vector3.up * 3,1,transform);
				ParticleManager.EmitParticleAt("LaserHitSparks",transform.position + Vector3.up * 3,30,transform);
			}

			if(dmgP.damageType == DamagePlayer.DamageType.bomb) 
			{
				ParticleManager.SpawnExplosion(transform.position+Vector3.up *2,3);
			}
		} else
		{
			if (canCollide)
			{
				StartCoroutine (TimerCollision());
				currentHealth--;
			}
		}
		if(currentHealth <= 1) {
			BarManager.instance.dmgAnim.SetBool("Warning",true);
		} else {
			BarManager.instance.dmgAnim.SetTrigger("Hit");
		}
		if(currentHealth <= 0) {
			rb.isKinematic = true;
			GameManager.instance.die();
		}

		UpdateDmgFX();

	}

	void UpdateDmgFX(){
		switch(currentHealth) {
			case 1:
				damages[0].SetActive(false);
				damages[1].SetActive(true);
			break;
			case 2:
				damages[0].SetActive(true);
				damages[1].SetActive(false);
			break;
			default:
				damages[0].SetActive(false);
				damages[1].SetActive(false);
			break;
		}
	}

	void NoDmgFX()
	{
		damages[0].SetActive(false);
		damages[1].SetActive(false);
	}


	public void Inmune()
	{
		StartCoroutine (TimerCollision());
	}

	IEnumerator TimerCollision()
	{
		canCollide = false;

		GetComponentInChildren<Animator> ().SetBool ("IsInmune",true);
		yield return new WaitForSeconds (safeTime);
		GetComponentInChildren<Animator> ().SetBool ("IsInmune",false);
		canCollide = true;
	}


}
