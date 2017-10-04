using UnityEngine;
using System.Collections;

namespace AFArcade {

public class Popup_Share : IPopup_Share
{
	UITexture screenshotTexture;

	protected override void Awake()
	{
		base.Awake();

		screenshotTexture = transform.Find("Button_Screenshot").GetComponent<UITexture>();

		transform.Find("Label_Title").GetComponent<UILabel>().text = Language.get("Share.ShareYourScore");
		transform.Find("Label_Desc1").GetComponent<UILabel>().text = Language.get("Share.Description").Split('%')[0];
		string hashtag = "#" + Application.productName.Replace(" ", "").ToUpper();
		transform.Find("Label_Hashtag").GetComponent<UILabel>().text = hashtag;
		transform.Find("Label_Hashtag").Find("Label_HashtagShadow").GetComponent<UILabel>().text = hashtag;
		transform.Find("Label_Desc2").GetComponent<UILabel>().text = Language.get("Share.Description").Split('%')[1];
		transform.Find("Button_Share").Find("Label_Share").GetComponent<UILabel>().text = Language.get("Share.Share");
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			base.close();
	}

	protected override void onShow()
	{
		base.onShow();

		screenshotTexture.mainTexture = ScreenshotTaker.instance.getLastScreenshot();
	}

	// --- Callbacks ---

	public void onShare()
	{
		Arcade_Share.instance.shareNativeImageScore(ScreenshotTaker.instance.getLastScreenshot());
	}
	
}

}