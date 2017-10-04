using UnityEngine; 
using System.Collections;
using System.Collections.Generic;
public class WeaponProjectile : Weapon {

	public GameObject bullet;
	public float bulletRate;
	//public float bulletSpeed;
	public int maxBullets;
	public float shootRate;
	public float damage;

	public string soundName;

	private List<GameObject> bullets;
	float time;
	int amountOfBullets;
	bool canShoot;
	//public bool shootEnable;

	//public Transform weaponPos;
	Car car;
	Animator anim;

	void Start ()
	{
		bullets = new List<GameObject> ();
		/*foreach (Transform item in transform) 
		{
			bullets.Add (item.gameObject);
			bullet.GetComponent<Bullet> ().damage = damage;
			item.gameObject.SetActive (false);
		}*/
		anim = GetComponentInChildren<Animator>();
		StartCoroutine (TimeToShoot());
		car = GameObject.FindObjectOfType<Car> ();
	}
	public override void Activate() {
		base.Activate();
//		shootEnable = true;
	}
	public override void Deactivate() {
		base.Deactivate();
//		shootEnable = false;
	}
	/*public void SetActive(bool isActive)
	{
		shootEnable = isActive;
		//StartCoroutine (DeActivate());
	}*/

	void Update () 
	{
		if (GameManager.instance.playing)
		{
			Shoot ();
		}
	}

	private void Shoot()
	{
		if (canShoot&&isActivated)
		{
			if (amountOfBullets < maxBullets) 
			{
				SpawnBullet ();
			} else {
				StartCoroutine (TimeToShoot ());
			}
		}
	}

	private void SpawnBullet()
	{
		if (time > bulletRate) 
		{
			amountOfBullets++;
			time -= bulletRate;
			GetBullet ();


			anim.SetTrigger("Muzzle");

			if(string.IsNullOrEmpty(soundName) == false) {
				SoundManager.PlayByNamePitched(soundName ,Random.Range(0.8f,1.3f));
			}

		} else 
		{
			time += Time.deltaTime;
		}
	}

	IEnumerator TimeToShoot()
	{
		canShoot = false;
		yield return new WaitForSeconds (shootRate);
		amountOfBullets = 0;
		canShoot = true;
	}

	private void GetBullet()
	{
		foreach (var item in bullets) 
		{
			if (!item.gameObject.activeInHierarchy) 
			{	
				item.SetActive (true);
				SetBullet (item);
				return;
			}
		}

		GameObject tempObj = Instantiate (bullet)as GameObject;
		tempObj.GetComponent<Projectile> ().damage = damage;
		bullets.Add (tempObj);
		SetBullet (tempObj);
	}

	private void SetBullet(GameObject bul)
	{
		bul.transform.position = transform.position;//transform.position;
		bul.transform.rotation = transform.rotation;
		bul.GetComponent<Projectile>().Shoot();
		//bul.GetComponent<Rigidbody>().velocity = (transform.forward * bulletSpeed) + car.GetComponent<Rigidbody>().velocity;
	}

	public override void Clean() {
		while(bullets.Count > 0) {
			Destroy(bullets[0].gameObject);
			bullets.RemoveAt(0);
		}
	}
}
