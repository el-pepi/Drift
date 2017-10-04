using UnityEngine;
using System.Collections;

namespace AFArcade {

public class Popup_DriftShare : IPopup_Share
{
	UITexture screenshotTexture;

	protected override void Awake()
	{
		base.Awake();

		screenshotTexture = transform.Find("Button_Screenshot").GetComponent<UITexture>();

		string hashtag = "#" + Application.productName.Replace(" ", "").ToUpper();
		transform.Find("Label_Hashtag").GetComponent<UILabel>().text = hashtag;
		transform.Find("Label_Hashtag").Find("Label_HashtagShadow").GetComponent<UILabel>().text = hashtag;
		transform.Find("Button_Share").GetComponentInChildren<UILabel>().text = Language.get("Share.Share");
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			base.close();
	}

	protected override void onShow()
	{
		base.onShow();
		DriftScreenshotTaker.instance.scoreText.text = ArtikFlowArcade.instance.getScore ().ToString();
		screenshotTexture.mainTexture = DriftScreenshotTaker.instance.GetPic ();
	}

	// --- Callbacks ---

	public void onShare()
	{
		Arcade_Share.instance.shareNativeImageScore(DriftScreenshotTaker.instance.GetPic ());
	}
	
}

}