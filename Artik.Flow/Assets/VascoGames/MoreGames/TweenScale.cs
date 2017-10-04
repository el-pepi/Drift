using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace VascoGames.MoreGames
{
	public class TweenScale : MonoBehaviour {
	
	    Tween tween;
	    public float Speed = 1;
	    public float ScaleFactor = 0.8f;
	    public Ease Easing;
	
	    public void ScaleBack()
	    {
	        if (tween != null && tween.IsPlaying()) tween.Kill(false);
	         tween  =  transform.DOScale(1.0f, Speed).SetEase(Easing);
	    }
	
	    public void Scale()
	    {
	        if (tween != null && tween.IsPlaying()) tween.Kill(false);
	        tween =  transform.DOScale(ScaleFactor, Speed).SetEase(Easing);
	    }
	}
}