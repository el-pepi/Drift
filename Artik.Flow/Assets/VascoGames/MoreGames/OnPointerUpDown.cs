using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace VascoGames.MoreGames
{
	public class OnPointerUpDown : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
	    public UnityEvent PointerDownEvent, PointerUpEvent;
	
	    public void OnPointerDown(PointerEventData eventData)
	    {
	        if (PointerDownEvent != null)
	            PointerDownEvent.Invoke();
	    }
	
	    public void OnPointerUp(PointerEventData eventData)
	    {
	        if (PointerUpEvent != null)
	            PointerUpEvent.Invoke();
	    }
	}
}