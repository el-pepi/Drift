using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LocalizationLabel
{
	public string key;
	public UILabel label;

	public void  SetLabel()
	{
		label.text = Language.get (key);
	}
}
