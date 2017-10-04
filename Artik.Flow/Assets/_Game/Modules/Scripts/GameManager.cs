using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using AFArcade;
using UnityEngine.Events;

public class GameManager : ArtikFlowArcadeGame
{
	public static GameManager instance;

	[HideInInspector]
	public UnityEvent eventReset = new UnityEvent();
	[HideInInspector]
	public UnityEvent eventPlay = new UnityEvent();

	[System.NonSerialized]
	public Car car;
	[HideInInspector]
	public bool playing;

	[HideInInspector]
	public bool moveEnemies;

	PickUpManager[] pickUpManagers;

	DriftCharacter actualCharacter;

	public Animator flash;

	public Animator shield;

	public Transform recharge;

	public bool onBoss;


	public override void Awake()
	{
		/*if(ArtikFlowArcade.instance == null) {
			SceneManager.LoadScene("ArtikFlowBase");
		}*/
		instance = this;
		car = GameObject.FindObjectOfType<Car> ();
		pickUpManagers = GameObject.FindObjectsOfType<PickUpManager> ();
		base.Awake();

	}

	// ----------------------- Abstract implementation

	/// <summary> Called after the StartScreen. The game has to start playing. </summary>
	public override void play()
	{
		ScoreManager.instance.camAddScore = true;

		onBoss = false;

		if (!TutorialManager.instace.onTutorial)
		{
			TutorialManager.instace.CheckFirstGame ();
		}

		if(BarManager.instance != null)
			BarManager.instance.Reset ();
		// playing = true; 		// set in GameIntro
		moveEnemies = true;
		LevelManager.instance.PlayLevel ();
		PowerUpManager.instace.CheckPowerUps ();

		ScoreManager.instance.UpdateAll ();
	
		if (!TutorialManager.instace.onTutorial)
		{
			ScoreManager.instance.UIState (true);
		}

		SoundManager.PlayByName("CarStart");


		if(PowerUpManager.instace.powerUpHealth == false) {
			shield.gameObject.SetActive(false);
		} else {
			if(car != null) {
				shield.gameObject.SetActive(true);
				shield.transform.position = car.transform.position;
				shield.transform.SetParent(car.transform);
				shield.transform.localRotation = Quaternion.identity;
				shield.enabled = true;
			}
		}

		if(PowerUpManager.instace.powerUpMagnet == false) {
			recharge.gameObject.SetActive(false);
		} else {
			if(car != null) {
				recharge.gameObject.SetActive(true);
				recharge.position = car.transform.position;
				recharge.SetParent(car.transform);
				recharge.localRotation = Quaternion.Euler(new Vector3(-90,0,0));
				//recharge.localRotation = Quaternion.identity;
			}
		}
		TutorialManager.instace.SetTutorial ();
		eventPlay.Invoke();
	}

	public void SetCharacter(Character character)
	{
		if(actualCharacter != character as DriftCharacter) {
			if(car != null) {
				WeaponManager.instance.ClearWeaponList();
				car.CleanUp();
				Destroy(car.gameObject);
			}

			actualCharacter = character as DriftCharacter;
			GameObject c = Instantiate(Resources.Load<GameObject>(actualCharacter.carPrefabName) ,new Vector3 (0f,-3.2f,21.8f),Quaternion.identity)as GameObject;
			car = c.GetComponent<Car>();

			Third.instance.SetTarget(c);
			Third.instance.SetOffsetAndMinSize (14f,65f,70f,false);
			Resources.UnloadUnusedAssets();
		}
	}


	/// <summary> Called during the LoadingScreen. The game has to reset its state and idle. </summary>
	public override void reset(Character character)
	{
		
		SetCharacter (character);


		if(car != null) {
			car.Reset();
			WeaponManager.instance.DeactivateWeapons();
		}
		playing = false;
		moveEnemies = false;
		foreach (var item in pickUpManagers) 
		{
			item.Reset ();
		}

		Third.instance.SetOffsetAndMinSize (14f,65f,70f,false);
		LevelManager.instance.ResetLevel ();
		EnergyCircle.instance.anim.SetBool("Hide",false);

		ScoreManager.instance.UIState (false);

		eventReset.Invoke();
		
		eventResetReady.Invoke();
	}

	/// <summary> Called during the GameplayScreen if the player revives. The game has to resume. </summary>
	public override void revive()
	{
		car.Revive ();
		ScoreManager.instance.UIState (true);
		//BarManager.instance.Reset();
		//EnergyCircle.instance.Reset ();
		BarManager.instance.Visible (true);
		BossManager.instace.SetBossRevive ();
		EnergyCircle.instance.Revive ();
		playing = true;
		moveEnemies = true;

	}

	/// <summary> Called during the CharSelectionScreen if the player changes the sound setting. true to turn the sound on, false to turn it off.</summary>
	public override void switchSound(bool state)
	{
		return;
	}

	// ----------------------- Custom functionality

	public void die()
	{

		EnergyCircle.instance.Reset();


		shield.transform.SetParent(null);
		shield.gameObject.SetActive(false);

		recharge.SetParent(null);
		recharge.gameObject.SetActive(false);
		if (!TutorialManager.instace.onTutorial)
		{
			playing = false;
			ScoreManager.instance.UIState (false);
			BarManager.instance.Visible (false);
			EnergyCircle.instance.anim.SetBool("Hide",true);
		}

		if(car != null) {
			ParticleManager.SpawnExplosion(car.transform.position, 1);
			SoundManager.DriveVolume(0f);
			car.OnDeath();
			WeaponManager.instance.DeactivateWeapons();
			SoundManager.PlayByName("PlayerExplosion");

			flash.SetTrigger("Flash");
		}
		BarManager.instance.dmgAnim.SetBool("Warning",false);



		if (!TutorialManager.instace.onTutorial)
		{
			Invoke ("FinishGame", 1.5f);
		}
		else
		{
			TutorialManager.instace.OnTutorialDead();
		}
	}

	void FinishGame()
	{
		moveEnemies = false;
		eventFinish.Invoke();
	}


}
