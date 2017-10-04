using System;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_IOS
using UnityEngine.Apple.ReplayKit;
using UnityEngine.iOS;
#endif

namespace AFBase {

public class Replay : MonoBehaviour
{
	public static Replay instance;
	
	[HideInInspector]
	public UnityEvent eventStartedRecording = new UnityEvent();
	[HideInInspector]
	public UnityEvent eventStoppedRecording = new UnityEvent();
	[HideInInspector]
	public UnityEvent eventRecordingAvailable = new UnityEvent();
	[HideInInspector]
	public UnityEvent eventRecordingUnavailable = new UnityEvent();
	[HideInInspector]
	public UnityEvent eventTryingToRecord = new UnityEvent();
	[HideInInspector]
	public UnityEvent eventReplayAvailable = new UnityEvent();

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
#if UNITY_ANDROID
		/*
		eventRecordingAvailable.Invoke();
		*/
		eventRecordingUnavailable.Invoke();
#elif UNITY_IOS
		/*
		if(ReplayKit.APIAvailable)
			eventRecordingAvailable.Invoke();
		*/
		eventRecordingUnavailable.Invoke();
#else
		eventRecordingUnavailable.Invoke();
#endif
	}

	public void startRecording()
	{
#if UNITY_ANDROID
		/*
		try
		{
			print("EVERYPLAY Trying... " + Everyplay.IsRecording());
			if (Everyplay.IsRecording())
				Everyplay.StopRecording();
			else
				Everyplay.StartRecording();
		}
		catch(Exception e)
		{
			print("EVERYPLAY Exception!! " + e.ToString());
		}

		eventTryingToRecord.Invoke();
		Invoke("updateRecStatusDeferred", 0.75f);
		*/
#elif UNITY_IOS
		try
		{
			print("REPLAYKIT Trying... " + ReplayKit.isRecording);
			if (ReplayKit.isRecording)
				ReplayKit.StopRecording();
			else
				ReplayKit.StartRecording();
		}
		catch (Exception e)
		{
			print("REPLAYKIT Exception!! " + e.ToString());
		}

		eventTryingToRecord.Invoke();
		Invoke("updateRecStatusDeferred", 0.75f);
#endif
	}

	void updateRecStatusDeferred()
	{
		Invoke("updateRecStatus", 0.75f);
	}

	void updateRecStatus()
	{

#if UNITY_ANDROID
		/*
		eventRecordingAvailable.Invoke();

		if (Everyplay.IsRecording())
			eventStartedRecording.Invoke();
		*/
#elif UNITY_IOS
		if(ReplayKit.APIAvailable)
			eventRecordingAvailable.Invoke();

		if (ReplayKit.isRecording)
			eventStartedRecording.Invoke();
#endif

	}

	public bool isRecording()
	{
#if UNITY_ANDROID
		/*
		return Everyplay.IsRecording();
		*/
		return false;
#elif UNITY_IOS
		return ReplayKit.isRecording;
#endif
	}

	public void showReplay()
	{
#if UNITY_ANDROID
		/*
		Everyplay.PlayLastRecording();
		*/
#elif UNITY_IOS
		ReplayKit.Preview();
#endif
	}

	public void stopRecording()
	{
#if UNITY_ANDROID
		/*
		if (Everyplay.IsRecording())
			eventReplayAvailable.Invoke();
		
		Everyplay.StopRecording();
		eventStoppedRecording.Invoke();
		*/
#elif UNITY_IOS
		/*
		if (ReplayKit.isRecording)
			eventReplayAvailable.Invoke();

		ReplayKit.StopRecording();
		eventStoppedRecording.Invoke();
		*/
#endif
	}

}

}