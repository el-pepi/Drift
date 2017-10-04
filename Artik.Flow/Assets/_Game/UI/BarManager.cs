using UnityEngine;
using System.Collections;

public class BarManager : MonoBehaviour 
{

	public static BarManager instance;
	public BossBar bossBar;
//	public EnergyBar energyBar;

	public Animator dmgAnim;

	void Awake () 
	{
		instance = this;
		bossBar = GetComponentInChildren<BossBar> ();
//		energyBar = GetComponentInChildren<EnergyBar> ();
	}

	public void Reset()
	{
//		energyBar.Reset ();
		bossBar.Reset ();
		BarManager.instance.Visible (false);
	}

	public void Visible(bool isVisible)
	{
//		energyBar.gameObject.SetActive (isVisible);
		bossBar.Visible(isVisible);
	}
}