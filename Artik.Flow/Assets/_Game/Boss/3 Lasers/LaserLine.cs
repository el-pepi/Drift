using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserLine : MonoBehaviour
{
	public GameObject laser;
	public ThreeLasers laserManager;
//	GameObject minScale;
	GameObject spriteTarget;
	Vector3 firstScale;
	public Vector3 scaleToGo;
	//GameObject laserO;
	float timeActive;

	Animator anim;


	void Awake ()
	{
		laser.gameObject.SetActive (false);
		spriteTarget = transform.GetChild (0).gameObject;
		//laserO = transform.GetChild (1).gameObject;
		firstScale = spriteTarget.transform.localScale;
//		minScale = transform.GetChild (2).gameObject;
//		scaleToGo = minScale.transform.localScale;
		spriteTarget.gameObject.SetActive (false);

		anim = GetComponent<Animator>();
	}

	public void SetLaser(ThreeLasers newLaser)
	{
		laserManager = newLaser;
	}

	public void ScaleTarget(float tShoot,float tActive)
	{
		spriteTarget.gameObject.SetActive (true);
		timeActive = tActive;
		iTween.ScaleTo (spriteTarget,iTween.Hash("scale",scaleToGo,"time",tShoot,"oncompletetarget",gameObject,"oncomplete","SpawnLaser","easetype",iTween.EaseType.linear));
	}

	public void SetTarget()
	{
		//laserO.gameObject.SetActive (false);
		iTween.Stop (spriteTarget);
		spriteTarget.transform.localScale = firstScale;
		spriteTarget.gameObject.SetActive (false);
	}

	public void SpawnLaser()
	{
		//	laserO.gameObject.SetActive (true);
		spriteTarget.transform.localScale = firstScale;
		anim.SetTrigger("Start");
		StartCoroutine (DisableLaser());
		SoundManager.PlayByName("BossTripleLaserShoot");
	}

	public void Reset()
	{
		StopCoroutine (DisableLaser());
		iTween.Stop (spriteTarget);
		SetTarget ();
	}


	IEnumerator DisableLaser()
	{
		yield return new WaitForSeconds (timeActive);
		laserManager.onAttack = false;
	//	laserO.gameObject.SetActive (false);
		anim.SetTrigger("Finish");
	}
}
