using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

namespace VascoGames.MoreGames
{
	public enum TweenDirection
	{
	    None = 0,
	    Left = 1,
	    Right = 2,
	    Up = 4,
	    Down = 8
	}
	
	public class TweenIn : MonoBehaviour
	{
	
	    public bool FadeIn = true;
	    public Ease Easing;
	    public float Speed;
	    public TweenDirection direction = TweenDirection.Up;
	    public float From;
	    public float count = -1;
	
	    public bool DoOnEbable = false;
	
	    void OnEnable()
	    {
	        if (DoOnEbable)
	        {
	            Animate();
	        }
	    }
	
	    public void Animate()
	    {
	      
	        Vector2 position = (transform as RectTransform).anchoredPosition;
	
	        switch (direction)
	        {
	            case TweenDirection.Left:
	                (transform as RectTransform).anchoredPosition += new Vector2(-From, (transform as RectTransform).anchoredPosition.y);
	                break;
	            case TweenDirection.Right:
	                (transform as RectTransform).anchoredPosition += new Vector2(From, (transform as RectTransform).anchoredPosition.y);
	                break;
	            case TweenDirection.Up:
	                (transform as RectTransform).anchoredPosition += new Vector2((transform as RectTransform).anchoredPosition.y, -From);
	                break;
	            case TweenDirection.Down:
	                (transform as RectTransform).anchoredPosition += new Vector2((transform as RectTransform).anchoredPosition.y, From);
	                break;
	            case TweenDirection.None:
	
	                break;
	            default:
	                break;
	        }
	
	        DOTween.To(() => (transform as RectTransform).anchoredPosition, x => (transform as RectTransform).anchoredPosition = x, position, Speed).SetEase(Easing).SetUpdate(true);
	
	        RawImage img = GetComponent<RawImage>();
	        if (FadeIn && img != null)
	        {
	            img.color *= new Color(1, 1, 1, 0);
	            img.DOFade(1, Speed * 2f).SetEase(Easing).SetUpdate(true);
	        }
	    }
	}
}