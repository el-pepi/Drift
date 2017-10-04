using UnityEngine;
using System.Collections;
using System;

namespace AFArcade {

public class Audio : MonoBehaviour
{
	public static Audio instance;

	private GameObject gameAudio;   // ArtikFlowConfiguration

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		gameAudio = ArtikFlowArcade.instance.gameAudio;
	}

	public void play(GameObject soundObject)
	{
		try
		{
			soundObject.GetComponent<AudioSource>().Play();
		}
		catch(Exception e)
		{
			print("[SOUND] Error playing sound '" + soundObject + "': " + e);
		}
		
	}

	public void playName(string sourceName)
	{
		try
		{
			Transform clip = transform.Find(sourceName);
			if (clip == null && gameAudio != null)
				clip = gameAudio.transform.Find(sourceName);

			clip.GetComponent<AudioSource>().Play();
		}
		catch(Exception e)
		{
            print("[SOUND] Error playing sound '" + sourceName + "': " + e);
		}
	}

	public void stopName(string sourceName)
	{
		try
		{
			Transform clip = transform.Find(sourceName);
			if (clip == null && gameAudio != null)
				clip = gameAudio.transform.Find(sourceName);

			clip.GetComponent<AudioSource>().Stop();
		}
		catch (Exception e)
		{
			Debug.LogWarning("[SOUND] Error stopping sound '" + sourceName + "': " + e);
		}
	}

	public void setVolume(float vol)
	{
		AudioListener.volume = vol;
	}

	public bool isPlaying(string sourceName)
	{
		try
		{
			Transform clip = transform.Find(sourceName);
			if (clip == null && gameAudio != null)
				clip = gameAudio.transform.Find(sourceName);

			return clip.GetComponent<AudioSource>().isPlaying;
		}
		catch (Exception e)
		{
			print("[SOUND] Error getting sound state '" + sourceName + "': " + e);

			return false;
		}

	}

}

}