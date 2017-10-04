using UnityEngine;
using System.Collections;

public class TargetLaser : MonoBehaviour {


	//public GameObject laser;
	public Laser laserManager;
	GameObject minScale;
	GameObject spriteTarget;
	Vector3 firstScale;
	public Vector3 scaleToGo;
	GameObject laserO;

	float finishTime = 0;
	LineRenderer[] lines;

	void Awake ()
	{
	//	laser.gameObject.SetActive (false);
		spriteTarget = transform.GetChild (0).gameObject;
		laserO = transform.GetChild (1).gameObject;
		laserO.SetActive(false);
		firstScale = spriteTarget.transform.localScale;
		minScale = transform.GetChild (2).gameObject;
		scaleToGo = minScale.transform.localScale;
		spriteTarget.gameObject.SetActive (false);

		lines = laserO.GetComponentsInChildren<LineRenderer>();

	}

	void Update(){
		if(finishTime > 0) {
			finishTime -= Time.deltaTime;
			lines[0].SetPosition(1,transform.InverseTransformPoint( laserManager.laserGuns[0].transform.position));
			lines[1].SetPosition(1,transform.InverseTransformPoint( laserManager.laserGuns[1].transform.position));
			if(finishTime <= 0) {
				laserO.SetActive(false);
				lines[0].SetPosition(1,Vector3.zero);
				lines[1].SetPosition(1,Vector3.zero);
				lines[0].SetPosition(0,Vector3.zero);
				lines[1].SetPosition(0,Vector3.zero);
				laserManager.laserGuns[0].GetChild(0).gameObject.SetActive(false);
				laserManager.laserGuns[1].GetChild(0).gameObject.SetActive(false);
			}
		}
	}

	public void SetLaser(Laser newLaser)
	{
		laserManager = newLaser;
	}

	public void ScaleTarget(float time)
	{
		spriteTarget.gameObject.SetActive (true);
		iTween.ScaleTo (spriteTarget,iTween.Hash("scale",scaleToGo,"time",time,"oncompletetarget",gameObject,"oncomplete","SpawnLaser","easetype",iTween.EaseType.easeInQuart));
		iTween.RotateAdd (spriteTarget,iTween.Hash("amount",Vector3.forward*360f,"time",time,"easetype",iTween.EaseType.linear));
	}
		



	public void SetTarget()
	{
		laserO.gameObject.SetActive (false);
		spriteTarget.transform.localScale = firstScale;
		spriteTarget.gameObject.SetActive (false);
		laserManager.laserGuns[0].GetChild(0).gameObject.SetActive(true);
		laserManager.laserGuns[1].GetChild(0).gameObject.SetActive(true);
		SoundManager.PlayByName("BossLaserCharge");

	}

	public void SpawnLaser()
	{
		laserManager.activateLaser = false;
		laserO.gameObject.SetActive (true);
		laserManager.realSpeed = laserManager.initialSpeedTarget;
		laserManager.onAttack = false;
		finishTime = 1f;

		SoundManager.PlayByName("BossLaserShoot");
	}
}
