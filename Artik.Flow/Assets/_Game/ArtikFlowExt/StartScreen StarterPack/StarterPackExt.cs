using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
	This class extends the StartScreen, and applies modifications to the StarterPack button.
*/

namespace AFArcade {

public class StarterPackExt : MonoBehaviour 
{
	UIAtlas buttonAtlas;

	Transform starterPack;
	Transform newBack;		// Extra decals

	void Awake()
	{
		starterPack = transform.Find("StarterPack");

		// Modify original items:
		buttonAtlas = (Resources.Load("NGUI/DriftStarterPackButtonAtlas") as GameObject).GetComponent<UIAtlas>();
		
		starterPack.Find("Decals").gameObject.SetActive(false);

		UISprite starterSprite = starterPack.Find("Button_Starter").GetComponent<UISprite>();
		starterSprite.atlas = buttonAtlas;
		starterSprite.spriteName = "Car7";
		starterSprite.type = UIBasicSprite.Type.Simple;
		starterSprite.keepAspectRatio = UIWidget.AspectRatioSource.Free;
		starterSprite.width = 73;
		starterSprite.height = 73;

		starterPack.Find("Sprite_Glow").GetComponent<UISprite>().atlas = buttonAtlas;
		starterPack.Find("Sprite_Glow").GetComponent<UISprite>().spriteName = "ButtonStarterPackGlowingStar";

		// Add new items:
		newBack = Instantiate(Resources.Load("StarterBack") as GameObject).transform;
		newBack.name = "StarterBack";
		newBack.parent = starterPack.Find("Button_Starter");
		newBack.localPosition = Vector3.zero;
		newBack.localScale = Vector3.one;

		// More tweaks
		UILabel timeLabel = starterPack.Find("Label_Starter").GetComponent<UILabel>();
		timeLabel.color = Color.white;
		timeLabel.bitmapFont = newBack.Find("Label_StarterPack").GetComponent<UILabel>().bitmapFont;
		timeLabel.spacingX = -5;
		timeLabel.transform.localPosition = new Vector3(timeLabel.transform.localPosition.x, -35.7f, timeLabel.transform.localPosition.z);
		timeLabel.transform.GetComponent<TweenPosition>().from.y = -35.7f;
		timeLabel.transform.GetComponent<TweenPosition>().to.y = -35.7f;
	}

	void Update()
	{
		// Avoid decals rotation...
		newBack.localPosition = Vector3.zero;
		newBack.rotation = Quaternion.identity;
	}


}

}