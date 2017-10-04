using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/* -------------------------------------------------------------
*	ArtikFlowGamemode class.
*	Zamaroht | January, 2017
*
*	ArtikFlowGamemode is the base class for the main ArtikFlow script
*	that implements each gamemode.
------------------------------------------------------------- */

namespace AFBase {

public abstract class ArtikFlowGamemode : MonoBehaviour 
{
    [HideInInspector]
    public UnityEvent eventInitialized = new UnityEvent();
    
}

}