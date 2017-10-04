using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AFArcade {

public class GemParticleManager : MonoBehaviour
{
	List<GemParticle> particles = null;
	
	void Awake()
	{
		particles = new List<GemParticle>();

		int i = 0;
		foreach(GemParticle p in transform.GetComponentsInChildren<GemParticle>(true))
		{
			particles.Add(p);
			i++;
		}
	}

	public void deactivate()
	{
		foreach (GemParticle p in particles)
			p.gameObject.SetActive(false);
	}

	public void explode()
	{
		foreach(GemParticle p in particles)
		{
			p.reset();
			p.gameObject.SetActive(true);
		}

		Invoke("travel", 0.2f);
	}

	public void travel()
	{
		foreach (GemParticle p in particles)
			p.travel();
	}

	public void setTarget(Vector3 new_target)
	{
		foreach (GemParticle p in particles)
			p.setTarget(new_target);
	}

}

}