using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFArcade {

	public abstract class IPopup_Revive : Popup
	{
		// --- Interface and methods to call:

		protected override void onShow() {
			if((ArtikFlowArcade.instance.configuration.enableRevive) && (AFBase.Ads.instance.isRewardedVideoAvailable())) {
				GoogleAnalyticsV4.instance.LogEvent("video_impression", "impression", "revive video impression", 0);
			}
		}
		// Can be overridden to know when the popup is shown.
		protected override void onHide() {}		// Can be overridden to know when the popup is hidden.

		// close() must be called when closing the popup. hide() can be used alternativaly to close silently.
		protected void close() { base.onClose(); }
		public override void hide() { base.hide(); }


	}

}