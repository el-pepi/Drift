using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleManager : MonoBehaviour {

	Dictionary<string,ParticleSystem> particles;

	static ParticleManager instance;

	public GameObject[] explosions;

	void Start () {
		instance = this;

		particles = new Dictionary<string, ParticleSystem>();
		foreach(ParticleSystem p in transform.GetComponentsInChildren<ParticleSystem>()) {
			particles.Add(p.transform.name,p);
		}
	}

	public static void EmitParticleAt(string particleName, Vector3 pos, int ammount){
		EmitParticleAt(particleName,pos,1,ammount);
	}
	public static void EmitParticleAt(string particleName, Vector3 pos, int ammount,Transform prnt){
		EmitParticleAt(particleName,pos,1,ammount);
		instance.particles[particleName].transform.parent = prnt;
	}
	public static void EmitParticleAt(string particleName, Vector3 pos,float size, int ammount){
		instance.particles[particleName].transform.localScale = Vector3.one * size;
		instance.particles[particleName].transform.position = pos;
		instance.particles[particleName].Emit(ammount);
	}
	public static void EmitParticleAt(string particleName, Vector3 pos,float size, int ammount, float rotation){
		instance.particles[particleName].transform.localScale = Vector3.one * size;
		instance.particles[particleName].transform.position = pos;
		instance.particles[particleName].transform.rotation = Quaternion.Euler(0,rotation,0);
		instance.particles[particleName].Emit(ammount);
	}
	public static void EmitParticleAt(string particleName, Vector3 pos,float size, int ammount,Color color){
		instance.particles[particleName].startColor = color;
		EmitParticleAt(particleName,pos,size,ammount);
	}
	public static void SpawnExplosion(Vector3 pos,int ind){
		GameObject e = (GameObject)Instantiate(instance.explosions[ind]);
		e.transform.position = pos;
		Destroy(e,3f);
	}
	public static void SpawnExplosion(Vector3 pos,int ind,Transform prnt){
		GameObject e = (GameObject)Instantiate(instance.explosions[ind]);
		e.transform.position = pos;
		e.transform.parent = prnt;
		Destroy(e,3f);
	}

	public static void ReturnParticle(string particleName){
		instance.particles[particleName].transform.parent = instance.transform;
	}
}
