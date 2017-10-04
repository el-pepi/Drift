using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace VascoGames.MoreGames
{
	public enum TweenRotationDirection
	{
	    Clockwise,
	    CounterClockwise
	}
	
	public class TweenRotate : MonoBehaviour {
	
	    public float Speed;
	    public TweenRotationDirection direction;
	
	    void OnEnable()
	    {
	        (transform as RectTransform).DORotate(new Vector3(0, 0, direction == TweenRotationDirection.Clockwise ? -360:360), Speed, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear).SetUpdate(true);
	    }
		 
	}
}