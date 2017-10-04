using UnityEngine;
using System.Collections;

public class EnergyPickup : PickUp
{

	public float bTime;

	public Renderer active;
	public Renderer inactive;
	public GameObject ball;

	public Transform rayo;

	public override void OnCollision()
	{
		if(!EnergyCircle.instance.onShootMode) 
		{
			EnergyCircle.instance.AddEnergy(amount);
			//BarManager.instance.energyBar.AddEnergy(amount);
			//BarManager.instance.energyBar.anim.SetTrigger("Hit");
		}
		else
		{
			Debug.Log ("Shoot Mode");
			EnergyCircle.instance.AddEnergyWithWeaponActive(amount / 2,bTime);
		}

		active.enabled = false;
		inactive.enabled = true;
		ball.SetActive(false);
		ParticleManager.EmitParticleAt("Energy",ball.transform.position,5);
		SoundManager.PlayByName("Energy");
	}

	public override void OnSpawn()
	{
		active.enabled = true;
		inactive.enabled = false;
		ball.SetActive(true);
	}

	void Update() {
		rayo.Rotate(Vector3.up * 180 * Time.deltaTime);
	}
}