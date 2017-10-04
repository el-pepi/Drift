using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AFBase;

namespace AFArcade {

public class Arcade_CrossPromo : MonoBehaviour 
{
	bool ready;

	void Start()
	{
		CrossPromo.instance.eventReadyToShow.AddListener(onCrossPromoReadyToShow);
		ArtikFlowArcade.instance.eventStateChange.AddListener(onArtikFlowStateChange);
	}

	void onCrossPromoReadyToShow()
	{
		ready = true;

		if (ArtikFlowArcade.instance.flowState == ArtikFlowArcade.State.START_SCREEN)
		{
			CrossPromo.instance.display();
			ready = false;
		}
	}

	void onArtikFlowStateChange(ArtikFlowArcade.State oldstate, ArtikFlowArcade.State newstate)
	{
		if(newstate == ArtikFlowArcade.State.START_SCREEN && ready)
		{
			CrossPromo.instance.display();
			ready = false;
		}
	}

}

}