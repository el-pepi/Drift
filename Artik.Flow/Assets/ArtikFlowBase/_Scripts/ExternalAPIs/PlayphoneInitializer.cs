using UnityEngine;
using System.Collections;

namespace AFBase {

public class PlayphoneInitializer : MonoBehaviour
{

	void Start()
	{
		if (ArtikFlowBase.instance.configuration.storeTarget != ArtikFlowBaseConfiguration.StoreTarget.PLAYPHONE)
			return;

		PlayPhone.Plugin.IncrementTracking();

		PlayPhone.Plugin.OnInit += () =>
		{
			PlayPhone.Plugin.ShowIcon();
			PlayPhone.Plugin.GetLaunchScreen();
		};

		PlayPhone.Plugin.OnInitError += (error) =>
		{
			//report error message;
		};

		PlayPhone.Plugin.Init();
	}

	void OnApplicationQuit()
	{
		if (ArtikFlowBase.instance.configuration.storeTarget != ArtikFlowBaseConfiguration.StoreTarget.PLAYPHONE)
			return;
		
		PlayPhone.Plugin.DecrementTracking();
	}

}

}