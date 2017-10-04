using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFArcade {

public abstract class GameScreen : MonoBehaviour 
{

    protected virtual void Awake()
    {
        GameScreen replacer = null;

        // Get the immedate subclass of GameScreen for this object
        System.Type thisInterface = GetType();
        while(thisInterface != null && thisInterface.BaseType != typeof(GameScreen))
            thisInterface = thisInterface.BaseType;

        foreach(GameScreen replacementScreen in ArtikFlowArcade.instance.configuration.replacementScreens)
        {
            if(replacementScreen.GetType() != GetType() && replacementScreen.GetType().IsSubclassOf(thisInterface))
            {
                // Replace it!
                replacer = replacementScreen;
                break;
            }
        }

        if(replacer != null)
        {
            print("[INFO] Overriding existing screen '" + gameObject.name + "' with '" + replacer.gameObject.name + "'");

            GameScreen newScreen = Instantiate(replacer);
            newScreen.transform.parent = transform.parent;
            newScreen.transform.position = transform.position;
            newScreen.transform.rotation = transform.rotation;
            newScreen.transform.localScale = transform.localScale;
            newScreen.transform.SetSiblingIndex(transform.GetSiblingIndex());
            newScreen.name = replacer.name;

            Destroy(gameObject);
            return;
        }

        // Screen attachments
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

}

}