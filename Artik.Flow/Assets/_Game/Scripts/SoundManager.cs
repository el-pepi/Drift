using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {

	AudioSource source;
	static SoundManager instance;

	public AudioClip[] clips;

	Dictionary<string,AudioClip> namedClips;

	public AudioSource driftSound;
	public AudioSource driveSound;
	public AudioSource LaserSound;
	public AudioSource pitchedSource;

	float shootTimer=0;

	// Use this for initialization
	void Start () {
		instance = this;
		source = GetComponent<AudioSource>();
		namedClips = new Dictionary<string, AudioClip>(clips.Length);
		foreach(AudioClip c in clips) {
			namedClips.Add(c.name,c);
		}
	}

	void Update(){
		if(shootTimer <= 0) {
			return;
		}
		shootTimer -= Time.deltaTime;
	}

	public static void PlayClip(AudioClip clip){
		instance.source.PlayOneShot(clip);
	}

	public static void PlayByName(string name){
		if(instance.namedClips.ContainsKey(name) == false) {
			Debug.LogError("No sound with name: " + name);
			return;
		}
		instance.source.PlayOneShot(instance.namedClips[name]);
	}
	public static void PlayByNamePitched(string name,float pitch){
		if(instance.namedClips.ContainsKey(name) == false) {
			Debug.LogError("No sound with name: " + name);
			return;
		}
		instance.pitchedSource.pitch = pitch;
		instance.pitchedSource.PlayOneShot(instance.namedClips[name]);
	}

	public static void DriftVolume(float v){
		instance.driftSound.volume = v;
	}
	public static void DriveVolume(float v){
		instance.driveSound.volume = v;
	}
	public static void SetLaser(bool on){
		if(on) {
			instance.LaserSound.Play();
		} else {
			instance.LaserSound.Stop();
		}
	}
}