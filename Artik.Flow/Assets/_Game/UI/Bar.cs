using UnityEngine;
using System.Collections;

public abstract class Bar : MonoBehaviour 
{
	[HideInInspector]
	public UIProgressBar slider;

	void Awake()
	{

		slider = this.GetComponent<UIProgressBar> ();
		InitAwake ();
	}

	public void AddEnergy(float amount)
	{
		//slider.value += amount/100;

		Hashtable ht = new Hashtable();

		ht.Add ("from",slider.value);
		ht.Add ("to",slider.value+amount/100);
		ht.Add ("speed",0.60f);
		ht.Add ("onupdate","ChangeValue");
		ht.Add ("oncomplete","CheckFullBar");
		ht.Add ("easytype",iTween.EaseType.easeInOutCubic);
		iTween.ValueTo (this.gameObject,ht);

	}

	public void ChangeValue(float value)
	{
		
		slider.value = value;

	}


	public abstract void InitAwake ();





	protected void ResetValue()
	{
		slider.value = 0;
	}

	public virtual void Reset()
	{
		ResetValue ();
	}

	public void BarVisible(bool isVisible)
	{
		for (int i = 0; i < transform.childCount; i++) 
		{
			transform.GetChild (i).gameObject.SetActive (isVisible);
		}
	}


	protected abstract void OnFullBar();

	public void CheckFullBar()
	{
		if (slider.value == 1)
		{
			OnFullBar ();
		}
	}

}
