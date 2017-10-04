using UnityEngine;
using System.Collections;

public class Bomb : PickUp
{
	MeshRenderer rend;

	void Awake()
	{
		inGroup = true;
		rend = GetComponent<MeshRenderer> ();
		SetVisible (false);
	}
	public override void OnCollision()
	{
		gameObject.SetActive (false);
		SoundManager.PlayByName("EnemyExplosion");
	}

	public void SetVisible(bool isVisible)
	{
		rend.enabled = isVisible;
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "BombSpawner") 
		{
			if(rend.enabled == false) {
				SetVisible(true);
				ParticleManager.EmitParticleAt("ShitSpawn",transform.position,10);
			}
		}
		if (col.gameObject.CompareTag ("Car")) 
		{
			OnCollision ();
		}
			
	}

	public override void OnSpawn()
	{
		SetVisible (false);
	}
}

