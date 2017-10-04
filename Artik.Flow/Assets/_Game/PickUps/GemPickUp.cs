using UnityEngine;
using System.Collections;

public class GemPickUp : PickUp {

	bool canCollide;

	void OnEnable()
	{
		canCollide = true;
	}

	public override void OnCollision()
	{
		if (canCollide) 
		{
			canCollide = false;
			Debug.Log ("Coins" + amount);
			GameManager.instance.eventAddCoins.Invoke (amount);
			ScoreManager.instance.UpdateGems ();
			ParticleManager.EmitParticleAt ("GemSpawn", transform.position + Vector3.up * 5f, 8);
		}
	}

	public override void OnSpawn() 
	{
		canCollide = true;	
	}
}
