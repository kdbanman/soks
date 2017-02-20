using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoundThing : MonoBehaviour {

	double average;

	void Start () {
		average = 1.0;

		string micName = Microphone.devices [0];
		Debug.Log (micName);

		var clip = Microphone.Start (micName, true, 10, 44100);
		var audioSource = GetComponent<AudioSource> ();
		audioSource.clip = clip;
		audioSource.time = 9f;
		audioSource.Play ();
	}
	
	void Update () {
		var audioSrc = GetComponent<AudioSource> ();
		var micPos = Microphone.GetPosition (null);

		// Set the play head 64 samples behind the microphone
		audioSrc.timeSamples = micPos;

		this.transform.localScale = Vector3.one + Vector3.one * Convert.ToSingle(Math.Log(average + 1) * 10);
	}

	void OnAudioFilterRead(float[] buffer, int channels) {
		// buffer lengths are sizes like 2048 and don't seem to change from call to call.
		for (int i = 0; i < buffer.Length; i++) {
			average = average * 0.9999 + Math.Abs(buffer[i]) * 0.0001;
		}
	}
}
