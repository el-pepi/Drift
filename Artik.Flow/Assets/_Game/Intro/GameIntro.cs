using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameIntro : MonoBehaviour 
{
	private Vector3 BACKGROUND_POS = new Vector3(-429f, -130f, -944f);
	private float SPEED = 200f;

	public GameObject[] objectsToToggle;
	public Animator tunnelAnim;

	Camera mainCamera;
	Animator cameraAnim;

	enum IntroState {
		NONE,
		WAITING,
		BEGGINING,
		FINISHED,
	}
	IntroState state = IntroState.NONE;

	void Start ()
	{
		mainCamera = Third.instance.transform.GetChild(0).GetComponent<Camera>();
		cameraAnim = Third.instance.GetComponent<Animator>();

		GameManager.instance.eventReset.AddListener(onReset);
		GameManager.instance.eventPlay.AddListener(() => { StartCoroutine(onPlay()); });
	}

	// ---

	void setState(IntroState newState)
	{
		if(newState == IntroState.WAITING)
		{
			foreach(GameObject g in objectsToToggle)
				g.SetActive(false);

			Third.instance.SetTarget(null);
			// Real pos: 
			// -24.27 51.74 -17.47
			// 45 31 0
			Third.instance.transform.position = new Vector3(-30f, 62.864f, -27f);
			Third.instance.transform.eulerAngles = new Vector3(39f, 31f, 0f);
			tunnelAnim.SetTrigger("Idle");
			if(state != newState)
			{
				cameraAnim.enabled = true;
				cameraAnim.SetTrigger("IntroIn");
			}
		}

		else if(newState == IntroState.BEGGINING)
		{
			cameraAnim.SetTrigger("IntroOut");
			tunnelAnim.SetTrigger("Start");
		}

		else if(newState == IntroState.FINISHED)
		{
			GameManager.instance.flash.SetTrigger("Flash");
			SoundManager.PlayByName("Flash");
			GameManager.instance.playing = true;
			cameraAnim.enabled = false;

			mainCamera.transform.localPosition = Vector3.zero;
			mainCamera.transform.eulerAngles = Vector3.zero;
			Third.instance.transform.position = Third.instance.start_pos;
			Third.instance.transform.rotation = Third.instance.start_rot;
			Third.instance.SetTarget(GameManager.instance.car.gameObject);
			
			mainCamera.transform.localPosition = Vector3.zero;
			mainCamera.transform.eulerAngles = Vector3.zero;

			GameManager.instance.car.GetAnimator().SetBool("WheelSpin",false);

			if (!TutorialManager.instace.onTutorial) {
				BarManager.instance.Visible (true);
			}
			else
			{
				TutorialManager.instace.ActivateHands ();
				BarManager.instance.Visible (false);
				TutorialManager.instace.Invoke ("DeltaTimeToCero",0.38f);
			}

			foreach(GameObject g in objectsToToggle)
				g.SetActive(true);
		}

		state = newState;
	}

	// ---

	void onReset()
	{
		setState(IntroState.WAITING);
	}

	IEnumerator onPlay()
	{
		setState(IntroState.BEGGINING);
		yield return new WaitForSeconds(2.5f);
		setState(IntroState.FINISHED);
	}		

}
