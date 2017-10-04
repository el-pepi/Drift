using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

namespace AFArcade {

public class PopupManager : MonoBehaviour
{
	public static PopupManager instance;

	public class PopupEvent : UnityEvent<Popup> { }
	public PopupEvent eventPopupShow = new PopupEvent();
	public PopupEvent eventPopupHide = new PopupEvent();

	UITexture background;
	Dictionary<Type, Popup> popupDictionary;

	Popup currentPopup;

	void Awake()
	{
		instance = this;

		background = transform.Find("Background").GetComponent<UITexture>();
	}

	void Start()
	{
		// Add additional game popups
		foreach(Popup popupPrefab in ArtikFlowArcade.instance.configuration.extraPopups)
		{
			Popup newPopup = Instantiate(popupPrefab);
            newPopup.transform.parent = transform.parent;
            newPopup.transform.localPosition = popupPrefab.transform.localPosition;
			newPopup.transform.localScale = popupPrefab.transform.localScale;
			newPopup.name = popupPrefab.name;
		}

		Invoke("registerPopups", 0.2f);	// Allow time for popups' initializations
	}

	void registerPopups()
	{
		popupDictionary = new Dictionary<Type, Popup>();
		foreach(Popup popup in transform.parent.gameObject.GetComponentsInChildren<Popup>(true))
		{
			System.Type popupInterface = popup.GetType();
			while(popupInterface.BaseType != null && popupInterface.BaseType != typeof(Popup))
				popupInterface = popupInterface.BaseType;

			popupDictionary.Add(popupInterface, popup);
			popup.eventHide.AddListener(onPopupHide);
		}

		// Hide 'em all
		foreach (Popup p in popupDictionary.Values)
			p.gameObject.SetActive(false);
	}

	void onPopupHide(Popup popup)
	{
		if (currentPopup == popup)
		{
			currentPopup = null;
			TweenAlpha.Begin(background.gameObject, 0.2f, 0f);

			eventPopupHide.Invoke(popup);
		}
		else
			Debug.Log("[WARNING] Invalid popup has emitted a close signal. Emitter: " + popup + ", expected: " + currentPopup);
	}

	// --- API ---

	/// <summary>Attemps to show popup of specified type. Returns if it could be shown successfully or not.</summary>
	public bool showPopup<T>() where T : Popup
	{
		if(currentPopup != null)
		{
			Debug.Log("[WARNING] Skipping popup of type " + typeof(T) + ": The popup " + currentPopup + " is still showing");
			return false;
		}

		Popup popup = popupDictionary[typeof(T)];

		if (popup == null)
		{
			Debug.Log("[WARNING] Couldn't find popup of type: " + typeof(T));
			return false;
		}

		currentPopup = popup;
		TweenAlpha.Begin(background.gameObject, 0.2f, 0.82f);
		popup.show();
		eventPopupShow.Invoke(popup);

		return true;
	}

	/// <summary>Returns the currently active popup, or null.</summary>
	public Popup getCurrentPopup()
	{
		return currentPopup;
	}

}

}