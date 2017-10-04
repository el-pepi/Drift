using UnityEngine;
using System.Collections;

public class BeatCounter : MonoBehaviour {

	public AudioSource musicSource;
	public float beatDuration;
	public float[] samples;
	public int actualSample;

	public Animator[] anims;
	public Animator gridAnim;

	float timeToCheck;
	float lastTIme = 0;
	int beat = 0;

	void Start(){
		timeToCheck = beatDuration;
	}

	void Update () {
		if(musicSource.time < lastTIme) {
			actualSample = 0;
			beat = 0;
		}
		lastTIme = musicSource.time;
		if(musicSource.timeSamples >= samples[actualSample] * musicSource.clip.frequency) {
			//timeToCheck += beatDuration;


			actualSample++;
			/*if(actualSample >= samples.Length) {
				actualSample = 0;
			}*/
			foreach(Animator a in anims) {
				a.SetTrigger("Beat");
				//a.Play("GridPulse",0,0.5f);
				//a.GetNextAnimatorClipInfo(0).Length;
			}
		}

		if(musicSource.timeSamples >= beat * beatDuration * musicSource.clip.frequency) {
			beat++;
			gridAnim.SetTrigger("Beat");
		}

		if(Input.GetKeyDown(KeyCode.A)) {
			musicSource.Stop();
			musicSource.Play();
			timeToCheck = 0;
			actualSample = 0;
			beat = 0;
		}


	/*	if(timeToCheck - musicSource.time <= beatDuration) {

			//((timeToCheck - musicSource.time) / beatDuration )/ 2;

			foreach(Animator a in anims) {
				a.Play("GridPulse",0,((timeToCheck - musicSource.time) / beatDuration )/ 2f);
				//a.GetNextAnimatorClipInfo(0).Length;
			}

			timeToCheck += beatDuration;
			if(timeToCheck > musicSource.clip.length) {
				timeToCheck = 0;
			}
		}*/
	}
}
