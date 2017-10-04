using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace AFArcade {

public abstract class Popup : MonoBehaviour
{
	protected virtual void onShow() {}       // Can be overridden to know when the popup is shown.
	protected virtual void onHide() {}       // Can be overridden to know when the popup is hidden.

	public class PopupEvent : UnityEvent<Popup> { };
	[HideInInspector]
	public PopupEvent eventHide = new PopupEvent();

	protected virtual void Awake()
    {
        Popup replacer = null;

        // Get the immedate subclass of Popup for this object
        System.Type thisInterface = GetType();
        while(thisInterface != null && thisInterface.BaseType != typeof(Popup))
            thisInterface = thisInterface.BaseType;

        foreach(Popup replacementPopup in ArtikFlowArcade.instance.configuration.replacementPopups)
        {
            if(replacementPopup.GetType() != GetType() && replacementPopup.GetType().IsSubclassOf(thisInterface))
            {
                // Replace it!
                replacer = replacementPopup;
                break;
            }
        }

        if(replacer != null)
        {
            print("[INFO] Overriding existing popup '" + gameObject.name + "' with '" + replacer.gameObject.name + "'");

            Popup newPopup = Instantiate(replacer);
            newPopup.transform.parent = transform.parent;
            newPopup.transform.position = transform.position;
            newPopup.transform.rotation = transform.rotation;
            newPopup.transform.localScale = transform.localScale;
            newPopup.transform.SetSiblingIndex(transform.GetSiblingIndex());
            newPopup.name = replacer.name;

            Destroy(gameObject);
            return;
        }

        // Popup attachments
        foreach(ArtikFlowArcadeConfiguration.ScreenPopupAttachment a in ArtikFlowArcade.instance.configuration.screenPopupAttachments)
        {
            if(System.Type.GetType(a.originalInterface).IsAssignableFrom(GetType()))
            {
                System.Type newScriptType = System.Type.GetType(a.scriptToAttach);
                print("[INFO] Attaching script of type '" + newScriptType.ToString() + "' to '" + gameObject.name + "'");
                gameObject.AddComponent(newScriptType);
            }
        }
    }

	// --- Public ---

	public virtual void show()
	{
		Audio.instance.playName("popup_in");
		gameObject.SetActive(true);
        onShow();
	}

	public virtual void hide()
	{
		gameObject.SetActive(false);
		eventHide.Invoke(this);
        onHide();
	}

	// --- Callbacks ---

	public virtual void onClose()
	{
		Audio.instance.playName("button");
		hide();
	}

}

}