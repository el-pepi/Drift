using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace AFArcade {
	
public class HiddenLogo : MonoBehaviour
{
	public static HiddenLogo instance;

	[Tooltip("Distancia en pixeles que el dedo se debe mover para considerar el swipe.")]
	[Range(0f, 600f)]
	public float MOVE_THRESHOLD = 10;
	[Tooltip("Velocidad a la que se acomodan las camaras.")]
	[Range(0f, 10f)]
	public float MOVE_SPEED = 1f;

	Camera hiddenCamera;
	Vector2 start_pos;
	Vector2 last_pos;
	bool update_input;
	bool swiping;
	float factor = 1;       // Entre 0 y 1, cuanto porcentaje de pantalla ocuparia el logo.
	float aim_factor = 1;   // A donde mover el factor cuando swiping esta en off.
	float touch_timer;
	int touch_phase;
	string touch_str;

	Camera[] allGameCameras;

	void Awake()
	{
		instance = this;
		hiddenCamera = GetComponentInChildren<Camera>();

		Transform t = transform.Find("UI").Find("Label_Join");
		t.GetComponent<UILabel>().text = Language.get("HiddenLogo.Join");
	}

	void Start()
	{
		ArtikFlowArcade.instance.eventStateChange.AddListener(onArtikFlowStateChange);
		ArtikFlowArcade.instance.eventInitialized.AddListener(updateAvailableCameras);
		
		PopupManager.instance.eventPopupShow.AddListener(onPopupShow);
		PopupManager.instance.eventPopupHide.AddListener(onPopupHide);

		if (ArtikFlowArcade.instance.configuration.storeTarget == ArtikFlowArcadeConfiguration.StoreTarget.FRENCH_PREMIUM || !ArtikFlowArcade.instance.configuration.hiddenLogoEnabled)
			disable();
	}

	void updateAvailableCameras()
	{
		// Update cameras
		allGameCameras = Camera.allCameras;
	}

	void onArtikFlowStateChange(ArtikFlowArcade.State oldstate, ArtikFlowArcade.State newstate)
	{
		if (oldstate == ArtikFlowArcade.State.START_SCREEN)
			disable();

		if (newstate == ArtikFlowArcade.State.START_SCREEN)
		{
			if(ArtikFlowArcade.instance.configuration.storeTarget != ArtikFlowArcadeConfiguration.StoreTarget.FRENCH_PREMIUM && ArtikFlowArcade.instance.configuration.hiddenLogoEnabled)
				enable();
		}
	}

	void onPopupShow(Popup popup)
	{
		disable();
	}

	void onPopupHide(Popup popup)
	{
		if (ArtikFlowArcade.instance.flowState == ArtikFlowArcade.State.START_SCREEN)
			enable();
	}

	void Update()
	{
		bool input_began;
		bool input_pressed;
		bool input_ended;
		Vector2 input_position;

#if UNITY_EDITOR
		input_began = Input.GetMouseButtonDown(0);
		input_pressed = Input.GetMouseButton(0);
		input_ended = Input.GetMouseButtonUp(0);
		input_position = Input.mousePosition;
#else
		input_began = (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
		input_pressed = (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved);
		input_ended = (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);
		input_position = (Input.touchCount > 0) ? Input.GetTouch(0).position : Vector2.zero;
#endif

		if(update_input)
		{
			if (input_began)
			{
				start_pos = input_position;
				last_pos = start_pos;
				swiping = true;
			}
			else if (input_pressed && Vector2.Distance(start_pos, input_position) > MOVE_THRESHOLD)
			{
				factor += (Mathf.Sign(last_pos.x - input_position.x) * (Vector2.Distance(last_pos, input_position) / Screen.width * 1.3f));
				factor = Mathf.Clamp01(factor);
			}
			else if (swiping && input_ended)
			{
				if (factor >= 0.5f)
					aim_factor = 1f;
				else
					aim_factor = 0f;

				swiping = false;
			}

			if (factor == 0)
			{
				if (Input.touchCount == 4 && touch_phase == 0)
				{
					touch_timer += Time.deltaTime;
					touch_str = "v~5z[vxy5]z~";
					if (touch_timer >= 4)
					{
						touch_str += "yv[~5Xvzvc~xv5av";
						touch_phase = 1;
					}
				}
				else if (Input.touchCount == 3 && touch_phase == 1)
					touch_phase = 2;
				else if (Input.touchCount == 2 && touch_phase == 2)
					touch_phase = 3;
				else if (Input.touchCount == 3 && touch_phase == 3)
					touch_phase = 4;
				else if (Input.touchCount == 4 && touch_phase == 4)
					touch_phase = 5;
				else if (Input.touchCount == 0 && touch_phase == 5)
				{
					Transform t = transform.Find("UI").Find("Label_Join");
					t.localScale = new Vector3(2, 2, 2);
					t.GetComponent<UILabel>().text = Caesar(touch_str + "~Xzv5_vvbv~v5av~" + "~hv~v|5cvvv", -21).ToUpper();
					touch_phase = 6;
				}
				else if(Input.touchCount == 0 && touch_phase != 6)
				{
					touch_timer = 0;
					touch_phase = 0;
				}
			}
			else
			{
                if (touch_phase == 6)
				{
					Transform t = transform.Find("UI").Find("Label_Join");
					t.localScale = Vector3.one;
					t.GetComponent<UILabel>().text = Language.get("HiddenLogo.Join");
				}
				touch_timer = 0;
				touch_phase = 0;
			}

		}

		last_pos = input_position;
		updateFactor();
		updateCameras();
	}

	void updateFactor()     // Updates the factor depending on aim_factor
	{
		if (!swiping && factor != aim_factor)
		{
			if (aim_factor > factor)
				factor += Mathf.Min(aim_factor - factor, MOVE_SPEED * Time.deltaTime);
			else
				factor -= Mathf.Min(factor - aim_factor, MOVE_SPEED * Time.deltaTime);
		}
	}

	void updateCameras()    // Updates the cameras' viewports depending on factor
	{
		if(allGameCameras == null)
			return;

		Rect mainCamRect = new Rect(1 - factor, 0, 1, 1);

		foreach(Camera cam in allGameCameras)
		{
			if(cam != null && cam != hiddenCamera)
				cam.rect = mainCamRect;
		}

		if (factor < 1)
		{
			hiddenCamera.gameObject.SetActive(true);
			hiddenCamera.rect = new Rect(-factor, 0, 1, 1);
		}
		else
			hiddenCamera.gameObject.SetActive(false);
	}

	public void enable()
	{
		update_input = true;

		updateAvailableCameras();
    }

	public void disable()
	{
		update_input = false;
		swiping = false;
		aim_factor = 1;
	}

	string Caesar(string source, Int16 shift)
	{
		return "Join!";

		/*
		var maxChar = Convert.ToInt32(char.MaxValue);
		var minChar = Convert.ToInt32(char.MinValue);

		var buffer = source.ToCharArray();

		for (var i = 0; i < buffer.Length; i++)
		{
			var shifted = Convert.ToInt32(buffer[i]) + shift;

			if (shifted > maxChar)
			{
				shifted -= maxChar;
			}
			else if (shifted < minChar)
			{
				shifted += maxChar;
			}

			buffer[i] = Convert.ToChar(shifted);
		}

		return new string(buffer);
		*/
	}

	// --- Callbacks ---

	public void onFacebookClick()
	{
		Application.OpenURL("http://facebook.com/artikgames");
    }

	public void onTwitterClick()
	{
		Application.OpenURL("http://twitter.com/artikgames");
	}

}

}