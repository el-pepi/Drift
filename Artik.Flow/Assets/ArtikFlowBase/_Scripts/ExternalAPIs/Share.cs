using UnityEngine;
using System.Collections;
using VoxelBusters.NativePlugins;

namespace AFBase {

public class Share : MonoBehaviour
{
	public static Share instance;

	void Awake()
	{
		instance = this;
	}

	// ----------- Twitter

	public void shareTwitterText(string text)
	{
		if (!gameObject.activeInHierarchy)
			return;
		
		TwitterShareComposer composer = new TwitterShareComposer();
		composer.Text = text;

		NPBinding.Sharing.ShowView(composer, null);
	}

	public void shareTwitterImage(Texture2D image, string text)
	{
		if (!gameObject.activeInHierarchy)
			return;
		
		TwitterShareComposer composer = new TwitterShareComposer();
		composer.Text = text;
		composer.AttachImage(image);

		NPBinding.Sharing.ShowView(composer, null);
	}

	public void shareTwitterScreenshot(string text)
	{
		if (!gameObject.activeInHierarchy)
			return;
		
		TwitterShareComposer composer = new TwitterShareComposer();
		composer.Text = text;
		composer.AttachScreenShot();

		NPBinding.Sharing.ShowView(composer, null);
	}

	// ----------- Facebook

	public void shareFacebookImage(Texture2D image)
	{
		if (!gameObject.activeInHierarchy)
			return;
		
		FBShareComposer composer = new FBShareComposer();
		composer.AttachImage(image);

		NPBinding.Sharing.ShowView(composer, null);
	}

	public void shareFacebookScreenshot()
	{
		if (!gameObject.activeInHierarchy)
			return;
		
		FBShareComposer composer = new FBShareComposer();
		composer.AttachScreenShot();

		NPBinding.Sharing.ShowView(composer, null);
	}

	// ----------- Native

	public void shareNativeText(string text)
	{
		if (!gameObject.activeInHierarchy)
			return;
		
		ShareSheet composer = new ShareSheet();
		composer.Text = text;

		NPBinding.UI.SetPopoverPointAtLastTouchPosition();
		NPBinding.Sharing.ShowView(composer, null);
	}

	public void shareNativeImage(Texture2D image, string text)
	{
		if (!gameObject.activeInHierarchy)
			return;
		
		ShareSheet composer = new ShareSheet();
		composer.Text = text;
		composer.AttachImage(image);

		NPBinding.UI.SetPopoverPointAtLastTouchPosition();
		NPBinding.Sharing.ShowView(composer, null);
	}

	public void shareNativeScreenshot(string text)
	{
		if (!gameObject.activeInHierarchy)
			return;
		
		ShareSheet composer = new ShareSheet();
		composer.Text = text;
		composer.AttachScreenShot();

		NPBinding.UI.SetPopoverPointAtLastTouchPosition();
		NPBinding.Sharing.ShowView(composer, null);
	}

}

}